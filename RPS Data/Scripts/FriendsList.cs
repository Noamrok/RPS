using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FriendsList : MonoBehaviour
{
    public GameObject buttonsContainer;
    public GameObject addFriendC;
    public GameObject friendsC;
    public GameObject notFoundM;
    public GameObject friendPrefab;
    private void OnEnable()
    {
        foreach (Button b in buttonsContainer.GetComponentsInChildren<Button>())
            b.interactable = false;
    }
    
    public void enableBut()
    {
        foreach (Button b in buttonsContainer.GetComponentsInChildren<Button>())
            b.interactable = true;
        addFriendC.SetActive(false);
    }

    public void addFriend()
    {
        string friendsName = GetComponentInChildren<InputField>().text;
        string uri = "https://rpsnood.azurewebsites.net/api/Player/GetFriend/" + friendsName;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            webRequest.SendWebRequest();
            while (!webRequest.isDone) ;
            if (webRequest.responseCode == 200)
            {
                GameObject text = Instantiate(friendPrefab, friendsC.transform);
                text.GetComponent<Text>().text = friendsName;
                notFoundM.SetActive(false);
            }
            else
            {
                notFoundM.SetActive(true);
            }
        }
    }
}
