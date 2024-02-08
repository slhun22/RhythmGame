using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float musicWaitTime;
    [SerializeField] float speed;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] Transform spawnLine;
    [SerializeField] Transform judgeLine;
    List<NodeInfo> currentSongDatas = new List<NodeInfo>(20);//contains current songs all nodedatas by using NodeInfo class.
    string songname;
    float BPM;
    float dist;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetDist();
        LoadNodeData("MilkyWayGalaxyTest");
        PrepareAllNodes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region TestFunc
    void LoadDataPassive()//only for test
    {
        NodeInfo nodeInfo1 = new NodeInfo(1, 1);
        currentSongDatas.Add(nodeInfo1);
        NodeInfo nodeInfo2 = new NodeInfo(2, 1.2f);
        currentSongDatas.Add(nodeInfo2);
        NodeInfo nodeInfo3 = new NodeInfo(3, 1.4f);
        currentSongDatas.Add(nodeInfo3);
        NodeInfo nodeInfo4 = new NodeInfo(4, 1.6f);
        currentSongDatas.Add(nodeInfo4);
        NodeInfo nodeInfo5 = new NodeInfo(2, 4f);
        currentSongDatas.Add(nodeInfo5);
        NodeInfo nodeInfo6 = new NodeInfo(3, 7f);
        currentSongDatas.Add(nodeInfo6);
        NodeInfo nodeInfo7 = new NodeInfo(1, 8f);
        currentSongDatas.Add(nodeInfo7);
        NodeInfo nodeInfo8 = new NodeInfo(2, 8.1f);
        currentSongDatas.Add(nodeInfo8);
    }
    #endregion
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
            string[] basicData = nodeDatas[0].Split(' ');
            songname = basicData[0];
            BPM = float.Parse(basicData[1]);
            int length = nodeDatas.Length;
            for (int i = 1; i < length; i++)
            {
                var s = nodeDatas[i];
                string[] nodeData = s.Split(' ');
                Debug.Log(nodeData[0] + " " + nodeData[1]);
                NodeInfo nodeInfo = new NodeInfo(int.Parse(nodeData[0]), float.Parse(nodeData[1]));
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
            nodeScript.speed = speed;
            nodeScript.dist = dist;
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
}
