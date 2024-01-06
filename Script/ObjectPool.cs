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

    void setNodePos(Node node)//�� ������ �����ϴ� getinfo�Լ��� �ʿ�
    {
        int line = Random.Range(1, 5);
        switch (line)
        {
            case 1:
                node.transform.position = transform.position + new Vector3(-6, 0, 0);//����1
                break;
            case 2:
                node.transform.position = transform.position + new Vector3(-2, 0, 0);//����2
                break;
            case 3:
                node.transform.position = transform.position + new Vector3(2, 0, 0);//����3
                break;
            case 4:
                node.transform.position = transform.position + new Vector3(6, 0, 0);//����4
                break;
        }
    }

    void GetNodeInfo(NodeInfo nodeData)//pool�� ���� �ȵ� �� ����. �׳� ó���� �Ѳ����� �ε��ϴ� ������� �ؾ��ҵ�
    {
       
    }
}
