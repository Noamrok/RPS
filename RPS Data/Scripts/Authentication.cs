using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Authentication : MonoBehaviour
{
    public InputField username; //inputfield of username
    public InputField password; //inputfield of username
    public Toggle checkbox; //checkbox of remember me
    private string Username; //saved username
    private string Password; //saved password

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("LoggedOut"))
        {
            if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
            {
                username.text = PlayerPrefs.GetString("Username");
                password.text = PlayerPrefs.GetString("Password");
                checkbox.isOn = true;
            }
            else if (PlayerPrefs.HasKey("Username"))
            {
                username.text = PlayerPrefs.GetString("Username");
                checkbox.isOn = false;
            }
            PlayerPrefs.DeleteKey("LoggedOut");
        }
        else if (PlayerPrefs.HasKey("Username")) //check if there is any saved username
        {
            username.text = PlayerPrefs.GetString("Username");
            checkbox.isOn = false;
        }
            //Username = PlayerPrefs.GetString("Username");
            //Password = PlayerPrefs.GetString("Password");
            //username.SetTextWithoutNotify(Username);
            //password.SetTextWithoutNotify(Password);
            //if (!checkbox.isOn) //if the check box is off, turn it on
            //    checkbox.isOn = true;
    }

    public void Authenticate()
    {
        StartCoroutine(getPlayerLoginInfo());
    }

    private IEnumerator getPlayerLoginInfo()
    {
        string uri = "https://rpsnood.azurewebsites.net/api/Player/GetPlayerLoginInfo/" + username.text;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.responseCode == 404)
                MoveScene("LoginScene");
            else
            {
                PlayerLoginInfo p = JsonUtility.FromJson<PlayerLoginInfo>(webRequest.downloadHandler.text);
                if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
                {
                    if (PlayerPrefs.GetString("Password") == password.text)
                    {
                        RememberMe();
                        MoveScene("MainMenuScene");
                    }
                    else if (p.Password == password.text)
                    {
                        RememberMe();
                        MoveScene("MainMenuScene");
                    }
                }
                else if (p.Password == password.text)
                {
                    RememberMe();
                    MoveScene("MainMenuScene");
                }
                else
                    MoveScene("LoginScene");
            }
        }
    }
    public void RememberMe()
    {
        PlayerPrefs.SetString("Username", username.text);
        if (checkbox.isOn)
        {
            //if checkbox is on, save username and password
            PlayerPrefs.SetString("Password", password.text);
        }
        else
        {
            //if checkbox turn off, delete the saved username and password
            //PlayerPrefs.DeleteKey("Username");
            PlayerPrefs.DeleteKey("Password");
        }
    }

    public void MoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); //load scene with sceneName
    }

    public void ExitGame()
    {
        Application.Quit(); //exit application
    }

    [Serializable]
    private class PlayerLoginInfo
    {
        public string Username;
        public string Password;
    }
}
