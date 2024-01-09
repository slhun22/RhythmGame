using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System;

public class Metronome : MonoBehaviour
{
    [SerializeField] AudioSource audiosrc;
    [SerializeField] AudioClip tickSound;
    public float BPM;
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

        }
    }    
}
