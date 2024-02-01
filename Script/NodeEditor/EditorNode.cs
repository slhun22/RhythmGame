using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorNode : MonoBehaviour
{
    public bool isSelected;
    float checkRange;
    bool isOntheMouse;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        isOntheMouse = false;
        isSelected = false;
        checkRange = 0.7f;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MouseOntheNode();
        SelectNode();
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
            if(!isSelected)
                spriteRenderer.color = Color.red;
            isOntheMouse = true;
        }
        else
        {
            if(!isSelected)
                spriteRenderer.color = Color.white;
            isOntheMouse = false;
        }     
    }

    void SelectNode()
    {
        if(isOntheMouse && Input.GetMouseButtonDown(0))
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
}
