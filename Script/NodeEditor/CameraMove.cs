using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float MaxCenterY { get; set; }
    [SerializeField] float speed;
    Vector2 beforePoint = Vector2.zero;
    Vector3 chaseBeforePos;
    bool isCameraChaseActive;
    Vector3 firstPos;


    private void Start()
    {
        MaxCenterY = 0;
        chaseBeforePos = new Vector3(-1, 0, -10);
        isCameraChaseActive = false;
        firstPos = transform.position;
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

                if (transform.position.y > MaxCenterY)
                    MaxCenterY = transform.position.y;
            }
        }

        else
            beforePoint = Vector2.zero;
    }

    void CameraChase()
    {
        int cameraStartPosY;
        if(EditorManager.instance.IsPlaying)
        {
            if(!isCameraChaseActive)
            {
                chaseBeforePos = transform.position;
                cameraStartPosY = EditorManager.instance.GetMusicCheckPosition();
                transform.position = new Vector3(-1, cameraStartPosY, -10);
                isCameraChaseActive = true;
            }

            if(!EditorManager.instance.IsPause)
                transform.Translate(EditorManager.instance.GetTranslatePower());
        }

        else if(!EditorManager.instance.IsPlaying && isCameraChaseActive)
        {
            transform.position = chaseBeforePos;
            isCameraChaseActive = false;
        }
    }
    public void TeleportStartPoint() { transform.position = firstPos; } //used in "S" Button
    public void TeleportEndPoint() { transform.position = new Vector3(-1, MaxCenterY, -10); } //used in "E" Button
}
