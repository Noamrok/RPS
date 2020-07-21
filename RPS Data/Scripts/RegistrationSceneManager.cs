using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System;

public class RegistrationSceneManager : MonoBehaviour
{
    private const int USNV = 0;
    private const int PSWV = 1;
    private const int PSWMV = 2;
    private const int EMV = 3;
    
    public InputField usernameInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;
    public InputField emailInput;

    public GameObject butContainer;
    private Button[] buttons;

    //public GameObject usernameDetails;
    //public GameObject passwordDetails;
    //public GameObject emailDetails;

    private void Start()
    {
        buttons = butContainer.GetComponentsInChildren<Button>();
    }
    private bool[] isValidArray = { false, false, false, false };
    public void usernameIntegrityCheck()
    {
        Image usernameImage = usernameInput.GetComponent<Image>();
        Regex reg = new Regex(@"^[a-zA-Z]{1}\w{3,7}$");
        //Debug.Log(reg.IsMatch(usernameInput.text) ? $"new things are being typed: {usernameInput.text})" : "not good");
        if (reg.IsMatch(usernameInput.text))
        {
            //usernameDetails.SetActive(false);
            usernameImage.color = Color.green;
            isValidArray[USNV] = true;
        }    
        else if(usernameInput.text == "")
        {
            //usernameDetails.SetActive(false);
            usernameImage.color = Color.white;
            isValidArray[USNV] = false;
        }    
        else
        {
            //usernameDetails.SetActive(true);
            usernameImage.color = Color.red;
            isValidArray[USNV] = false;
        }   
    }
    public void passwordIntegrityCheck()
    {
        Image passwordImage = passwordInput.GetComponent<Image>();
        Regex reg = new Regex(@"^\S{6,10}$");
        //Debug.Log(reg.IsMatch(usernameInput.text) ? $"new things are being typed: {usernameInput.text})" : "not good");
        if (reg.IsMatch(passwordInput.text))
        {
            //passwordDetails.SetActive(false);
            passwordImage.color = Color.green;
            isValidArray[PSWV] = true;
        }
        else if (passwordInput.text == "")
        {
            //passwordDetails.SetActive(false);
            passwordImage.color = Color.white;
            isValidArray[PSWV] = false;
        }
        else
        {
            //passwordDetails.SetActive(true);
            passwordImage.color = Color.red;
            isValidArray[PSWV] = false;
        }
        if (confirmPasswordInput.text != "")
            confirmPasswordCheck();
    }
    public void confirmPasswordCheck()
    {
        Image confirmPasswordImage = confirmPasswordInput.GetComponent<Image>();
        if (confirmPasswordInput.text == passwordInput.text)
        {
            confirmPasswordImage.color = Color.green;
            isValidArray[PSWMV] = true;
        }
        else if (confirmPasswordInput.text == "")
        {
            confirmPasswordImage.color = Color.white;
            isValidArray[PSWMV] = false;
        }
        else
        {
            confirmPasswordImage.color = Color.red;
            isValidArray[PSWMV] = false;
        }
    }
    public void emailIntegrityCheck()
    {
        Image emailImage = emailInput.GetComponent<Image>();
        Regex reg = new Regex(@"^\w{1,40}@(gmail|outlook|yahoo|walla|hotmail)\.(com|co\.il)$");
        //Debug.Log(reg.IsMatch(usernameInput.text) ? $"new things are being typed: {usernameInput.text})" : "not good");
        if (reg.IsMatch(emailInput.text))
        {
            //emailDetails.SetActive(false);
            emailImage.color = Color.green;
            isValidArray[EMV] = true;
        }
        else if (emailInput.text == "")
        {
            //emailDetails.SetActive(false);
            emailImage.color = Color.white;
            isValidArray[EMV] = false;
        }
        else
        {
            //emailDetails.SetActive(true);
            emailImage.color = Color.red;
            isValidArray[EMV] = false;
        }
    }
    public void backToLogin()
    {
        SceneManager.LoadScene("LoginScene");
    }
    public void moveScene()
    {
        foreach (Button b in buttons)
            b.interactable = false;
        bool allValid = true;
        for (int i = 0; i < 4; i++)
            if (isValidArray[i] == false)
            {
                allValid = false;
                break;
            }
        if(allValid == true)
            StartCoroutine(savePlayer());
    }
    private IEnumerator savePlayer()
    {
        string uri = "https://rpsnood.azurewebsites.net/api/Player/PostNewPlayer";
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("password", passwordInput.text);
        form.AddField("email", emailInput.text);

        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();

            if (www.responseCode == 400)
                SceneManager.LoadScene("RegistrationScene");
            else
            {
                Debug.Log("Form upload complete!");
                if(!PlayerPrefs.HasKey("Username"))
                    PlayerPrefs.SetString("Username", usernameInput.text);
                SceneManager.LoadScene("LoginScene");
            }
        }
    }
}