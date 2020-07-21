using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using UnityEngine.SceneManagement;

public class WinnerMessage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.SetLocationPercent(1280, 780, 25, 75, 20, 80, true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void backToMenu()
    {
        PlayerPrefs.DeleteKey("PlayerNumInRoom");
        SceneManager.LoadScene("MainMenuScene");
    }
}
