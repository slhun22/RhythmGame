using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorManager : MonoBehaviour
{
    [SerializeField] GameObject nodeObj;
    bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateInfiniteGrid()
    {

    }
 
    public void GenerateNode()
    {
        if(!isActive)
        {
            isActive = true;
            
            nodeObj.SetActive(true);
            nodeObj.transform.position = transform.position;
        }
    }

    void NodeControl()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        nodeObj.transform.position = mousePos;
    }
}
