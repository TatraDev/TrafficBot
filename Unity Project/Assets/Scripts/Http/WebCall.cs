using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class WebCall : MonoBehaviour
{
    public TMP_Text unityDataText;
    public TMP_Text actionText;

    readonly string unityURL = "http://127.0.0.1:8000/TrafficBot/unity";
    readonly string botURL = "http://127.0.0.1:8000/TrafficBot/bot";

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(PostRequestUnityData());
        StartCoroutine(GetRequestAction());
    }

    IEnumerator PostRequestUnityData()
    {
        while (true)
        {
            List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
            wwwForm.Add(new MultipartFormDataSection("unity_data", GameManager.main.Info()));

            UnityWebRequest www = UnityWebRequest.Post(unityURL, wwwForm);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                unityDataText.text = www.downloadHandler.text;
            }

            yield return null;
        }
    }

    private void StringToTrainsSpeed(string sList)
    {
        string[] sArray = sList.Split(',');

        for (int i = 0; i < sArray.Length; i++)
        {
            if (sArray[i] == "1")
                GameManager.main.trains[i].Speed += Random.Range(2.5f, 4f);
            else if (sArray[i] == "0")
                GameManager.main.trains[i].Speed -= Random.Range(2.5f, 4f);
        }
    }

    IEnumerator GetRequestAction()
    {
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get(botURL);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                actionText.text = data;

                if (data != "")
                {
                    if (data == "reset")
                    {
                        GameEvents.current.AddLog();
                        GameEvents.current.GameRestart();
                    }
                    else
                    {
                        StringToTrainsSpeed(data);
                    }
                }
            }
        }
    }
}

