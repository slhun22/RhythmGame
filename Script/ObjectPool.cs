using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] Node _nodePrefab;
    IObjectPool<Node> _nodePool;
    float _time;
    private void Awake()
    {
        _nodePool = new ObjectPool<Node>(
            createNode,
            onGetNode,
            onReleaseNode,
            onDestroyNode,
            maxSize: 40
            );

        _time = 0;
    }

    Node createNode()
    {
        Node node = Instantiate(_nodePrefab);
        node.setPool(_nodePool);
        return node;
    }

    void onGetNode(Node node)
    {
        node.gameObject.SetActive(true);
    }
    void onReleaseNode(Node node)
    {
        node.gameObject.SetActive(false);
    }
    
    void onDestroyNode(Node node)
    {
        Destroy(node.gameObject);
    }

    private void Update()
    {
        if (_time > 2)
        {
            var node = _nodePool.Get();
            setNodePos(node);
            _time = 0;
        }

        else _time += Time.deltaTime;
    }

    void setNodePos(Node node)//이 역할을 수행하는 getinfo함수가 필요
    {
        int line = Random.Range(1, 5);
        switch (line)
        {
            case 1:
                node.transform.position = transform.position + new Vector3(-6, 0, 0);//라인1
                break;
            case 2:
                node.transform.position = transform.position + new Vector3(-2, 0, 0);//라인2
                break;
            case 3:
                node.transform.position = transform.position + new Vector3(2, 0, 0);//라인3
                break;
            case 4:
                node.transform.position = transform.position + new Vector3(6, 0, 0);//라인4
                break;
        }
    }

    void GetNodeInfo(NodeInfo nodeData)//pool을 쓰면 안될 것 같다. 그냥 처음에 한꺼번에 로드하는 방식으로 해야할듯
    {
       
    }
}
