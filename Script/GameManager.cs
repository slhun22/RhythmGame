using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;//singleton pattern instance

   
    public float musicWaitTime;
    public float speed;
    public int combo { get; set; }
    public float dist { get; private set; }
    public float BPM { get; private set; }
    [SerializeField] GameObject laneStructure;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] GameObject longNodePrefab;
    [SerializeField] GameObject skyNodePrefab;
    [SerializeField] GameObject arkNodePrefab;
    [SerializeField] List<GameObject> longNodeHitVFXs;
    [SerializeField] List<GameObject> arkNodeHitVFXs;
    [SerializeField] Transform spawnLine;
    [SerializeField] Transform judgeLine;
    [SerializeField] TextMeshProUGUI comboUI;
    [SerializeField] TextMeshProUGUI judgeUI;
    [SerializeField] TextMeshProUGUI detailJudgeUI;
    [SerializeField] VertexGradient perfectColor;
    [SerializeField] VertexGradient greatColor;
    [SerializeField] VertexGradient goodColor;
    [SerializeField] VertexGradient badColor;
    [SerializeField] VertexGradient missColor;
    const float LINE1_POS_X = -4.8357f;
    const float LINE2_POS_X = -1.6212f;
    const float LINE3_POS_X = 1.6212f;
    const float LINE4_POS_X = 4.8357f;
    const float SKY_LINE1_POS_X = -2.85f;
    const float SKY_LINE2_POS_X = -0.9f;
    const float SKY_LINE3_POS_X = 0.9f;
    const float SKY_LINE4_POS_X = 2.85f;
    const float SKY_LINE_POS_Y = -3.0f; 
    
    List<NodeInfo> currentSongDatas = new List<NodeInfo>(20);//contains current songs all nodedatas by using NodeInfo class.
    string songname;
   
   
    private void Awake()
    {
        Application.targetFrameRate = 60;

        if(instance == null)
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
        SetDist();
        BPM = 120;
        ArkNodeTest().Forget();
        //LoadNodeData("LongNode1");
        //PrepareAllNodes();
        combo = 0;
        judgeUI.text = "";
        detailJudgeUI.text = "";
    }

    private async UniTaskVoid ArkNodeTest()
    {
        while(true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            var arkNodeObj = Instantiate(arkNodePrefab);
            Node arkNodeScript = arkNodeObj.GetComponent<Node>();
            arkNodeScript.SetNodeLine(Random.Range(1, 5));
            SetNodePos(arkNodeScript);
        }
    }


    // Update is called once per frame
    void Update()
    {
        comboUI.text = combo.ToString();
    }
    void SetDist()
    {
        dist = spawnLine.position.y - judgeLine.position.y;
    }
    public void LoadNodeData(string songName)
    {
        string path = string.Format("{0}/{1}.txt", Application.persistentDataPath, songName);
        
        if(File.Exists(path))
        {
            string[] nodeDatas = File.ReadAllLines(path);
            string[] basicData = nodeDatas[0].Split('\t');
            songname = basicData[0];
            BPM = float.Parse(basicData[1]);
            int length = nodeDatas.Length;
            for (int i = 1; i < length; i++)
            {
                var s = nodeDatas[i];
                string[] nodeData = s.Split('\t');
                NodeInfo nodeInfo = new NodeInfo(int.Parse(nodeData[0]), float.Parse(nodeData[1]), float.Parse(nodeData[2]));
                currentSongDatas.Add(nodeInfo);
            }
        }

        else
        {
            Debug.Log("File lost");
        }      
    }

    public void PrepareAllNodes()
    {
        int length = currentSongDatas.Count;
        for(int i = 0; i < length; i++)
        {
            NodeInfo nodeData = currentSongDatas[i];
            GameObject nodeObj;
            if (nodeData.longBitNum == -1)
                nodeObj = Instantiate(nodePrefab, laneStructure.transform);
            else
            {
                nodeObj = Instantiate(longNodePrefab, laneStructure.transform);
                LongNode longNodeScript = nodeObj.GetComponentInChildren<LongNode>();
                longNodeScript.SetBitNum(nodeData.longBitNum);
            }

            Node nodeScript = nodeObj.GetComponent<Node>();
            nodeScript.SetNodeLine(nodeData.lineNum);
            SetNodePos(nodeScript);
            ActivateNode(nodeData.bit, nodeObj).Forget();
        }
    }
    private async UniTaskVoid ActivateNode(float bit, GameObject nodeObj)
    {
        float activateBit = bit - (BPM * dist) / (60 * speed);
        float secPerBit = 60 / BPM;
        float waitTime = activateBit * secPerBit;
        nodeObj.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime + musicWaitTime));
        nodeObj.SetActive(true);
    }
    void SetNodePos(Node node)
    {
        switch (node.line)
        {
            case 1:
                if(node.isSkyNode) node.transform.position = spawnLine.position + new Vector3(SKY_LINE1_POS_X, 0, SKY_LINE_POS_Y);//sky lane1
                else node.transform.position = spawnLine.position + new Vector3(LINE1_POS_X, 0, 0);//lane1
                break;
            case 2:
                if (node.isSkyNode) node.transform.position = spawnLine.position + new Vector3(SKY_LINE2_POS_X, 0, SKY_LINE_POS_Y);//sky lane2
                else node.transform.position = spawnLine.position + new Vector3(LINE2_POS_X, 0, 0);//lane2
                break;
            case 3:
                if (node.isSkyNode) node.transform.position = spawnLine.position + new Vector3(SKY_LINE3_POS_X, 0, SKY_LINE_POS_Y);//sky lane3
                else node.transform.position = spawnLine.position + new Vector3(LINE3_POS_X, 0, 0);//lane3
                break;
            case 4:
                if (node.isSkyNode) node.transform.position = spawnLine.position + new Vector3(SKY_LINE4_POS_X, 0, SKY_LINE_POS_Y);//sky lane4
                else node.transform.position = spawnLine.position + new Vector3(LINE4_POS_X, 0, 0);//lane4
                break;
        }
    }
    public async UniTaskVoid SetJudegeUI(int n)
    {
        switch(n)
        {
            case 0 :
                judgeUI.text = "PERFECT";
                judgeUI.colorGradient = perfectColor;
                break;
            case 1:
                judgeUI.text = "GREAT";
                judgeUI.colorGradient = greatColor;
                break;
            case 2:
                judgeUI.text = "GOOD";
                judgeUI.colorGradient = goodColor;
                break;
            case 3:
                judgeUI.text = "BAD";
                judgeUI.colorGradient = badColor;
                break;
            case 4:
                judgeUI.text = "MISS";
                judgeUI.colorGradient = missColor;
                break;
            default:
                Debug.Log("Wrong number input.");
                break;
        }

        for (int i = 1; i < 5; i++)
        {
            judgeUI.fontSize = 11 * i;
            await UniTask.Delay(TimeSpan.FromMilliseconds(2));
        }
    }

    public async UniTaskVoid SetDetailJudgeUI(float diff)
    {
        if (diff < 0)
        {
            detailJudgeUI.text = "Early";
            detailJudgeUI.color = Color.blue;
        }
        else if (diff > 0)
        {
            detailJudgeUI.text = "Late";
            detailJudgeUI.color = Color.red;
        }
        else
            detailJudgeUI.text = "";

        for (int i = 1; i < 5; i++)
        {
            detailJudgeUI.fontSize = 8 * i;
            await UniTask.Delay(TimeSpan.FromMilliseconds(2));
        }
    }

    public void ClearDetailJudge()
    {
        detailJudgeUI.text = "";
    }

    public void VFXOn(int lineNum, bool isSkyNode)
    {
        if (isSkyNode)
        {
            arkNodeHitVFXs[lineNum - 1].SetActive(true);
            laneStructure.transform.DORotate(new Vector3(0, GetTiltState(), 0), 0.4f);//아크노트 플레인 기울임 연출 코드임
        }
           

        else
            longNodeHitVFXs[lineNum - 1].SetActive(true);
    }
    public void VFXOff(int lineNum, bool isSkyNode)
    {
        if (isSkyNode)
        {
            arkNodeHitVFXs[lineNum - 1].SetActive(false);
            laneStructure.transform.DORotate(new Vector3(0, GetTiltState(), 0), 0.4f);//아크노트 플레인 기울임 연출 코드임
        }
           

        else
            longNodeHitVFXs[lineNum - 1].SetActive(false);
    }
    int GetTiltState()
    {
        int result = 0;
        if (arkNodeHitVFXs[0].activeSelf) result += 2;
        if (arkNodeHitVFXs[1].activeSelf) result += 1;
        if (arkNodeHitVFXs[2].activeSelf) result -= 1;
        if (arkNodeHitVFXs[3].activeSelf) result -= 2;

        return result;
    }
}
