using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileSceneManager : MonoBehaviour
{
    public Text[] stats;
    public Text[] info;
    
    public void FetchPlayerStats()
    {
        StartCoroutine(getPlayerStats());
    }
    private IEnumerator getPlayerStats()
    {
        string uri = "https://rpsnood.azurewebsites.net/api/Player/GetPlayerStats/" + PlayerPrefs.GetString("Username");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
                Debug.Log(": Error: " + webRequest.error);
            else
            {
                PlayerModel p = JsonUtility.FromJson<PlayerModel>(webRequest.downloadHandler.text);
                stats[0].text = p.NumOfGames.ToString();
                stats[1].text = p.Wins.ToString();
                stats[2].text = p.Loses.ToString();
                if (p.Loses == 0)
                    stats[3].text = "Inf";
                else
                    stats[3].text = (Convert.ToDouble(p.Wins) / p.Loses).ToString();
                info[0].text = p.Username;
                info[1].text = p.Email;
            }
        }
    }

    [Serializable]
    private class PlayerModel
    {
        public string Username;
        public string Password;
        public string Email;
        public int Wins;
        public int Loses;
        public int NumOfGames;
    }
}
