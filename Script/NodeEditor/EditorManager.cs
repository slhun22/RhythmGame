using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance = null;//singleton pattern instance

    public bool IsPlaying { get; private set; }
    public bool IsPause { get; private set; }
    [SerializeField] CameraMove cameraMoveScript;
    [SerializeField] GameObject lineNodeObj;
    [SerializeField] GameObject musicCheckPointObj;
    [SerializeField] GameObject progressBarObj;
    [SerializeField] List<Transform> lineParents = new List<Transform>(8);
    [SerializeField] int toplineNum;//1 start
    [SerializeField] int musicCheckNum;//0 start
    [SerializeField] TMP_InputField songNameInput;
    [SerializeField] TMP_InputField bpmInput;
    //[SerializeField] TMP_InputField offsetInput;
    [SerializeField] AudioSource audiosrc;
    [SerializeField] AudioClip music;
    [SerializeField] TextMeshProUGUI pitchText;
    [SerializeField] GameObject popUpCanvas;
    [SerializeField] TextMeshProUGUI popUpMainText;
    [SerializeField] Image popUpPanel;
    [SerializeField] Color32 successColor;
    [SerializeField] Color32 failColor;
    float BPM;
    //float musicOffset;  
    bool isProgressBarActive;
    string songName;
    List<NodeInfo> nodeInfos = new List<NodeInfo>();
    Dictionary<GameObject, GameObject> longNodeDic = new Dictionary<GameObject, GameObject>(1000);//Caching container for longNode set
    enum Mode { Save, Load, Error }

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (instance == null)
        {
            instance = this;
        }

        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        toplineNum = 10;
        musicCheckNum = 0;
        audiosrc.clip = music;
        IsPlaying = false;
        IsPause = false;
        isProgressBarActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        GenerateNewLine();
        MusicPlay();
        MusicProgressBar();
        SimulateSpeedManage();
    }
    
    void GenerateNewLine()//Create new Lines
    {
        int expectedTopLineNum = (int)cameraMoveScript.MaxCenterY + 10;
        while (expectedTopLineNum > toplineNum)
        {
            Instantiate(lineNodeObj, new Vector3(-8, toplineNum - 4, 0), Quaternion.identity, lineParents[0]);
            Instantiate(lineNodeObj, new Vector3(-6, toplineNum - 4, 0), Quaternion.identity, lineParents[1]);
            Instantiate(lineNodeObj, new Vector3(-4, toplineNum - 4, 0), Quaternion.identity, lineParents[2]);
            Instantiate(lineNodeObj, new Vector3(-2, toplineNum - 4, 0), Quaternion.identity, lineParents[3]);

            Instantiate(lineNodeObj, new Vector3(0.5f, toplineNum - 4, 0), Quaternion.identity, lineParents[4]);
            Instantiate(lineNodeObj, new Vector3(2.5f, toplineNum - 4, 0), Quaternion.identity, lineParents[5]);
            Instantiate(lineNodeObj, new Vector3(4.5f, toplineNum - 4, 0), Quaternion.identity, lineParents[6]);
            Instantiate(lineNodeObj, new Vector3(6.5f, toplineNum - 4, 0), Quaternion.identity, lineParents[7]);

            Instantiate(musicCheckPointObj, new Vector3(-9.5f, toplineNum - 4, 0), Quaternion.identity, lineParents[8]);
            toplineNum++;
        }
    }
    //public void SetOffset() { musicOffset = float.Parse(offsetInput.text); }
    public int GetMusicCheckPosition() { return musicCheckNum - 4; }//return real position of check point
    public void SetMusicCheckPoint(int y) { musicCheckNum = y; }//send position number of the check point to manager
    public void AddLongNodeSet(GameObject startNode, GameObject endNode)
    {
        if (longNodeDic.ContainsKey(startNode))
            PopUpManage(false, Mode.Error).Forget();

        longNodeDic.Add(startNode, endNode);
    }

    public void DeleteLongNodeSet(GameObject startNode) { longNodeDic.Remove(startNode); }
    public GameObject GetLongNodeEnd(GameObject startNode) { return longNodeDic[startNode]; }
    public void MusicPlay()
    {
        if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!IsPlaying)
            {
                float timePerBit = 60 / BPM;
                float playStartTime = 0.15f + timePerBit * musicCheckNum / 4;
                audiosrc.time = playStartTime;
                audiosrc.Play();
                IsPlaying = true;
            }
            else
            {
                audiosrc.Stop();
                IsPlaying = false;
                IsPause = false;
            }
        }  
        
        else if(IsPlaying && Input.GetKeyDown(KeyCode.P))
        {
            if(!IsPause)
            {
                audiosrc.Pause();
                IsPause = true;
            }
            else
            {
                audiosrc.UnPause();
                IsPause = false;
            }
        }
    }

    public Vector2 GetTranslatePower() { return Vector2.up * BPM / 60 * 4 * Time.deltaTime * audiosrc.pitch; }
    void MusicProgressBar()
    {
        if(IsPlaying)
        {
            if(!isProgressBarActive)
            {
                progressBarObj.transform.position = new Vector3(-1, GetMusicCheckPosition(), 0);
                isProgressBarActive = true;
            }
           
            if(!IsPause)
                progressBarObj.transform.Translate(GetTranslatePower());
        }

        else if(!IsPlaying && isProgressBarActive)
        {
            progressBarObj.transform.position = new Vector3(-1, -9, 0);
            isProgressBarActive = false;
        }       
    }

    public void SimulateSpeedManage()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (audiosrc.pitch <= 0.2f)
                audiosrc.pitch = 0.2f;

            else
            {
                audiosrc.pitch -= 0.1f;
                pitchText.text = $"Speed : {audiosrc.pitch}";
            }
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (audiosrc.pitch >= 1)
                audiosrc.pitch = 1f;

            else
            {
                audiosrc.pitch += 0.1f;
                pitchText.text = $"Speed : {audiosrc.pitch}";
            }
        }
    }

    public void Save()
    {
        List<NodeInfo> nodeInfos = new List<NodeInfo>();
        List<List<EditorNode>> lineNodesLists = new List<List<EditorNode>>();
        for (int i = 0; i < 8; i++)
        {
            var lineNodesList = new List<EditorNode>();
            var lineNodes = lineParents[i].GetComponentsInChildren<EditorNode>();

            for (int j = 0; j < lineNodes.Length; j++)
                lineNodesList.Add(lineNodes[j]);

            lineNodesLists.Add(lineNodesList);
        }

        for(int i = 0; i < toplineNum; i++)     
            for(int j = 0; j < 8; j++)
                if ((lineNodesLists[j])[i].isSelected || (lineNodesLists[j])[i].isLongNode)
                    nodeInfos.Add(ExtractNodeInfo((lineNodesLists[j])[i]));

        songName = songNameInput.text;
        string path = string.Format("{0}/{1}.txt", Application.persistentDataPath, songName);
        if (File.Exists(path)) File.Delete(path);

        if(float.TryParse(bpmInput.text, out BPM))
            BPM = float.Parse(bpmInput.text);
        else
        {
            PopUpManage(false, Mode.Save).Forget();
            return;
        }

        string basicSongData = $"{songName}\t{BPM}\n";
        File.AppendAllText(path, basicSongData);
        int length = nodeInfos.Count;
        for (int i = 0; i < length; i++)
        {
            bool isSkyNode = nodeInfos[i].IsSkyNode;
            int lineNum = nodeInfos[i].LineNum;
            float bits = nodeInfos[i].Bit;
            float longBitNum = nodeInfos[i].LongBitNum;
            string data = $"{isSkyNode}\t{lineNum}\t{bits}\t{longBitNum}\n";
            File.AppendAllText(path, data);
        }
        PopUpManage(true, Mode.Save).Forget();
    }

    NodeInfo ExtractNodeInfo(EditorNode editorNode)
    {
        int lineNum = 0;       
        float nodePosX = editorNode.gameObject.transform.position.x;

        bool isSkyNode = false;
        if (nodePosX > 0) 
            isSkyNode = true;

        if (nodePosX == -8 || nodePosX == 0.5f) lineNum = 1;
        if (nodePosX == -6 || nodePosX == 2.5f) lineNum = 2;
        if (nodePosX == -4 || nodePosX == 4.5f) lineNum = 3;
        if (nodePosX == -2 || nodePosX == 6.5f) lineNum = 4;

        float editorPosY = editorNode.gameObject.transform.position.y; //-4 start
        float cumulatedBit = editorPosY / 4 + 1;
        float longBitNum = editorNode.longBitNum;
        NodeInfo nodeInfo = new NodeInfo(isSkyNode, lineNum, cumulatedBit, longBitNum);

        return nodeInfo;
    }
    bool LoadStructure()
    {
        nodeInfos.Clear();
        songName = songNameInput.text;
        string path = string.Format("{0}/{1}.txt", Application.persistentDataPath, songName);
        if(File.Exists(path))
        {
            string[] datas = File.ReadAllLines(path);
            string[] s;
            int lineNum;
            bool isSkyNode;
            float bit, longBitNum, maxbit = 0;
            string[] basicData = datas[0].Split('\t');
            songName = basicData[0];
            BPM = float.Parse(basicData[1]);
            bpmInput.text = BPM.ToString();
            int length = datas.Length;
            for(int i = 1; i < length; i++)
            {
                s = datas[i].Split('\t');
                isSkyNode = bool.Parse(s[0]);
                lineNum = int.Parse(s[1]);
                bit = float.Parse(s[2]);
                longBitNum = float.Parse(s[3]);
                nodeInfos.Add(new NodeInfo(isSkyNode, lineNum, bit, longBitNum));
                if (bit > maxbit) maxbit = bit;
            }
            //(toplinenum 1) == (-4 of position y)
            int beforeTopLineNum = (int)(maxbit * 4) + 1;
            cameraMoveScript.MaxCenterY = (beforeTopLineNum - 10) + 20;//automatically generate line by function "GenerateNewLine()", +20 is for longNode offset
            return true;
        }
       
        else
        {
            Debug.Log("No matching file");
            return false;
        }
    }

    void LoadComplete()
    {
        List<List<EditorNode>> lineNodesLists = new List<List<EditorNode>>();
        for (int i = 0; i < 8; i++)
        {
            var lineNodesList = new List<EditorNode>();
            var lineNodes = lineParents[i].GetComponentsInChildren<EditorNode>();

            for (int j = 0; j < lineNodes.Length; j++)
                lineNodesList.Add(lineNodes[j]);

            lineNodesLists.Add(lineNodesList);
        }

        int length = nodeInfos.Count;
        for (int i = 0; i < length; i++)
        {
            bool isSkyNode = nodeInfos[i].IsSkyNode;
            int lineNum = nodeInfos[i].LineNum;
            float bit = nodeInfos[i].Bit;
            float longBitNum = nodeInfos[i].LongBitNum;
            int index = (int)(bit * 4);
            int nodeLength = (int)(longBitNum * 4);
            int skyOffset = 0;
            if (isSkyNode)
                skyOffset = 4;

            if(longBitNum == -1)//normal node case
                (lineNodesLists[lineNum - 1 + skyOffset])[index].SetNodeSelectMode();
           
            else//long node case
            {
                var startNode = (lineNodesLists[lineNum - 1 + skyOffset])[index].gameObject;
                (lineNodesLists[lineNum - 1 + skyOffset])[index].SetNodeLongNodeHead(longBitNum);

                for (int j = 0; j < nodeLength; j++)
                    (lineNodesLists[lineNum - 1 + skyOffset])[++index].SetNodeLongNodeTail();

                var endNode = (lineNodesLists[lineNum - 1 + skyOffset])[index].gameObject;
                AddLongNodeSet(startNode, endNode);
            }            
        }
    }
    async UniTaskVoid LoadUniTask()
    {
        bool loadStructureFinished = LoadStructure();
        if(loadStructureFinished)
        {
            await UniTask.WaitUntil(() => (int)cameraMoveScript.MaxCenterY + 10 <= toplineNum);
            LoadComplete();
            PopUpManage(true, Mode.Load).Forget();
        }
        else
        {
            PopUpManage(false, Mode.Load).Forget();
        }    
    }
    public void Load()
    {
        LoadUniTask().Forget();
    }

    async UniTaskVoid PopUpManage(bool isSuccess, Mode mode)
    {
        if (isSuccess)
        {
            if(mode == Mode.Save)
                popUpMainText.text = "Save Success";
            else
                popUpMainText.text = "Load Success";

            popUpMainText.color = Color.blue;
            popUpPanel.color = successColor;
        }
        else
        {
            if (mode == Mode.Save)
                popUpMainText.text = "Save Failed (BPM Unknown)";
            else if(mode == Mode.Load)
                popUpMainText.text = "Load Failed (Location Unknown)";
            else if(mode == Mode.Error)
                popUpMainText.text = "LongNode Error. Please Restart";

            popUpMainText.color = Color.red;
            popUpPanel.color = failColor;
        }

        popUpCanvas.SetActive(true);

        await UniTask.WaitUntil(() => Input.anyKeyDown);//enter 치면 error나니 주의
        popUpCanvas.SetActive(false);
    }
}
