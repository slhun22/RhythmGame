using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class Node : MonoBehaviour
{
    public int Line { get; private set; }  
    public bool IsEnd { get; private set; } // only used in LongNode and ArkNode
    public bool arkMode; // only used in ArkNode
    public bool headMode; // only used in LongNode
    SpriteRenderer spriteRenderer; // only used in LongNode

    public bool isSkyNode; // only used in SkyNode

    float speed;
    float dist;
    float timer;
    float expectedArriveTime;
    const float PERFECT_TIME = 41.7f * 0.001f;
    const float GREAT_TIME = 83.3f * 0.001f;
    const float GOOD_TIME = 108.3f * 0.001f;
    const float BAD_TIME = 125.0f * 0.001f;

    void Start()
    {
        //line = 1;
        timer = 0f;
        speed = GameManager.instance.speed;
        dist = GameManager.instance.Dist;
        expectedArriveTime = dist / speed;
        spriteRenderer = GetComponent<SpriteRenderer>();
        IsEnd = false;
    }

    // Update is called once per frame
    void Update()//세부적인 타임체킹이 필요하므로 update에서 시간 돌리기
    {
        timer += Time.deltaTime;
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (IsEnd) return;

        if (arkMode)
        {
            if (expectedArriveTime - timer <= 0)
            {
                GameManager.instance.VFXOn(Line, isSkyNode);
                IsEnd = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(GetNodeLaneInput()) && expectedArriveTime - timer < BAD_TIME)
            {
                NodeJudgement(timer);

                if (!headMode)
                {
                    ActivateVFX().Forget();
                    gameObject.SetActive(false);
                }
                  
                else
                {
                    GameManager.instance.VFXOn(Line, isSkyNode);
                    IsEnd = true;
                    spriteRenderer.color = new Color32(0, 0, 0, 0);
                }
            }

            else if (timer - expectedArriveTime > BAD_TIME)
            {
                Debug.Log("Miss");
                GameManager.instance.SetJudegeUI(4).Forget();
                GameManager.instance.ClearDetailJudge();
                GameManager.instance.Combo = 0;
                if (!headMode)
                    gameObject.SetActive(false);
                else
                {
                    IsEnd = true;
                    spriteRenderer.color = new Color32(0, 0, 0, 0);
                }
            }
        }      
    }
    public KeyCode GetNodeLaneInput()
    {
        KeyCode laneInput = KeyCode.Space;
        switch (Line)
        {
            case 1:
                if (isSkyNode) laneInput = KeyCode.E;
                else laneInput = KeyCode.D;
                break;
            case 2:
                if (isSkyNode) laneInput = KeyCode.R;
                else laneInput = KeyCode.F;
                break;
            case 3:
                if (isSkyNode) laneInput = KeyCode.U;
                else laneInput = KeyCode.J;
                break;
            case 4:
                if (isSkyNode) laneInput = KeyCode.I;
                else laneInput = KeyCode.K;
                break;
        }
        return laneInput;
    }
    void NodeJudgement(float inputTime)
    {
        float actualDiff = inputTime - expectedArriveTime;
        float diff = Mathf.Abs(actualDiff);

        if (0 <= diff && diff < PERFECT_TIME)
        {
            Debug.Log("perfect");
            GameManager.instance.SetJudegeUI(0).Forget();
            GameManager.instance.ClearDetailJudge();
            GameManager.instance.Combo++;
        }
        else if (PERFECT_TIME <= diff && diff < GREAT_TIME)
        {
            Debug.Log("Great");
            GameManager.instance.SetJudegeUI(1).Forget();
            GameManager.instance.SetDetailJudgeUI(actualDiff).Forget();
            GameManager.instance.Combo++;
        }
        else if (GREAT_TIME <= diff && diff < GOOD_TIME)
        {
            Debug.Log("Good");
            GameManager.instance.SetJudegeUI(2).Forget();
            GameManager.instance.SetDetailJudgeUI(actualDiff).Forget();
            GameManager.instance.Combo = 0;
        }  
        else if (GOOD_TIME <= diff && diff < BAD_TIME) 
        {
            Debug.Log("Bad");
            GameManager.instance.SetJudegeUI(3).Forget();
            GameManager.instance.SetDetailJudgeUI(actualDiff).Forget();
            GameManager.instance.Combo = 0;
        }
    }    
    public void SetNodeLine(int line)
    {
        this.Line = line;
    }
    async UniTaskVoid ActivateVFX()
    {
        Vector3 judgeLinePos = new Vector3(0, -4, 0);
        GameObject hitObj = ObjectPool.instance.pool.Get();

        if (isSkyNode)
            hitObj.transform.position = judgeLinePos + GameManager.instance.skyLineVecs[Line - 1];
        else
            hitObj.transform.position = judgeLinePos + GameManager.instance.groundLineVecs[Line - 1];

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        ObjectPool.instance.pool.Release(hitObj);
    }
}
