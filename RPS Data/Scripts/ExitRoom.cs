using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitRoom : MonoBehaviour
{
    private void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.SetLocationPercent(250, 100, 3, 15, 3, 15, true);
    }
    // Start is called before the first frame update
    public void exitRoom()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
