using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EditorNode : MonoBehaviour
{
    public bool isSelected;
    public bool isLongNode;//longnode start node => true
    public float longBitNum;
    private static bool isLongNodeSelectedGlobal = false;//�ճ�Ʈ ǥ�⸦ ���� ���������� ����ϴ� static bool��. Node �������� isLongNode�� ȥ�� ����
    private static EditorNode longNodeStartPoint = null;//�ճ�Ʈ ǥ�⸦ ���� ���������� startNode�� ĳ���ϴ� ����
    float checkRange;
    bool isOntheMouse;
    bool isLocked;//for component(Middle part) of the longNode. Node�� lock�ϸ� user�� click�� response���� �ʰ�x �ȴ�.
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        isOntheMouse = false;
        isSelected = false;
        isLongNode = false;
        isLocked = false;
        longBitNum = -1;
        checkRange = 0.7f;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MouseOntheNode();
        SelectNode();
        SelectLongNode();
    }

    float CalculateManhattanDist(Vector2 a, Vector2 b)
    {
        float distX = Mathf.Abs(a.x - b.x);
        float distY = Mathf.Abs(a.y - b.y);

        return distX + distY;
    }

    void MouseOntheNode()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        float dist = CalculateManhattanDist((Vector2)transform.position, (Vector2)mousePos);

        if (dist < checkRange)
        {
            if((!isSelected) && (!isLongNode) && (!isLocked))
                spriteRenderer.color = Color.red;
            isOntheMouse = true;
        }
        else
        {
            if((!isSelected) && (!isLongNode) && (!isLocked))
                spriteRenderer.color = Color.white;
            isOntheMouse = false;
        }     
    }

    void SelectNode()
    {
        if(isOntheMouse && Input.GetMouseButtonDown(0) && !isLongNode && !isLocked)
        {
            switch(isSelected)
            {
                case true ://unselect case
                    spriteRenderer.color = Color.white;
                    isSelected = false;
                    break;
                case false://select case
                    spriteRenderer.color = Color.blue;
                    isSelected = true;
                    break;
            }
        }
    }

    public void SetNodeSelectMode() //for Load function in EditorManager
    {
        spriteRenderer.color = Color.blue;
        isSelected = true;
    }
    public void SelectLongNode()
    {
        if (isOntheMouse && Input.GetMouseButtonDown(1) && !isSelected && !isLocked)
        {
            if(isLongNode)//�̹� �ճ�Ʈ�� ���������� ����� ��� �ش� �ճ�Ʈ ������ �ν�
            {
                spriteRenderer.color = Color.white;
                isLongNode = false;

                if (isLongNodeSelectedGlobal)//�ճ�Ʈ�� ���������� �������� �����ϴ� �������� ��Ȳ
                    isLongNodeSelectedGlobal = false;

                else//�Ϲ����� �ճ�Ʈ ��� ��Ȳ
                {
                    longBitNum = -1;
                    GameObject endNode = EditorManager.instance.GetLongNodeEnd(gameObject);
                    float startPosY = transform.position.y;
                    float endPosY = endNode.transform.position.y;
                    EditorNode[] lineFriends = transform.parent.GetComponentsInChildren<EditorNode>();
                    for (int i = 0; i < lineFriends.Length; i++)
                    {
                        float targetPosY = lineFriends[i].transform.position.y;
                        if (startPosY < targetPosY && targetPosY <= endPosY)
                            lineFriends[i].UnlockNode();
                    }
                    EditorManager.instance.DeleteLongNodeSet(gameObject);
                }           
            }

            else if (!isLongNodeSelectedGlobal)//�ճ�Ʈ�� ���������� ���ؾ��ϴ� ���
            {
                spriteRenderer.color = Color.green;
                longNodeStartPoint = this;
                isLongNode = true;
                isLongNodeSelectedGlobal = true;
            }

            else if (longNodeStartPoint.gameObject.transform.position.x == transform.position.x)//�ճ�Ʈ�� �� ������ ���ؾ��ϴ� ���, ���� ���϶��θ� selectable�ϵ���
            {
                float endPosY = transform.position.y;
                float startPosY = longNodeStartPoint.transform.position.y;
                longNodeStartPoint.longBitNum = (endPosY - startPosY) / 4;
                EditorManager.instance.AddLongNodeSet(longNodeStartPoint.gameObject, gameObject);
                EditorNode[] lineFriends = transform.parent.GetComponentsInChildren<EditorNode>();
                for (int i = 0; i < lineFriends.Length; i++)
                {
                    float targetPosY = lineFriends[i].transform.position.y;
                    if (startPosY < targetPosY && targetPosY <= endPosY)
                        lineFriends[i].LockNode();
                }
                isLongNodeSelectedGlobal = false;
            }
        } 
    }

    void LockNode()//use this function when node is used as middle part of the long node.
    {
        isLocked = true;
        spriteRenderer.color = Color.green;
        
        if(isSelected)
            spriteRenderer.color = Color.yellow;
    }
    void UnlockNode()
    {
        isLocked = false;
        spriteRenderer.color = Color.white;
    }
}
