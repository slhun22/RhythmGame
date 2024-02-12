using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LongNode : MonoBehaviour //HeadNode�� �ڽ����� �ճ�Ʈ ��ǰ�� �ֱ�, ������ ũ�Ⱑ �ڽ��̸� �ٸ��� ����ȴ�. �׷��� bpm���� length�� �����ϴ°� �´�
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

    async UniTaskVoid Timer() //�������� Ÿ��üŷ�� �ʿ�����Ƿ� �񵿱� task�� �ðܳ���
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
