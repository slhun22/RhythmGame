using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MusicPlay : MonoBehaviour {

    [SerializeField] AudioClip clip;
    static AudioSource audiosrc;
    [SerializeField] float offset;
    void Start() {
        audiosrc = GetComponent<AudioSource>();
        audiosrc.clip = clip;
        offset = OptionManager.instance.GetMusicOffset();
        PlayMusic().Forget();
    }

    async UniTaskVoid PlayMusic() {
        await UniTask.Delay(TimeSpan.FromSeconds(GameManager.instance.musicWaitTime + offset * 0.001f));
        audiosrc.Play();
    }

    public void SetOffset(float offset) { this.offset = offset * 0.001f; }

    public static void PauseMusic() {
        audiosrc.Pause();
    }

    public static void UnpauseMusic() {
        audiosrc.UnPause();
    }
}
