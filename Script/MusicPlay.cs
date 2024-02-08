using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlay : MonoBehaviour
{
    [SerializeField] GameManager gm;
    [SerializeField] AudioClip clip;
    AudioSource audiosrc;
    bool isPlaying = true;
    void Start()
    {
        audiosrc = GetComponent<AudioSource>();
        audiosrc.clip = clip;
        PlayMusic().Forget();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (isPlaying)
            {
                audiosrc.Pause();
                isPlaying = false;
            }

            else
            {
                audiosrc.UnPause();
                isPlaying = true;
            }
        }
    }

    async UniTaskVoid PlayMusic()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(gm.musicWaitTime));
        audiosrc.Play();
    }
}
