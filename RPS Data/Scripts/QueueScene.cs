using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Runtime.CompilerServices;

public class QueueScene : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log("OnEnable");
        StartCoroutine(EnterQueueOnRoom());
    }

    IEnumerator ReadyToStartGame()
    {
        bool isReady = false;
        while (!isReady)
        {
            isReady = poolRoom();
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene("Board");
    }
        
    bool poolRoom()
    {
        //string uri = "https://localhost:44324/api/Queue/GetReadyForGame/" + "Noam"; //PlayerPrefs.GetString("Username");
        string uri = "https://rpsnood.azurewebsites.net/api/Queue/GetReadyForGame/" + PlayerPrefs.GetString("Username");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SendWebRequest();
            while (!webRequest.isDone) ;
            if (webRequest.responseCode == 200)
                return true;
            return false;
        }
    }
    
    IEnumerator EnterQueueOnRoom()
    {
        Debug.Log("EnterQueueOnRoom");
        //string uri = "https://localhost:44324/api/Queue/PostPlayerToQueue";
        string uri = "https://rpsnood.azurewebsites.net/api/Queue/PostPlayerToQueue";

        WWWForm form = new WWWForm();
        //form.AddField("username", "Noam");
        form.AddField("username", PlayerPrefs.GetString("Username"));

        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();
            if(www.responseCode == 201)
            {
                PlayerPrefs.SetInt("PlayerNumInRoom", 1);
                StartCoroutine(ReadyToStartGame());
            }
            else if(www.responseCode == 200)
            {
                PlayerPrefs.SetInt("PlayerNumInRoom", 2);
                SceneManager.LoadScene("Board");
            }
        }
    }
}
