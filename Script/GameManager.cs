using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject nodePrefab;
    [SerializeField] Transform laneCenter;
    string songname;
    float BPM;
    List<NodeInfo> currentSongDatas = new List<NodeInfo>(20);//contains current songs all nodedatas by using NodeInfo class.
    public float speed;


    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
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
    public void LoadNodeData(string songName)
    {
        string path = string.Format("{0}/{1}.txt", Application.persistentDataPath, songName);
        
        if(File.Exists(path))
        {
            string textData = File.ReadAllText(path);
            string[] nodeDatas = textData.Split('\n');
            string[] basicData = nodeDatas[0].Split(' ');
            songname = basicData[0];
            BPM = float.Parse(basicData[1]);
            int length = nodeDatas.Length;
            for (int i = 1; i < length; i++)
            {
                var s = nodeDatas[i];
                string[] nodeData = s.Split(' ');
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
            nodeScript.SetNodeLine(nodeData.lineNum);
            SetNodePos(nodeScript);
            //ActivateNode(nodeData.time, node).Forget();
        }
    }

    private async UniTaskVoid ActivateNode(float time, GameObject nodeObj)//need to be fixed
    {
        nodeObj.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        nodeObj.SetActive(true);
    }
    void SetNodePos(Node node)
    {
        switch (node.line)
        {
            case 1:
                node.transform.position = laneCenter.position + new Vector3(-6, 0, 0);//lane1
                break;
            case 2:
                node.transform.position = laneCenter.position + new Vector3(-2, 0, 0);//lane2
                break;
            case 3:
                node.transform.position = laneCenter.position + new Vector3(2, 0, 0);//lane3
                break;
            case 4:
                node.transform.position = laneCenter.position + new Vector3(6, 0, 0);//lane4
                break;
        }
    }
}
