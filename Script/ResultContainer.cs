using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultContainer : MonoBehaviour
{
    int perfectN;
    int greatN;
    int goodN;
    int badN;
    int missN;
    int maxCombo;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void ResultSave(int perfectN, int greatN, int goodN, int badN, int missN, int maxCombo)
    {
        
    }
}
