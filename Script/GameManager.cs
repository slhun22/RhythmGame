using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks.CompilerServices;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;//singleton pattern instance

   
    public float musicWaitTime;
    public float speed;
    public int combo { get; set; }
    public float dist { get; private set; }
    public float BPM { get; private set; }
    [SerializeField] GameObject nodePrefab;
    [SerializeField] GameObject longNodePrefab;
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
        SetDist();
        BPM = 120;
        LongNodeTest().Forget();
        //LoadNodeData("MilkyWayGalaxyTest");
        //PrepareAllNodes();
        combo = 0;
        judgeUI.text = "";
        detailJudgeUI.text = "";
    }

    async UniTaskVoid LongNodeTest()
    {
        while(true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            GameObject longNodeobj = Instantiate(longNodePrefab);
            Node headNode = longNodeobj.GetComponent<Node>();
            headNode.SetNodeLine(1);
            SetNodePos(headNode);
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
            GameObject node = Instantiate(nodePrefab);
            var nodeScript = node.GetComponent<Node>();
            nodeScript.SetNodeLine(nodeData.lineNum);
            SetNodePos(nodeScript);
            ActivateNode(nodeData.bit, node).Forget();
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
                node.transform.position = spawnLine.position + new Vector3(-6, 0, 0);//lane1
                break;
            case 2:
                node.transform.position = spawnLine.position + new Vector3(-2, 0, 0);//lane2
                break;
            case 3:
                node.transform.position = spawnLine.position + new Vector3(2, 0, 0);//lane3
                break;
            case 4:
                node.transform.position = spawnLine.position + new Vector3(6, 0, 0);//lane4
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
}
