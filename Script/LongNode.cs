using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class LongNode : MonoBehaviour
{
    [SerializeField] Node headNode;
    [SerializeField] float bitNum;
    [SerializeField] Color32 missColor;
    [SerializeField] Material missMat;
    Color32 originalColor;
    Material originalMat;
    SpriteRenderer spriteRenderer;
    MeshRenderer meshRenderer;
    float lastTime;
    float safeTime;
    bool timeOver;
    bool missWarning;
    bool safeTimeOver;
    const float PARENT_SIZE = 0.4637f;//virtualLength = actualLength / PARENT_SIZE
    void Start()
    {
        if (headNode.isSkyNode)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            originalMat = meshRenderer.material;
        }          
        else
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }      
        safeTime = 30 / GameManager.instance.BPM;
        safeTimeOver = false;
        missWarning = false;
        SetLongNode();
        LongJudgement().Forget();
    }
    private void Update()
    {
        SafeTimer();
        if (headNode.isSkyNode)
            ArkNodeResizeByHit();
        else
            LongNodeResizeByHit();
    }

    void SetLongNode()
    {
        float speed = GameManager.instance.speed;
        timeOver = false;
        float timePerBit = 60 / GameManager.instance.BPM;
        lastTime = timePerBit * bitNum;
        float actualLength = speed * lastTime;
        float virtualLength;
        if(headNode.isSkyNode) 
            virtualLength = actualLength;
        else
            virtualLength = actualLength / PARENT_SIZE;

        transform.localScale = new Vector3(1, virtualLength ,1);
        transform.localPosition = new Vector3(0, virtualLength / 2, 0);
    }
    async UniTaskVoid Timer() //세부적인 타임체킹이 필요없으므로 비동기 task로 맡겨놓기
    {
        await UniTask.Delay(TimeSpan.FromSeconds(lastTime));
        timeOver = true;
    }
    void SafeTimer() //used in update func
    {
        if(missWarning)
        {
            safeTime -= Time.deltaTime;

            if (safeTime < 0)
                safeTimeOver = true;
        }    
    }
    async UniTaskVoid LongJudgement()
    {
        float timePerBit = 60 / GameManager.instance.BPM;
        await UniTask.WaitUntil(() => headNode.IsEnd);//head hit 전까지 대기
        Timer().Forget();
        while(!timeOver)
        {        
            await UniTask.Delay(TimeSpan.FromSeconds(timePerBit / 4));//determined by song's BPM
            if (Input.GetKey(headNode.GetNodeLaneInput()))
            {
                missWarning = false;
                safeTimeOver = false;
                safeTime = timePerBit / 3;
                Debug.Log("perfect");

                if (headNode.isSkyNode)
                meshRenderer.material = originalMat;
                else
                    spriteRenderer.color = originalColor;

                GameManager.instance.VFXOn(headNode.Line, headNode.isSkyNode);
                GameManager.instance.SetJudegeUI(0).Forget();
                GameManager.instance.Combo++;
            }

            else if (!Input.GetKey(headNode.GetNodeLaneInput()))
            {
                missWarning = true;

                if (headNode.isSkyNode)
                    meshRenderer.material = missMat;
                else
                    spriteRenderer.color = missColor;
     
                if (safeTimeOver) //miss
                {
                    GameManager.instance.VFXOff(headNode.Line, headNode.isSkyNode);
                    Debug.Log("MissLong");
                    GameManager.instance.SetJudegeUI(4).Forget();
                    GameManager.instance.Combo = 0;
                }

                else // miss safe time
                {
                    Debug.Log("perfect");
                    GameManager.instance.SetJudegeUI(0).Forget();
                    GameManager.instance.Combo++;
                }
            }
            GameManager.instance.ClearDetailJudge();
        }
        GameManager.instance.VFXOff(headNode.Line, headNode.isSkyNode);
        headNode.gameObject.SetActive(false);
    }

    void LongNodeResizeByHit()//직접 보이기 때문에 자연스러움을 위해 판정과 별개로 update문에 넣어야 할듯
    {
        if(Input.GetKey(headNode.GetNodeLaneInput()) && headNode.IsEnd)
        {
            const float judgeLineY = -4.0f;
            float halfLength = transform.lossyScale.y / 2;
            float longNodeTopY = transform.position.y + halfLength;
            float resizeHalfLength = (longNodeTopY - judgeLineY) / 2;
            if (resizeHalfLength < 0)
                resizeHalfLength = 0;
            transform.localScale = new Vector3(1, resizeHalfLength * 2 / PARENT_SIZE, 1);
            transform.position = new Vector3(transform.position.x, judgeLineY + resizeHalfLength, 0);
        }       
    }
    void ArkNodeResizeByHit()
    {
        if(Input.GetKey(headNode.GetNodeLaneInput()) && headNode.IsEnd)
        {
            const float judgeLineY = -4.0f;
            float halfLength = transform.lossyScale.y / 2;
            float longNodeTopY = transform.position.y + halfLength;
            float resizeHalfLength = (longNodeTopY - judgeLineY) / 2;
            if (resizeHalfLength < 0)
                resizeHalfLength = 0;
            transform.localScale = new Vector3(1, resizeHalfLength * 2, 1);
            transform.position = new Vector3(transform.position.x, judgeLineY + resizeHalfLength, -3);
        }
    }
    public void SetBitNum(float bitNum) { this.bitNum = bitNum; }
}
