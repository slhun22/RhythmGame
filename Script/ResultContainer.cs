using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultContainer : MonoBehaviour
{
    [SerializeField] float waitTime;
    int cnt;
    public int PerfectN { get; private set; }
    public int GreatN { get; private set; }
    public int GoodN { get; private set; }
    public int BadN { get; private set; }
    public int MissN { get; private set; }
    public int MaxCombo { get; private set; }
    public int EarlyN { get; private set; }
    public int LateN { get; private set; }
    public string FinalState { get; private set; }
    public string SongName { get; private set; }
    public string Composer {  get; private set; }
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void OnEnable()
    {
        EndTimer(waitTime).Forget();
    }
    async UniTaskVoid EndTimer(float waitTime)
    {
        Debug.Log("EndTimer Activated");
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        Debug.Log("Send Result Start");
        SaveGameResult();
        SceneManager.LoadScene("ResultScene");
    }
    void SaveGameResult()
    {
        PerfectN = GameManager.instance.PerfectNum;
        GreatN = GameManager.instance.GreatNum;
        GoodN = GameManager.instance.GoodNum;
        BadN = GameManager.instance.BadNum;
        MissN = GameManager.instance.MissNum;
        MaxCombo = GameManager.instance.MaxCombo;
        EarlyN = GameManager.instance.EarlyNum;
        LateN = GameManager.instance.LateNum;
        SongName = GameManager.instance.SongName;
        Composer = GameManager.instance.Composer;
        switch (GameManager.instance.FinalStateResult)
        {
            case GameManager.FinalState.AP:
                FinalState = "AP";
                break;
            case GameManager.FinalState.FC:
                FinalState = "FC";
                break;
            case GameManager.FinalState.NO:
                FinalState = "C";
                break;
        }
    }
}
