using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorNode : MonoBehaviour
{
    [SerializeField] float searchRadius;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    Vector2 SearchClosestPos()
    {
        var possibleLines = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        int lineN = possibleLines.Length;
        var shortest = possibleLines[0];
        for (int i = 1; i < lineN; i++)
        {
            float dist = CalculateManhattanDist(transform.position, possibleLines[i].gameObject.transform.position);
            float distShort = CalculateManhattanDist(transform.position, shortest.gameObject.transform.position);
            if (dist < distShort) shortest = possibleLines[i];
        }

        return (Vector2)shortest.gameObject.transform.position;
    }

    float CalculateManhattanDist(Vector2 a, Vector2 b)
    {
        float distX = Mathf.Abs(a.x - b.x);
        float distY = Mathf.Abs(a.y - b.y);

        return distX + distY;
    }

    void PutNode()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 targetPos = SearchClosestPos();
            switch((int)targetPos.x)
            {
                
            }
        }
    }
}
