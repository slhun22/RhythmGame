using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class Metronome : MonoBehaviour
{
    [SerializeField] AudioSource audiosrc;
    [SerializeField] AudioSource musicAudioSrc;
    [SerializeField] AudioClip tickSound;
    [SerializeField] AudioClip musicClip;
    public float BPM;
    bool isFirst = true;
    // Start is called before the first frame update
    void Start()
    {
        MetronomeActivate().Forget();
    }
    public void SetBPM(float BPM) { this.BPM = BPM; }

    async UniTaskVoid MetronomeActivate()
    {
        while(true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(60 / BPM));
            audiosrc.PlayOneShot(tickSound);
            if (isFirst)
            {
                musicAudioSrc.PlayOneShot(musicClip);
                isFirst = false;
            }
        }
    }  
}
