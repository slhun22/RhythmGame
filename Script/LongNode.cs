using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LongNode : MonoBehaviour //HeadNode의 자식으로 롱노트 부품을 넣기, 판정의 크기가 자식이면 다르게 적용된다. 그러면 bpm으로 length를 측정하는게 맞다
{
    [SerializeField] Node headNode;
    [SerializeField] float length;
    float lastTime;
    bool timeOver;
    float speed;
    void Start()
    {       
        speed = GameManager.instance.speed;
        lastTime = length / speed;
        timeOver = false;
        LongJudgement().Forget();
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

    async UniTaskVoid LongJudgement()
    {
        await UniTask.WaitUntil(() => headNode.isEnd);
        Timer().Forget();

        while(!timeOver)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
            if(Input.GetKey(GetNodeLaneInput()))
            {
                Debug.Log("perfect");
                GameManager.instance.SetJudegeUI(0).Forget();
                GameManager.instance.ClearDetailJudge();
                GameManager.instance.combo++;
            }
            else
            {
                Debug.Log("MissLong");
                GameManager.instance.SetJudegeUI(4).Forget();
                GameManager.instance.ClearDetailJudge();
                GameManager.instance.combo = 0;
            }          
        }

        headNode.gameObject.SetActive(false);
    }
}
