using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorManager : MonoBehaviour
{
    [SerializeField] CameraMove cameraMoveScript;
    [SerializeField] GameObject lineNodeObj;
    [SerializeField] List<Transform> lineParents = new List<Transform>(4);
    [SerializeField] int toplineNum;
    // Start is called before the first frame update
    void Start()
    {
        toplineNum = 10;
    }

    // Update is called once per frame
    void Update()
    {
        GenerateNewLine();
    }
    
    void GenerateNewLine()
    {
        int expectedTopLineNum = (int)cameraMoveScript.maxCenterY + 10;
        while (expectedTopLineNum > toplineNum)
        {
            Instantiate(lineNodeObj, new Vector3(-7, toplineNum - 4, 0), Quaternion.identity, lineParents[0]);
            Instantiate(lineNodeObj, new Vector3(-3, toplineNum - 4, 0), Quaternion.identity, lineParents[1]);
            Instantiate(lineNodeObj, new Vector3(1, toplineNum - 4, 0), Quaternion.identity, lineParents[2]);
            Instantiate(lineNodeObj, new Vector3(5, toplineNum - 4, 0), Quaternion.identity, lineParents[3]);
            toplineNum++;
        }
    }
}
