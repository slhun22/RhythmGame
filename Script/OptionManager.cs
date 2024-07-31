using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour {
    public static OptionManager instance = null;
    [SerializeField] int _musicOffset;
    [SerializeField] float _noteSpeed;
    [SerializeField] AudioClip _tickSound;
    [SerializeField] Button _offsetStartButton;
    [SerializeField] Button _quitButton;
    [SerializeField] TextMeshProUGUI _currentOffsetTM;
    [SerializeField] TextMeshProUGUI _currentNoteSpeedTM;
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
        DontDestroyOnLoad(gameObject);
    }
    private void Start() {
        _audioSource = GetComponent<AudioSource>();
        UpdateNoteSpeedUI();
    }

    public void NoteSpeedUp() {
        if (_noteSpeed <= 10.9f)
            _noteSpeed += 0.1f;
        UpdateNoteSpeedUI();
    }
    public void NoteSpeedUpDouble() {
        if (_noteSpeed <= 10)
            _noteSpeed += 1;
        UpdateNoteSpeedUI();
    }


    public void NoteSpeedDown() {
        if (_noteSpeed >= 0.1f)
            _noteSpeed -= 0.1f;
        UpdateNoteSpeedUI();
    }
    public void NoteSpeedDownDouble() {
        if (_noteSpeed >= 1)
            _noteSpeed -= 1;
        UpdateNoteSpeedUI();
    }

    public void MusicOffsetUp() {
        _musicOffset++;
        UpdateCurrentOffsetUI();
    }

    public void MusicOffsetDown() {
        _musicOffset--;
        UpdateCurrentOffsetUI();
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
        UpdateCurrentOffsetUI();
    }

    private async UniTaskVoid OffsetUniTask() {
        InitializeOffsetList();
        _offsetStartButton.enabled = false;
        _quitButton.enabled = false;
        int i, j, result, sum = 0;
        for (i = 0; i < 4; i++) {
            var timerStopTime = OffsetTimer();
            _audioSource.pitch = 1;
            for (j = 0; j < 3; j++) {
                _audioSource.PlayOneShot(_tickSound);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            }
            _audioSource.pitch = 1.4f;
            _audioSource.PlayOneShot(_tickSound);
            result = (int)MathF.Floor((1.5f - await timerStopTime) * 1000);

            _offsetNumberTMList[i].text = $"{result}";
            sum += result;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
        _musicOffset = sum / 4;
        UpdateCurrentOffsetUI();
        _offsetStartButton.enabled = true;
        _quitButton.enabled = true;
    }

    private async UniTask<float> OffsetTimer() {
        float timer = 0;
        while (!Input.GetKeyDown(KeyCode.Space)) {
            timer += Time.deltaTime;
            await UniTask.NextFrame();
        }
        return timer;
    }

    private void UpdateCurrentOffsetUI() {
        _currentOffsetTM.text = $"current offset : {_musicOffset}";
    }

    private void UpdateNoteSpeedUI() {
        _currentNoteSpeedTM.text = $"current notespeed : {_noteSpeed}";
    }

}
