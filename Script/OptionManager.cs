using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour {
    public static OptionManager instance = null;
    [SerializeField] float _musicOffset;
    [SerializeField] float _noteSpeed;
    [SerializeField] AudioClip _tickSound;
    [SerializeField] Button _offsetStartButton;
    [SerializeField] Button _quitButton;
    [SerializeField] TextMeshProUGUI _currentOffsetTM;
    [SerializeField] List<TextMeshProUGUI> _offsetNumberTMList;

    AudioSource _audioSource;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            if (instance != this)
                Destroy(this.gameObject);
        }
    }
    private void Start() {
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetNoteSpeed(float noteSpeed) {
        _noteSpeed = noteSpeed;
    }

    public void SetMusicOffset(float offset) {
        _musicOffset = offset;
    }
    public float GetNoteSpeed() { return _noteSpeed; }

    public float GetMusicOffset() { return _musicOffset; }

    public void ChangeOffset() {
        OffsetUniTask().Forget();
    }

    public void InitializeOffsetList() {
        int i = 0;
        for (i = 0; i < 4; i++) {
            _offsetNumberTMList[i].text = "";
        }
        _currentOffsetTM.text = $"current offset : {_musicOffset}";
    }

    private async UniTaskVoid OffsetUniTask() {
        _offsetStartButton.enabled = false;
        _quitButton.enabled = false;
        int i, j;
        float sum = 0, result = 0;
        for (i = 0; i < 4; i++) {
            _audioSource.pitch = 1;
            for (j = 0; j < 3; j++) {
                _audioSource.PlayOneShot(_tickSound);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            }
            _audioSource.pitch = 1.4f;
            _audioSource.PlayOneShot(_tickSound);
            result += i;
            _offsetNumberTMList[i].text = $"{result}";
            sum += result;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
        _musicOffset = sum / 4;
        _currentOffsetTM.text = $"current offset : {_musicOffset}";
        _offsetStartButton.enabled = true;
        _quitButton.enabled = true;
    }
}
