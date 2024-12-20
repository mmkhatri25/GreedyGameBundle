﻿using System.Net;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
namespace SocketIO
{
    public class SplashScreenScript : MonoBehaviour
    {

        public string GreedyGameScene;
        public bool isUAT;

        public string url, uatUrl, productionUrl, gameId;
        public string socketUrl;
        public string userIdUat, userIdLive, userIdVar;
        public string GameId;

        [Serializable]
        public class BalanceREsponse
        {
            public int status;
            public string message;
            public Result result;
        }
        public class Result
        {
            public string currentBalance;
            public bool isCoinSeller;
            public string name, gameId, storeId;
        }
        public class SendData
        {
            public string userId;
        }

        private void Awake()
        {

#if (UNITY_EDITOR)
            if (isUAT)
            {
                url = uatUrl;
                userIdVar = userIdUat;
            }
            else
            {
                url = productionUrl;
                userIdVar = userIdLive;

            }

            StartCoroutine(GetDataApi(url, userIdVar));
            PlayerPrefs.SetString("userId", userIdVar);
            PlayerPrefs.SetString("GameId", GameId);
            //Debug.Log("===== game id - " + GameId + "player prefs game id " + PlayerPrefs.GetString("GameId"));

#else
            getIntentData ();

#endif

        }

        private bool getIntentData()
        {
#if (!UNITY_EDITOR && UNITY_ANDROID)
        return CreatePushClass (new AndroidJavaClass ("com.unity3d.player.UnityPlayer"));
#endif
            return false;
        }

        public bool CreatePushClass(AndroidJavaClass UnityPlayer)
        {
#if UNITY_ANDROID
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            AndroidJavaObject extras = GetExtras(intent);

            if (extras != null)
            {
                string ex = GetProperty(extras, "userId");
                url = GetProperty(extras, "url");
                gameId = GetProperty(extras, "GameId");
                socketUrl = GetProperty(extras, "socketUrl");
                PlayerPrefs.SetString("socketurl", socketUrl);
                PlayerPrefs.SetString("userId", ex);
                PlayerPrefs.SetString("GameId", gameId);
                return true;
            }
#endif
            return false;
        }

        private AndroidJavaObject GetExtras(AndroidJavaObject intent)
        {
            AndroidJavaObject extras = null;

            try
            {
                extras = intent.Call<AndroidJavaObject>("getExtras");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            return extras;
        }

        private string GetProperty(AndroidJavaObject extras, string name)
        {
            string s = string.Empty;

            try
            {
                s = extras.Call<string>("getString", name);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            return s;
        }

        public void Start()
        {
            StartCoroutine(Wait());
        }
        IEnumerator Wait()
        {
            // RingFillImg.fillAmount = 0;

            // while (RingFillImg.fillAmount < 1)
            // {
            //     RingFillImg.fillAmount += 0.05f;
            //     yield return new WaitForSeconds(0.05f);
            // }
            // StartCoroutine(GetDataApi("http://216.48.182.176:4000/auth/login", "63aeec564718b29ebf9a0198"));
            // yield return new WaitForSeconds(2f);
            yield return new WaitWhile(() => PlayerPrefs.GetString("userId") == string.Empty);

            StartCoroutine(GetDataApi(url + "/auth/login", PlayerPrefs.GetString("userId")));
            //StartCoroutine(GetDataApi("https://gapi.yaravoice.com:4002/auth/login", PlayerPrefs.GetString("userId"))); //production



        }

        public IEnumerator GetDataApi(string URL, string userid)
        {
            WWWForm form = new WWWForm();
            form.AddField("userId", userid);

            using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    //Debug.Log(www.error);
                }
                else
                {
                    BalanceREsponse result = Newtonsoft.Json.JsonConvert.DeserializeObject<BalanceREsponse>(www.downloadHandler.text);
                    //Debug.Log("result   " + result.result.name);
                    if (result.status == 200)
                    {
                        Debug.Log("Data Recieved successfully.... " + www.downloadHandler.text);
                        PlayerPrefs.SetString("currentBalance", result.result.currentBalance);
                        PlayerPrefs.SetString("gameId", result.result.gameId);
                        PlayerPrefs.SetString("GameIdSaved", PlayerPrefs.GetString("GameId"));
                        //Debug.Log("==== result on   GetDataApi gameid - " + result.result.gameId);
                        PlayerPrefs.SetString("storeId", result.result.storeId);
                        PlayerPrefs.SetString("name", result.result.name);
                        //SceneManager.LoadScene(1);
                        //Debug.Log("PlayerPrefs gameid - " + PlayerPrefs.GetString("GameId"));

                        if (PlayerPrefs.GetString("GameId") == "4")
                            SceneManager.LoadScene(1);
                        else
                            SceneManager.LoadScene(2);

                    }
                    else
                    {
                        if (result.status == 400)
                        {
                            //Debug.Log("Status Code" + result.status);
                            AndroidToastMsg.ShowAndroidToastMessage(result.message);
                        }
                        try
                        {
                            // Application.Quit();
                            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                            activity.Call("finish");
                        }
                        catch (UnityException ex)
                        {
                            Debug.Log("Exception:" + ex.ToString() + ex.HelpLink + ex.HResult);
                        }
                    }
                }
            }
        }
    }
}