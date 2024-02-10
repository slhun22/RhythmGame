using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Pool;

public class Node : MonoBehaviour
{
    float speed;
    float dist;
    public int line { get; private set; }
    float timer;
    float expectedArriveTime;
    const float PERFECT_TIME = 41.7f * 0.001f;
    const float GREAT_TIME = 83.3f * 0.001f;
    const float GOOD_TIME = 108.3f * 0.001f;
    const float BAD_TIME = 125.0f * 0.001f;
    void Start()
    {
        timer = 0f;
        speed = GameManager.instance.speed;
        dist = GameManager.instance.dist;
        expectedArriveTime = dist / speed;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (Input.GetKeyDown(GetNodeLaneInput()) && expectedArriveTime - timer < BAD_TIME)
        {
            nodeJudgement(timer);
            gameObject.SetActive(false);
        }

        if (timer - expectedArriveTime > BAD_TIME)
        {
            Debug.Log("Miss");
            GameManager.instance.SetJudegeUI(4).Forget();
            GameManager.instance.ClearDetailJudge();
            GameManager.instance.combo = 0;
            gameObject.SetActive(false);
        }
    }
    KeyCode GetNodeLaneInput()
    {
        KeyCode laneInput = KeyCode.Space;
        switch (line)
        {
            case 1:
                laneInput = KeyCode.D;
                break;
            case 2:
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

    void nodeJudgement(float inputTime)
    {
        float actualDiff = inputTime - expectedArriveTime;
        float diff = Mathf.Abs(actualDiff);

        if (0 <= diff && diff < PERFECT_TIME)
        {
            Debug.Log("perfect");
            GameManager.instance.SetJudegeUI(0).Forget();
            GameManager.instance.ClearDetailJudge();
            GameManager.instance.combo++;
        }
        else if (PERFECT_TIME <= diff && diff < GREAT_TIME)
        {
            Debug.Log("Great");
            GameManager.instance.SetJudegeUI(1).Forget();
            GameManager.instance.SetDetailJudgeUI(actualDiff).Forget();
            GameManager.instance.combo++;
        }
        else if (GREAT_TIME <= diff && diff < GOOD_TIME)
        {
            Debug.Log("Good");
            GameManager.instance.SetJudegeUI(2).Forget();
            GameManager.instance.SetDetailJudgeUI(actualDiff).Forget();
            GameManager.instance.combo = 0;
        }  
        else if (GOOD_TIME <= diff && diff<BAD_TIME) 
        {
            Debug.Log("Bad");
            GameManager.instance.SetJudegeUI(3).Forget();
            GameManager.instance.SetDetailJudgeUI(actualDiff).Forget();
            GameManager.instance.combo = 0;
        }
    }
    
    public void SetNodeLine(int line)
    {
        this.line = line;
    }
    
}
