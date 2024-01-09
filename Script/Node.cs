using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Pool;

public class Node : MonoBehaviour
{
    float speed;
    public int line { get; private set;}
    float timer;
    float expectedArriveTime;
    void OnEnable()
    {
        timer = 0f;
        speed = 20f;
        expectedArriveTime = 10f / speed;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (Input.GetKeyDown(GetNodeLaneInput()) && expectedArriveTime - timer < 0.08f)
        {
            nodeJudgement(timer);
            gameObject.SetActive(false);
        }

        if(timer - expectedArriveTime > 0.08f)
        {
            Debug.Log("Miss");
            gameObject.SetActive(false);
        }
    }
    KeyCode GetNodeLaneInput()
    {
        KeyCode laneInput = KeyCode.Space;
        switch(line)
        {
            case 1 : 
                laneInput = KeyCode.D;
                break;
            case 2 : 
                laneInput = KeyCode.F;
                break;
            case 3:
                laneInput = KeyCode.J;
                break;
            case 4:
                laneInput = KeyCode.K;
                break;
        }
        return laneInput;
    }

    void nodeJudgement(float inputTime)//판정시간을 speed에 상대적이게 바꿔야할듯
    {
        float diff = Mathf.Abs(inputTime - expectedArriveTime);

        if (0 <= diff && diff < 0.02f) Debug.Log("perfect");
        else if (0.02f <= diff && diff < 0.04f) Debug.Log("Great");
        else if (0.04f <= diff && diff < 0.06f) Debug.Log("Good");
        else if (0.06f <= diff && diff < 0.08f) Debug.Log("Bad");
    }
    
    public void SetNodeLine(int line)
    {
        this.line = line;
    }
    
}
