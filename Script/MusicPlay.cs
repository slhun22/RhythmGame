using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlay : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    AudioSource audiosrc;
    bool isPlaying = true;
    void Start()
    {
        audiosrc = GetComponent<AudioSource>();
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
}
