using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Pool;

public class Node : MonoBehaviour
{
    float _speed;
    int _startLine;
    float _occurTime;
    float _timer;
    float _expectedArriveTime;
    IObjectPool<Node> _nodePool;

    public void setPool(IObjectPool<Node> pool)
    {
        _nodePool = pool;
    }
    void OnEnable()
    {
        _timer = 0f;
        _speed = 20f;
        _expectedArriveTime = 10f / _speed;
        _startLine = Random.Range(1, 6);
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space) && _expectedArriveTime - _timer < 0.08f)
        {
            nodeJudgement(_timer);
            destroyNode();
        }

        if(_timer - _expectedArriveTime > 0.08f)
        {
            Debug.Log("Miss");
            destroyNode();
        }
    }

    void nodeJudgement(float inputTime)//판정시간을 speed에 상대적이게 바꿔야할듯
    {
        float diff = Mathf.Abs(inputTime - _expectedArriveTime);

        if (0 <= diff && diff < 0.02f) Debug.Log("perfect");
        else if (0.02f <= diff && diff < 0.04f) Debug.Log("Great");
        else if (0.04f <= diff && diff < 0.06f) Debug.Log("Good");
        else if (0.06f <= diff && diff < 0.08f) Debug.Log("Bad");
    }

    void destroyNode()
    {
        _nodePool.Release(this);
    }
}
