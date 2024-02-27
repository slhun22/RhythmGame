using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance = null;//singleton pattern instance

    public bool isPlaying;
    [SerializeField] CameraMove cameraMoveScript;
    [SerializeField] GameObject lineNodeObj;
    [SerializeField] GameObject musicCheckPointObj;
    [SerializeField] GameObject progressBarObj;
    [SerializeField] List<Transform> lineParents = new List<Transform>(8);
    [SerializeField] int toplineNum;//1 start
    [SerializeField] int musicCheckNum;//0 start
    [SerializeField] TMP_InputField songNameInput;
    [SerializeField] TMP_InputField bpmInput;
    [SerializeField] AudioSource audiosrc;
    [SerializeField] AudioClip music;
    [SerializeField] TextMeshProUGUI pitchText;
    [SerializeField] GameObject popUpCanvas;
    [SerializeField] TextMeshProUGUI popUpMainText;
    [SerializeField] Image popUpPanel;
    [SerializeField] Color32 successColor;
    [SerializeField] Color32 failColor;
    float BPM;
    bool isProgressBarActive;
    string songName;
    List<NodeInfo> nodeInfos = new List<NodeInfo>();
    Dictionary<GameObject, GameObject> longNodeDic = new Dictionary<GameObject, GameObject>(1000);//Caching container for longNode set

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
        isPlaying = false;
        isProgressBarActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        GenerateNewLine();
        MusicProgressBar();
        SimulateSpeedManage();
    }
    
    void GenerateNewLine()//Create new Lines
    {
        int expectedTopLineNum = (int)cameraMoveScript.maxCenterY + 10;
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
    public void SetBPM() { BPM = float.Parse(bpmInput.text); }
    public void SetSongName() { songName = songNameInput.text; }
    public int GetMusicCheckPosition() { return musicCheckNum - 4; }//return real position of check point
    public void SetMusicCheckPoint(int y) { musicCheckNum = y; }//send position number of the check point to manager
    public void AddLongNodeSet(GameObject startNode, GameObject endNode)
    {
        if (longNodeDic.ContainsKey(startNode))
            PopUpManage(false, 2).Forget();

        longNodeDic.Add(startNode, endNode);
    }

    public void DeleteLongNodeSet(GameObject startNode) { longNodeDic.Remove(startNode); }
    public GameObject GetLongNodeEnd(GameObject startNode) { return longNodeDic[startNode]; }
    public void MusicPlay()
    {
        if (!isPlaying)
        {
            float timePerBit = 60 / BPM;
            float playStartTime = timePerBit * musicCheckNum / 4;
            audiosrc.time = playStartTime;
            audiosrc.Play();
            isPlaying = true;
        }

        else
        {
            audiosrc.Stop();
            isPlaying = false;
        }
    }

    public Vector2 GetTranslatePower() { return Vector2.up * BPM / 60 * 4 * Time.deltaTime * audiosrc.pitch; }
    void MusicProgressBar()
    {
        if(isPlaying)
        {
            if(!isProgressBarActive)
            {
                progressBarObj.transform.position = new Vector3(-1, GetMusicCheckPosition(), 0);
                isProgressBarActive = true;
            }
           
            progressBarObj.transform.Translate(GetTranslatePower());
        }

        else if(!isPlaying && isProgressBarActive)
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

        SetSongName();
        string path = string.Format("{0}/{1}.txt", Application.persistentDataPath, songName);
        if (File.Exists(path)) File.Delete(path);

        if(BPM == 0)
        {
            PopUpManage(false, 0).Forget();
            return;
        }

        string basicSongData = $"{songName}\t{BPM}\n";
        File.AppendAllText(path, basicSongData);
        int length = nodeInfos.Count;
        for (int i = 0; i < length; i++)
        {
            bool isSkyNode = nodeInfos[i].isSkyNode;
            int lineNum = nodeInfos[i].lineNum;
            float bits = nodeInfos[i].bit;
            float longBitNum = nodeInfos[i].longBitNum;
            string data = $"{isSkyNode}\t{lineNum}\t{bits}\t{longBitNum}\n";
            File.AppendAllText(path, data);
        }
        PopUpManage(true, 0).Forget();
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
            Debug.Log($"Song name : {songName}, BPM : {BPM}");
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
            cameraMoveScript.maxCenterY = (beforeTopLineNum - 10) + 20;//automatically generate line by function "GenerateNewLine()", +20 is for longNode offset
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
            bool isSkyNode = nodeInfos[i].isSkyNode;
            int lineNum = nodeInfos[i].lineNum;
            float bit = nodeInfos[i].bit;
            float longBitNum = nodeInfos[i].longBitNum;
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
                (lineNodesLists[lineNum - 1 + skyOffset])[index].SetNodeLongNodeHead();

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
            await UniTask.WaitUntil(() => (int)cameraMoveScript.maxCenterY + 10 <= toplineNum);
            LoadComplete();
            PopUpManage(true, 1).Forget();
        }
        else
        {
            PopUpManage(false, 1).Forget();
        }    
    }
    public void Load()
    {
        LoadUniTask().Forget();
    }


    /// <param name="mode">[0 : save] [1 : load] [2 : longnodeError]</param>
    async UniTaskVoid PopUpManage(bool isSuccess, int mode)
    {
        if (isSuccess)
        {
            if(mode == 0)
                popUpMainText.text = "Save Success";
            else
                popUpMainText.text = "Load Success";

            popUpMainText.color = Color.blue;
            popUpPanel.color = successColor;
        }
        else
        {
            if (mode == 0)
                popUpMainText.text = "Save Failed (BPM Unknown)";
            else if(mode == 1)
                popUpMainText.text = "Load Failed (Location Unknown)";
            else if(mode == 2)
                popUpMainText.text = "LongNode Error. Please Restart";

            popUpMainText.color = Color.red;
            popUpPanel.color = failColor;
        }

        popUpCanvas.SetActive(true);

        await UniTask.WaitUntil(() => Input.anyKeyDown);//enter 치면 error나니 주의
        popUpCanvas.SetActive(false);
    }
}
