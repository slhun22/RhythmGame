using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MusicPlay : MonoBehaviour {

    [SerializeField] AudioClip clip;
    AudioSource audiosrc;
    [SerializeField] float offset;
    void Start() {
        audiosrc = GetComponent<AudioSource>();
        audiosrc.clip = clip;
        PlayMusic().Forget();
    }

    async UniTaskVoid PlayMusic() {
        await UniTask.Delay(TimeSpan.FromSeconds(GameManager.instance.musicWaitTime - offset));
        audiosrc.Play();
    }

    public void SetOffset(float offset) { this.offset = offset * 0.001f; }
}
