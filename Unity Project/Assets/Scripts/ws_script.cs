using System.Collections;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Text;

public class ws_script : MonoBehaviour
{
    public string ip = "localhost";
    public string port = "8000";
    public GameObject netPanel;
    private WebSocket w;

    public void Connect()
    {
        Debug.Log(ip);
        Debug.Log(port);
        StartCoroutine(StartWebSocket());
    }

    public void ChangedIp(string ip)
    {
        if (ip == "")
        {
            ip = "localhost";
        }
        this.ip = ip;
    }

    public void ChangedPort(string port)
    {
        if (port == "")
        {
            port = "8000";
        }
        this.port = port;
    }

    private IEnumerator StartWebSocket()
    {
        w = new WebSocket(new Uri("ws://" + ip + ":" + port));

        yield return StartCoroutine(w.Connect());

        w.SendString("START");

        StartCoroutine(Receive());

        while (true)
        {
            w.SendString(GameManager.main.Info());

            yield return new WaitForSeconds(0.03f);

            if (w.error != null)
            {
                Debug.LogError("Error: " + w.error);

                if (netPanel != null)
                {
                    netPanel.SetActive(true);
                }

                break;
            }
        }

        w.Close();
    }

    private IEnumerator Receive()
    {
        while (true)
        {
            string reply = w.RecvString();

            if (reply != null)
            {
                if (reply.StartsWith("AI"))
                {
                    string s = reply.Substring(2);

                    if (s == "reset")
                    {
                        UnityMainThreadDispatcher.Instance().EnqueueAsync(() => GameEvents.current.AddLog());
                        UnityMainThreadDispatcher.Instance().EnqueueAsync(() => GameEvents.current.GameRestart());
                    }
                    else
                    {
                        UnityMainThreadDispatcher.Instance().EnqueueAsync(() => StringToTrainsSpeed(s));
                    }
                }
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
                GameManager.main.trains[i].Speed += UnityEngine.Random.Range(2.5f, 4f);
            else if (sArray[i] == "0")
                GameManager.main.trains[i].Speed -= UnityEngine.Random.Range(2.5f, 4f);
        }
    }
}

