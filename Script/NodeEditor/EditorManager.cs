using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    [SerializeField] CameraMove cameraMoveScript;
    [SerializeField] GameObject lineNodeObj;
    [SerializeField] GameObject musicCheckPointObj;
    [SerializeField] GameObject progressBarObj;
    [SerializeField] List<Transform> lineParents = new List<Transform>(4);
    [SerializeField] int toplineNum;
    [SerializeField] int musicCheckNum = -1;
    [SerializeField] TMP_InputField bpmInput;
    [SerializeField] AudioSource audiosrc;
    [SerializeField] AudioClip music;
    float BPM;
    bool isPlaying;
    // Start is called before the first frame update
    void Start()
    {
        toplineNum = 10;
        audiosrc.clip = music;
        isPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        GenerateNewLine();
    }
    
    void GenerateNewLine()//Create new Lines
    {
        int expectedTopLineNum = (int)cameraMoveScript.maxCenterY + 10;
        while (expectedTopLineNum > toplineNum)
        {
            Instantiate(lineNodeObj, new Vector3(-7, toplineNum - 4, 0), Quaternion.identity, lineParents[0]);
            Instantiate(lineNodeObj, new Vector3(-3, toplineNum - 4, 0), Quaternion.identity, lineParents[1]);
            Instantiate(lineNodeObj, new Vector3(1, toplineNum - 4, 0), Quaternion.identity, lineParents[2]);
            Instantiate(lineNodeObj, new Vector3(5, toplineNum - 4, 0), Quaternion.identity, lineParents[3]);
            Instantiate(musicCheckPointObj, new Vector3(-9.5f, toplineNum - 4, 0), Quaternion.identity, lineParents[4]);
            toplineNum++;
        }
    }
    public void SetBPM() { BPM = float.Parse(bpmInput.text); }
    public void SetMusicCheckPoint(int y) { musicCheckNum = y; }
    public void MusicPlay()
    {
        if (!isPlaying)
        {
            SetBPM();
            float timePerBit = 60 / BPM;
            float playStartTime = timePerBit * musicCheckNum / 4;
            audiosrc.time = playStartTime;
            audiosrc.Play();
            isPlaying = true;
            MusicProgressBar().Forget();
        }

        else
        {
            audiosrc.Stop();
            isPlaying = false;
        }   
    }

    async UniTaskVoid MusicProgressBar()
    {
        await UniTask.WaitUntil(() => isPlaying);
        float bitsPerSecond = BPM / 60;
        float speed = bitsPerSecond * 4;
        while(isPlaying)
        {
            progressBarObj.transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
    }
}
