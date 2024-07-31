using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneManager : MonoBehaviour
{
    ResultContainer resultContainer;
    [SerializeField] TextMeshProUGUI songNameText;
    [SerializeField] TextMeshProUGUI composerText;
    [SerializeField] TextMeshProUGUI perfectNumText;
    [SerializeField] TextMeshProUGUI greatNumText;
    [SerializeField] TextMeshProUGUI goodNumText;
    [SerializeField] TextMeshProUGUI badNumText;
    [SerializeField] TextMeshProUGUI missNumText;
    [SerializeField] TextMeshProUGUI maxComboText;
    [SerializeField] TextMeshProUGUI earlyNumText;
    [SerializeField] TextMeshProUGUI lateNumText;
    [SerializeField] TextMeshProUGUI finalStateText;
    [SerializeField] VertexGradient AP_Color;
    [SerializeField] VertexGradient FC_Color;
    [SerializeField] VertexGradient C_Color;

    private void Start()
    {
        resultContainer = GameObject.Find("EndMarker").GetComponent<ResultContainer>();
        PasteResult();
    }
    void PasteResult()
    {
        songNameText.text = resultContainer.SongName;
        composerText.text = resultContainer.Composer;
        perfectNumText.text = resultContainer.PerfectN.ToString("D4");
        greatNumText.text = resultContainer.GreatN.ToString("D4");
        goodNumText.text = resultContainer.GoodN.ToString("D4");
        badNumText.text = resultContainer.BadN.ToString("D4");
        missNumText.text = resultContainer.MissN.ToString("D4");
        earlyNumText.text = resultContainer.EarlyN.ToString("D4");
        lateNumText.text = resultContainer.LateN.ToString("D4");
        maxComboText.text = resultContainer.MaxCombo.ToString("D4");

        finalStateText.text = resultContainer.FinalState;
        switch (resultContainer.FinalState)
        {
            case "AP":
                finalStateText.colorGradient = AP_Color;
                break;
            case "FC":
                finalStateText.colorGradient = FC_Color;
                break;
            case "C":
                finalStateText.colorGradient = C_Color;
                break;
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
