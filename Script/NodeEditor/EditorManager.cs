using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public bool isPlaying;
    [SerializeField] CameraMove cameraMoveScript;
    [SerializeField] GameObject lineNodeObj;
    [SerializeField] GameObject musicCheckPointObj;
    [SerializeField] GameObject progressBarObj;
    [SerializeField] List<Transform> lineParents = new List<Transform>(4);
    [SerializeField] int toplineNum;
    [SerializeField] int musicCheckNum;
    [SerializeField] TMP_InputField bpmInput;
    [SerializeField] AudioSource audiosrc;
    [SerializeField] AudioClip music;
    [SerializeField] TextMeshProUGUI pitchText;
    float BPM;
    bool isProgressBarActive;
    // Start is called before the first frame update
    void Start()
    {
        toplineNum = 10;
        musicCheckNum = 0;
        audiosrc.clip = music;
        isPlaying = false;
        isProgressBarActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        GenerateNewLine();
        MusicProgressBar();
        SimulateSpeedManage();
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
    public int GetMusicCheckPosition() { return musicCheckNum - 4; }//return real position of check point
    public void SetMusicCheckPoint(int y) { musicCheckNum = y; }//send position number of the check point to manager
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
        }

        else
        {
            audiosrc.Stop();
            isPlaying = false;
        }
    }

    public Vector2 GetTranslatePower() { return Vector2.up * BPM / 60 * 4 * Time.deltaTime * audiosrc.pitch; }
    void MusicProgressBar()
    {
        if(isPlaying)
        {
            if(!isProgressBarActive)
            {
                progressBarObj.transform.position = new Vector3(-1, GetMusicCheckPosition(), 0);
                isProgressBarActive = true;
            }
           
            progressBarObj.transform.Translate(GetTranslatePower());
        }

        else if(!isPlaying && isProgressBarActive)
        {
            progressBarObj.transform.position = new Vector3(-1, -9, 0);
            isProgressBarActive = false;
        }       
    }

    public void SimulateSpeedManage()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (audiosrc.pitch <= 0.2f)
                audiosrc.pitch = 0.2f;

            else
            {
                audiosrc.pitch -= 0.1f;
                pitchText.text = $"Speed : {audiosrc.pitch}";
            }
        }


        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (audiosrc.pitch >= 1)
                audiosrc.pitch = 1f;

            else
            {
                audiosrc.pitch += 0.1f;
                pitchText.text = $"Speed : {audiosrc.pitch}";
            }
        }
    }
}
