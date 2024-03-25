using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;//singleton pattern instance

    public int Combo { get; set; }
    public float Dist { get; private set; }
    public float BPM { get; private set; }

    public float musicWaitTime;
    public float speed;
    public readonly List<Vector3> groundLineVecs = new List<Vector3>();
    public readonly List<Vector3> skyLineVecs = new List<Vector3>();

    [SerializeField] GameObject laneStructure;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] GameObject longNodePrefab;
    [SerializeField] GameObject skyNodePrefab;
    [SerializeField] GameObject arkNodePrefab;
    [SerializeField] List<GameObject> longNodeHitVFX;
    [SerializeField] List<GameObject> arkNodeHitVFX;
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
        Dist = spawnLine.position.y - judgeLine.position.y;
        InitializeLineVectors();
        //BPM = 120;
        //ArkNodeTest().Forget();
        LoadNodeData("MilkyWayGalaxy");
        PrepareAllNodes();
        Combo = 0;
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
        comboUI.text = Combo.ToString();
    }

    void InitializeLineVectors()
    {
        groundLineVecs.Add(new Vector3(-4.8357f, 0, 0));
        groundLineVecs.Add(new Vector3(-1.6212f, 0, 0));
        groundLineVecs.Add(new Vector3(1.6212f, 0, 0));
        groundLineVecs.Add(new Vector3(4.8357f, 0, 0));
        skyLineVecs.Add(new Vector3(-2.85f, 0, -3));
        skyLineVecs.Add(new Vector3(-0.9f, 0, -3));
        skyLineVecs.Add(new Vector3(0.9f, 0, -3));
        skyLineVecs.Add(new Vector3(2.85f, 0, -3));
    }
    public void LoadNodeData(string songName)
    {
        string path = string.Format("{0}/{1}.txt", Application.dataPath, songName);
        
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
                NodeInfo nodeInfo = new NodeInfo(bool.Parse(nodeData[0]), int.Parse(nodeData[1]) ,float.Parse(nodeData[2]), float.Parse(nodeData[3]));
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

            if(nodeData.IsSkyNode)
            {
                if (nodeData.LongBitNum == -1)
                    nodeObj = Instantiate(skyNodePrefab, laneStructure.transform);
                else
                {
                    nodeObj = Instantiate(arkNodePrefab, laneStructure.transform);
                    LongNode longNodeScript = nodeObj.GetComponentInChildren<LongNode>();
                    longNodeScript.SetBitNum(nodeData.LongBitNum);
                }
            }

            else
            {
                if (nodeData.LongBitNum == -1)
                    nodeObj = Instantiate(nodePrefab, laneStructure.transform);
                else
                {
                    nodeObj = Instantiate(longNodePrefab, laneStructure.transform);
                    LongNode longNodeScript = nodeObj.GetComponentInChildren<LongNode>();
                    longNodeScript.SetBitNum(nodeData.LongBitNum);
                }
            }

            Node nodeScript = nodeObj.GetComponent<Node>();
            nodeScript.SetNodeLine(nodeData.LineNum);
            SetNodePos(nodeScript);
            ActivateNode(nodeData.Bit, nodeObj).Forget();
        }
    }
    private async UniTaskVoid ActivateNode(float bit, GameObject nodeObj)
    {
        float activateBit = bit - (BPM * Dist) / (60 * speed);
        float secPerBit = 60 / BPM;
        float waitTime = activateBit * secPerBit;
        nodeObj.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime + musicWaitTime));
        nodeObj.SetActive(true);
    }
    void SetNodePos(Node node)
    {
        if (node.isSkyNode)
            node.transform.position = spawnLine.position + skyLineVecs[node.Line - 1];
        else
            node.transform.position = spawnLine.position + groundLineVecs[node.Line - 1];      
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
            arkNodeHitVFX[lineNum - 1].SetActive(true);
            laneStructure.transform.DORotate(new Vector3(0, GetTiltState(), 0), 0.4f);//아크노트 플레인 기울임 연출 코드임
        }
           

        else
            longNodeHitVFX[lineNum - 1].SetActive(true);
    }
    public void VFXOff(int lineNum, bool isSkyNode)
    {
        if (isSkyNode)
        {
            arkNodeHitVFX[lineNum - 1].SetActive(false);
            laneStructure.transform.DORotate(new Vector3(0, GetTiltState(), 0), 0.4f);//아크노트 플레인 기울임 연출 코드임
        }
           

        else
            longNodeHitVFX[lineNum - 1].SetActive(false);
    }
    int GetTiltState()
    {
        int result = 0;
        if (arkNodeHitVFX[0].activeSelf) result += 2;
        if (arkNodeHitVFX[1].activeSelf) result += 1;
        if (arkNodeHitVFX[2].activeSelf) result -= 1;
        if (arkNodeHitVFX[3].activeSelf) result -= 2;

        return result;
    }
}
