using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class MusicCheckPoint : MonoBehaviour
{
    static int count = 10;
    EditorManager manager;
    SpriteRenderer spriteRenderer;
    bool isOntheMouse;
    bool isSelected;
    float checkRange;
    [SerializeField] bool isGreen;
    [SerializeField] bool isInitialMember;

    // Start is called before the first frame update
    void Start()
    {
        if(!isInitialMember)
        {
            if (count % 4 == 0)
                isGreen = false;
            else
                isGreen = true;

            count++;
        }
   
        isOntheMouse = false;
        isSelected = false;
        checkRange = 0.5f;
        manager = GameObject.Find("EditorManager").GetComponent<EditorManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        MouseOn();
        SelectNode();
    }

    float CalculateManhattanDist(Vector2 a, Vector2 b)
    {
        float distX = Mathf.Abs(a.x - b.x);
        float distY = Mathf.Abs(a.y - b.y);

        return distX + distY;
    }

    void MouseOn()
    {
        if (isSelected) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        float dist = CalculateManhattanDist((Vector2)transform.position, (Vector2)mousePos);

        if (dist < checkRange)
        {        
            spriteRenderer.color = Color.red;
            
            isOntheMouse = true;
        }
        else
        {  
            if(isGreen) spriteRenderer.color = Color.green;
            else spriteRenderer.color = Color.yellow;
            isOntheMouse = false;
        }
    }
    void SelectNode()
    {
        if (isOntheMouse && Input.GetMouseButtonDown(0))
        {
            switch (isSelected)
            {
                case true://unselect case
                    if (isGreen) spriteRenderer.color = Color.green;
                    else spriteRenderer.color = Color.yellow;
                    isSelected = false;
                    break;
                case false://select case
                    spriteRenderer.color = Color.blue;
                    manager.SetMusicCheckPoint((int)transform.position.y + 4);
                    isSelected = true;
                    break;
            }
        }
    }
}
