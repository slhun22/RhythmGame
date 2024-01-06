using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<NodeInfo> currentSongDatas;//contains current songs all nodedatas.

    // Start is called before the first frame update
    void Start()
    {
        LoadNodeData("asdf");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNodeData(string textData)//textfile form : linenum occurtime \n linenum occurtime ...  
    {
        //text>>nodeInfo parsing
        string[] nodeDatas = textData.Split('\n');
        foreach(string s in nodeDatas)
        {
            string[] nodeData = s.Split(' ');
            NodeInfo nodeInfo = new NodeInfo(int.Parse(nodeData[0]), float.Parse(nodeData[1]));
            currentSongDatas.Add(nodeInfo);
        }
    }
}
