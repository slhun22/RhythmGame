using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] EditorManager manager;
    public float maxCenterY { get; private set; }
    [SerializeField] float speed;
    Vector2 beforePoint = Vector2.zero;
    Vector3 chaseBeforePos;
    bool isCameraChaseActive;


    private void Start()
    {
        maxCenterY = 0;
        chaseBeforePos = new Vector3(-1, 0, -10);
        isCameraChaseActive = false;
        manager = GameObject.Find("EditorManager").GetComponent<EditorManager>();
    }

    private void Update()
    {
        MoveOnMouse();
        CameraChase();
    }

    void MoveOnMouse()
    {
        if (Input.GetMouseButton(2))
        {
            Vector2 currentPoint = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

            if (beforePoint == Vector2.zero)
                beforePoint = currentPoint;

            Vector2 diff = beforePoint - currentPoint;

            if(transform.position.y >= 0 || transform.position.y < 0 && diff.y > 0)
            {
                transform.position += new Vector3(0, diff.y, 0) * speed * Time.deltaTime;

                if (transform.position.y > maxCenterY)
                    maxCenterY = transform.position.y;
            }
        }

        else
            beforePoint = Vector2.zero;
    }

    void CameraChase()
    {
        int cameraStartPosY;
        if(manager.isPlaying)
        {
            if(!isCameraChaseActive)
            {
                chaseBeforePos = transform.position;

                if (manager.GetMusicCheckPosition() < 0) 
                    cameraStartPosY = 0;
                else 
                    cameraStartPosY = manager.GetMusicCheckPosition();

                transform.position = new Vector3(-1, cameraStartPosY, -10);
                isCameraChaseActive = true;
            }

            transform.Translate(manager.GetTranslatePower());
        }

        else if(!manager.isPlaying && isCameraChaseActive)
        {
            transform.position = chaseBeforePos;
            isCameraChaseActive = false;
        }
    }
}
