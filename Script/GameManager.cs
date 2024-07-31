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

    public int PerfectNum { get; private set; }
    public int GreatNum { get; private set; }  
    public int GoodNum { get; private set; }   
    public int BadNum { get; private set; }
    public int MissNum { get; private set; }
    public int EarlyNum { get; private set; }
    public int LateNum { get; private set; }
    public int MaxCombo { get; private set; }
    public string SongName { get; private set; }
    public string Composer {  get; private set; }

    public float musicWaitTime;
    public float speed;
    public readonly List<Vector3> groundLineVecs = new List<Vector3>();
    public readonly List<Vector3> skyLineVecs = new List<Vector3>();

    public enum FinalState { AP, FC, NO }

    public FinalState FinalStateResult {  get; private set; }

    [SerializeField] GameObject laneStructure;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] GameObject longNodePrefab;
    [SerializeField] GameObject skyNodePrefab;
    [SerializeField] GameObject arkNodePrefab;
    [SerializeField] GameObject endMarker;
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
    [SerializeField] Color32 AP_Color;
    [SerializeField] Color32 FC_Color;
    [SerializeField] Color32 NO_Color;
    List<NodeInfo> currentSongDatas = new List<NodeInfo>(20);//contains current songs all nodedatas by using NodeInfo class.

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
        Dist = spawnLine.position.y - judgeLine.position.y;
        InitializeLineVectors();
        InitializeBasicInfo();
        LoadNodeData("Milky_Way_Galaxy_(SIHanatsuka_Remix)");
        PrepareAllNodes();   
    }
    void InitializeBasicInfo()
    {
        Combo = 0;
        PerfectNum = 0;
        GreatNum = 0;
        GoodNum = 0;
        BadNum = 0;
        MissNum = 0;
        MaxCombo = 0;
        EarlyNum = 0;
        LateNum = 0;
        comboUI.text = "0";
        comboUI.color = AP_Color;
        FinalStateResult = FinalState.AP;
        judgeUI.text = "";
        detailJudgeUI.text = "";
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
        string path = string.Format("{0}/{1}.txt", Application.streamingAssetsPath, songName);

        if (File.Exists(path))
        {
            string[] nodeDatas = File.ReadAllLines(path);
            string[] basicData = nodeDatas[0].Split('\t');
            SongName = basicData[0].Replace('_', ' ');
            Composer = basicData[1].Replace('_', ' ');
            BPM = float.Parse(basicData[2]);
            int length = nodeDatas.Length;
            string endBit = "-1";
            for (int i = 1; i < length; i++)
            {
                var s = nodeDatas[i];
                string[] nodeData = s.Split('\t');
                NodeInfo nodeInfo = new NodeInfo(bool.Parse(nodeData[0]), int.Parse(nodeData[1]), float.Parse(nodeData[2]), float.Parse(nodeData[3]));
                endBit = nodeData[2];
                currentSongDatas.Add(nodeInfo);
            }
            NodeInfo endMarker = new NodeInfo(false, -1, float.Parse(endBit), -1);//채보의 끝을 알리는 엔드마커 추가
            currentSongDatas.Add(endMarker);
        }

        else
        {
            Debug.Log("File lost");
        }
    }
    public void PrepareAllNodes()
    {
        int length = currentSongDatas.Count;
        for (int i = 0; i < length; i++)
        {
            NodeInfo nodeData = currentSongDatas[i];
            GameObject nodeObj = null;

            if(nodeData.LineNum == -1)//endMarker
            {
                nodeObj = endMarker;
            }

            else if (nodeData.IsSkyNode)
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

            else if (!nodeData.IsSkyNode)
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
        if (node.Line == -1)
            return;


        if (node.isSkyNode)
            node.transform.position = spawnLine.position + skyLineVecs[node.Line - 1];
        else
            node.transform.position = spawnLine.position + groundLineVecs[node.Line - 1];
    }
    void SetComboUI(int n)
    {
        comboUI.text = Combo.ToString();

        if (n == 1 && FinalStateResult == FinalState.AP)
        {
            comboUI.color = FC_Color;
            FinalStateResult = FinalState.FC;
        }     
        else if (n > 1 && FinalStateResult == FinalState.FC)
        {
            comboUI.color = NO_Color;
            FinalStateResult = FinalState.NO;
        }         
    }
    void SetMaxCombo()
    {
        if (MaxCombo < Combo)
            MaxCombo = Combo;
    }
    public async UniTaskVoid SetJudegeUI(int n)
    {
        switch (n)
        {
            case 0:
                judgeUI.text = "PERFECT";
                judgeUI.colorGradient = perfectColor;
                Combo++;
                PerfectNum++;
                break;
            case 1:
                judgeUI.text = "GREAT";
                judgeUI.colorGradient = greatColor;
                Combo++;
                GreatNum++;
                break;
            case 2:
                judgeUI.text = "GOOD";
                judgeUI.colorGradient = goodColor;
                SetMaxCombo();
                Combo = 0;
                GoodNum++;
                break;
            case 3:
                judgeUI.text = "BAD";
                judgeUI.colorGradient = badColor;
                SetMaxCombo();
                Combo = 0;
                BadNum++;
                break;
            case 4:
                judgeUI.text = "MISS";
                judgeUI.colorGradient = missColor;
                SetMaxCombo();
                Combo = 0;
                MissNum++;
                break;
            default:
                Debug.Log("Wrong number input.");
                break;
        }

        SetComboUI(n);

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
            EarlyNum++;
        }
        else if (diff > 0)
        {
            detailJudgeUI.text = "Late";
            detailJudgeUI.color = Color.red;
            LateNum++;
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
