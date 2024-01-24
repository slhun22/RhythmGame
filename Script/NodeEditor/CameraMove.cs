using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float maxCenterY { get; private set; }
    [SerializeField] float speed;
    Vector2 beforePoint = Vector2.zero;

    private void Start()
    {
        maxCenterY = 0;
    }

    private void Update()
    {
        MoveOnMouse();
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
}
