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
    [SerializeField] List<Transform> lineParents = new List<Transform>(4);
    [SerializeField] int toplineNum;//1 start
    [SerializeField] int musicCheckNum;//0 start
    [SerializeField] TMP_InputField songNameInput;
    [SerializeField] TMP_InputField bpmInput;
    [SerializeField] AudioSource audiosrc;
    [SerializeField] AudioClip music;
    [SerializeField] TextMeshProUGUI pitchText;
    float BPM;
    bool isProgressBarActive;
    string songName;
    List<NodeInfo> nodeInfos = new List<NodeInfo>(2000);
    Dictionary<GameObject, GameObject> longNodeDic = new Dictionary<GameObject, GameObject>(1000);//Caching variable for longNode set

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
            Instantiate(lineNodeObj, new Vector3(-7, toplineNum - 4, 0), Quaternion.identity, lineParents[0]);
            Instantiate(lineNodeObj, new Vector3(-3, toplineNum - 4, 0), Quaternion.identity, lineParents[1]);
            Instantiate(lineNodeObj, new Vector3(1, toplineNum - 4, 0), Quaternion.identity, lineParents[2]);
            Instantiate(lineNodeObj, new Vector3(5, toplineNum - 4, 0), Quaternion.identity, lineParents[3]);
            Instantiate(musicCheckPointObj, new Vector3(-9.5f, toplineNum - 4, 0), Quaternion.identity, lineParents[4]);
            toplineNum++;
        }
    }
    public void SetBPM() { BPM = float.Parse(bpmInput.text); }
    public void SetSongName() { songName = songNameInput.text; }
    public int GetMusicCheckPosition() { return musicCheckNum - 4; }//return real position of check point
    public void SetMusicCheckPoint(int y) { musicCheckNum = y; }//send position number of the check point to manager
    public void AddLongNodeSet(GameObject startNode, GameObject endNode) { longNodeDic.Add(startNode, endNode); }
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
        List<NodeInfo> nodeInfos = new List<NodeInfo>(2000);
        var line1Node = lineParents[0].GetComponentsInChildren<EditorNode>();
        var line2Node = lineParents[1].GetComponentsInChildren<EditorNode>();
        var line3Node = lineParents[2].GetComponentsInChildren<EditorNode>();
        var line4Node = lineParents[3].GetComponentsInChildren<EditorNode>();

        for(int i = 0; i < toplineNum; i++)
        {
            //save 과정에서 롱노트 detect가 잘 안됨
            if (line1Node[i].isSelected || line1Node[i].isLongNode) nodeInfos.Add(ExtractNodeInfo(line1Node[i]));
            if (line2Node[i].isSelected || line1Node[i].isLongNode) nodeInfos.Add(ExtractNodeInfo(line2Node[i]));
            if (line3Node[i].isSelected || line1Node[i].isLongNode) nodeInfos.Add(ExtractNodeInfo(line3Node[i]));
            if (line4Node[i].isSelected || line1Node[i].isLongNode) nodeInfos.Add(ExtractNodeInfo(line4Node[i]));
        }

        SetSongName();
        string path = string.Format("{0}/{1}.txt", Application.persistentDataPath, songName);
        if (File.Exists(path)) File.Delete(path);
        string basicSongData = $"{songName} {BPM}\n";
        File.AppendAllText(path, basicSongData);
        int length = nodeInfos.Count;
        for (int i = 0; i < length; i++)
        {
            int lineNum = nodeInfos[i].lineNum;
            float bits = nodeInfos[i].bit;
            float longBitNum = nodeInfos[i].longBitNum;
            string data = $"{lineNum} {bits} {longBitNum}\n";
            File.AppendAllText(path, data);
        }
    }

    NodeInfo ExtractNodeInfo(EditorNode editorNode)
    {
        int lineNum = 0;
        switch ((int)editorNode.gameObject.transform.position.x)
        {
            case -7:
                lineNum = 1;
                break;
            case -3:
                lineNum = 2;
                break;
            case 1:
                lineNum = 3;
                break;
            case 5:
                lineNum = 4;
                break;
            default:
                Debug.Log("Wrong lineNum position detected!!");
                break;
        }

        float editorPosY = editorNode.gameObject.transform.position.y; //-4 start
        float cumulatedBit = editorPosY / 4 + 1;
        float longBitNum = editorNode.longBitNum;
        NodeInfo nodeInfo = new NodeInfo(lineNum, cumulatedBit, longBitNum);

        return nodeInfo;
    }
    void LoadStructure()
    {
        string path = string.Format("{0}/{1}.txt", Application.persistentDataPath, songName);
        if(File.Exists(path))
        {
            string[] datas = File.ReadAllLines(path);
            string[] s;
            int lineNum;
            float bit = 0;
            float longBitNum = -1;
            float maxbit = 0;
            string[] basicData = datas[0].Split(' ');
            songName = basicData[0];
            BPM = float.Parse(basicData[1]);
            Debug.Log($"Song name : {songName}, BPM : {BPM}");

            for(int i = 1; i < datas.Length; i++)
            {
                s = datas[i].Split(' ');
                lineNum = int.Parse(s[0]);
                bit = float.Parse(s[1]);
                longBitNum = float.Parse(s[2]);
                nodeInfos.Add(new NodeInfo(lineNum, bit, longBitNum));
                if (bit > maxbit) maxbit = bit;
            }
            //(toplinenum 1) == (-4 of position y)
            int beforeTopLineNum = (int)(maxbit * 4) + 1;
            cameraMoveScript.maxCenterY = beforeTopLineNum - 10;//automatically generate line by function "GenerateNewLine()"
        }
       
        else
        {
            Debug.Log("No matching file");
        }
    }

    void LoadComplete()
    {
        var line1Node = lineParents[0].GetComponentsInChildren<EditorNode>();
        var line2Node = lineParents[1].GetComponentsInChildren<EditorNode>();
        var line3Node = lineParents[2].GetComponentsInChildren<EditorNode>();
        var line4Node = lineParents[3].GetComponentsInChildren<EditorNode>();

        for (int i = 0; i < nodeInfos.Count; i++)
        {
            int lineNum = nodeInfos[i].lineNum;
            float bit = nodeInfos[i].bit;
            int index = (int)(bit * 4);
            if (lineNum == 1) line1Node[index].SetNodeSelectMode();
            else if (lineNum == 2) line2Node[index].SetNodeSelectMode();
            else if (lineNum == 3) line3Node[index].SetNodeSelectMode();
            else if (lineNum == 4) line4Node[index].SetNodeSelectMode();
            else Debug.Log("Wrong lineNum");
        }
    }

    async UniTaskVoid LoadUniTask()
    {
        LoadStructure();
        await UniTask.WaitUntil(() => (int)cameraMoveScript.maxCenterY + 10 <= toplineNum);
        LoadComplete();
    }
    public void Load()
    {
        LoadUniTask().Forget();
    }
}
