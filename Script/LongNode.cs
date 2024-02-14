using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LongNode : MonoBehaviour
{
    [SerializeField] Node headNode;
    [SerializeField] float bitNum;
    [SerializeField] Color32 missColor;
    Color32 originalColor;
    SpriteRenderer spriteRenderer;
    float lastTime;
    float safeTime;
    bool timeOver;
    bool missWarning;
    bool safeTimeOver;
    const float PARENT_SIZE = 0.4637f;//virtualLength = actualLength / PARENT_SIZE
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        safeTime = 30 / GameManager.instance.BPM;
        safeTimeOver = false;
        missWarning = false;
        SetLongNode();
        LongJudgement().Forget();
    }
    private void Update()
    {
        SafeTimer();
        LongNodeResizeByHit();
    }

    void SetLongNode()
    {
        float speed = GameManager.instance.speed;
        timeOver = false;
        float timePerBit = 60 / GameManager.instance.BPM;
        lastTime = timePerBit * bitNum;
        float actualLength = speed * lastTime;
        float virtualLength = actualLength / PARENT_SIZE;
        transform.localScale = new Vector3(1, virtualLength ,1);
        transform.localPosition = new Vector3(0, virtualLength / 2, 0);
    }

    KeyCode GetNodeLaneInput()
    {
        KeyCode laneInput = KeyCode.Space;
        switch (headNode.line)
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

    async UniTaskVoid Timer() //세부적인 타임체킹이 필요없으므로 비동기 task로 맡겨놓기
    {
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        timeOver = true;
    }
    void SafeTimer() //used in update func
    {
        if(missWarning)
        {
            safeTime -= Time.deltaTime;

            if (safeTime < 0)
                safeTimeOver = true;
        }    
    }
    async UniTaskVoid LongJudgement()
    {
        float timePerBit = 60 / GameManager.instance.BPM;
        await UniTask.WaitUntil(() => headNode.isEnd);//head hit 전까지 대기
        Timer().Forget();
        while(!timeOver)
        {        
            await UniTask.Delay(TimeSpan.FromSeconds(timePerBit / 4));//determined by song's BPM
            if (Input.GetKey(GetNodeLaneInput()))
            {
                missWarning = false;
                safeTimeOver = false;
                safeTime = timePerBit / 2;
                Debug.Log("perfect");
                spriteRenderer.color = originalColor;
                GameManager.instance.SetJudegeUI(0).Forget();
                GameManager.instance.combo++;
            }

            else if (!Input.GetKey(GetNodeLaneInput()))
            {
                missWarning = true;
                spriteRenderer.color = missColor;
                if (safeTimeOver) //miss
                {
                    Debug.Log("MissLong");
                    GameManager.instance.SetJudegeUI(4).Forget();
                    GameManager.instance.combo = 0;
                }

                else // miss safe time
                {
                    Debug.Log("perfect");
                    GameManager.instance.SetJudegeUI(0).Forget();
                    GameManager.instance.combo++;
                }
            }
            GameManager.instance.ClearDetailJudge();
        }
        headNode.gameObject.SetActive(false);
    }

    void LongNodeResizeByHit()//직접 보이기 때문에 자연스러움을 위해 판정과 별개로 update문에 넣어야 할듯
    {
        if(Input.GetKey(GetNodeLaneInput()) && headNode.isEnd)
        {
            const float judgeLineY = -4.0f;
            float halfLength = transform.lossyScale.y / 2;
            float longNodeTopY = transform.position.y + halfLength;
            float resizeHalfLength = (longNodeTopY - judgeLineY) / 2;
            if (resizeHalfLength < 0)
                resizeHalfLength = 0;
            transform.localScale = new Vector3(1, resizeHalfLength * 2 / PARENT_SIZE, 1);
            transform.position = new Vector3(transform.position.x, judgeLineY + resizeHalfLength, 0);
        }       
    }
}
