using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class MainMenuMovement : MonoBehaviour
{
    public RectTransform MenuContianer;
    private RectTransform[] MenuWindows;
    private float smoothspeed = 10f;
    private Vector3 desiredPosition;
    private Vector3[] menuPositions;
    private bool[] active;
    public GameObject waitingQ;
    public GameObject addFriendC;
    public Button QCancelButton;
    public Button[] buttons;
    public bool areButtonsDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        MenuWindows = new RectTransform[MenuContianer.transform.childCount];
        for (int i = 0;i < MenuContianer.transform.childCount; i++)
            MenuWindows[i] = (RectTransform)MenuContianer.GetChild(i);
        menuPositions = new Vector3[MenuWindows.Length];
        active = new bool[MenuWindows.Length];
        for (int i = 0; i < MenuWindows.Length; i++)
        {
            menuPositions[i] = MenuWindows[i].anchoredPosition;
            active[i] = false;
        }
        buttons = MenuContianer.GetChild(0).GetComponentsInChildren<Button>();
    }
    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < MenuWindows.Length; i++)
        {
            if(active[i])
                if(i != 3)
                    MenuWindows[i].anchoredPosition = Vector3.MoveTowards(MenuWindows[i].anchoredPosition, MenuContianer.anchoredPosition, smoothspeed);
                else
                    MenuWindows[i].anchoredPosition = Vector3.MoveTowards(MenuWindows[i].anchoredPosition, desiredPosition, smoothspeed);
            else
                MenuWindows[i].anchoredPosition = Vector3.MoveTowards(MenuWindows[i].anchoredPosition, menuPositions[i], smoothspeed);
        }
    }
    public void Logout()
    {
        PlayerPrefs.SetInt("LoggedOut", 1);
        SceneManager.LoadScene("LoginScene"); //load scene with sceneName
    }
    public void showCredits()
    {
        SceneManager.LoadScene("CreditsScene"); //load scene with sceneName
    }
    public void MoveMenu(int id)
    {
        if (id == 3)
            desiredPosition = new Vector3(MenuWindows[id].anchoredPosition.x - MenuWindows[id].rect.width, 70, 0);
        active[id] ^= true;
        if (!areButtonsDisabled)
        {
            foreach (Button b in buttons)
                b.interactable = false;
            areButtonsDisabled = true;
        }
        else
        {
            foreach (Button b in buttons)
                b.interactable = true;
            areButtonsDisabled = false;
        }
    }
    public void QuickPlay()
    {
        foreach (Button b in buttons)
            b.interactable = false;
        areButtonsDisabled = true;
        waitingQ.SetActive(true);
        Debug.Log("QuickPlay");
    }
    public void addAFriend()
    {
        addFriendC.SetActive(true);
    }
    public void CancelQueue()
    {
        PlayerPrefs.DeleteKey("PlayerNumInRoom");
        StartCoroutine(LeaveQueue());
        foreach (Button b in buttons)
            b.interactable = true;
        areButtonsDisabled = false;
        waitingQ.SetActive(false);
    }
    IEnumerator LeaveQueue()
    {
        string uri = "https://rpsnood.azurewebsites.net/api/Queue/DeletePlayerFromQueue/" + PlayerPrefs.GetString("Username");
        using (UnityWebRequest www = UnityWebRequest.Delete(uri))
        {
            yield return www.SendWebRequest();
        }
    }
}
