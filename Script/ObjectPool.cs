using UnityEngine;
using UnityEngine.Pool;
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance = null;//singleton pattern instance
    public IObjectPool<GameObject> pool;
    [SerializeField] GameObject hitVFXPrefab;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }

        pool = new ObjectPool<GameObject>(
            CreateVFX,
            OnGetVFX,
            OnReleaseVFX,
            OnDestroyVFX,
            maxSize: 30
            );
    }

    GameObject CreateVFX()
    {
        GameObject vfxObj = Instantiate(hitVFXPrefab);
        return vfxObj;
    }

    void OnGetVFX(GameObject vfxObj)
    {
        vfxObj.SetActive(true);
    }
    void OnReleaseVFX(GameObject vfxObj)
    {
        vfxObj.SetActive(false);
    }
    
    void OnDestroyVFX(GameObject vfxObj)
    {
        Destroy(vfxObj);
    }
}