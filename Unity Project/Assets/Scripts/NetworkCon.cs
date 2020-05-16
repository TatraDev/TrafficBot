using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections;
using System.Globalization;

public class NetworkCon : MonoBehaviour
{
    GameManager manager;
    Thread mThread;

    public List<Train> trains;

    float train_1_speed = 1;

    public string text = "none";

    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;

    TcpListener listener;

    bool running;

    bool resetScene = false;

    public void Start()
    {
        manager = GetComponent<GameManager>();
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    private void Update()
    {
        trains[0].Speed = train_1_speed;
    }

    private void GetInfo()
    {
        try
        {
            listener = new TcpListener(IPAddress.Parse(connectionIP), connectionPort);
            listener.Start();

            manager.reset = true;

            running = true;
            while (running)
            {
                Connection();
            }
            listener.Stop();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }
    }

    private void Connection()
    {
        TcpClient client = listener.AcceptTcpClient();

        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        byte[] data = Encoding.UTF8.GetBytes(manager.Info());
        nwStream.Write(data, 0, data.Length);

        if (manager.reset == true)
        {
            manager.bonus = 0;
            manager.time = 0;
            manager.reset = false;
        }

        if (dataReceived != null && dataReceived.Length > 0)
        {
            if (dataReceived == "stop")
            {
                running = false;
            }
            else
            {
                train_1_speed = StringToFloat(dataReceived) * trains[0].MaxSpeed;
                Debug.Log("speed in percent " + dataReceived);
                Debug.Log("speed " + train_1_speed);
            }
        }

        nwStream.Close();
        client.Close();
    }

    private float StringToFloat(string sFloat)
    {
        float result = 0;
        float.TryParse(sFloat, NumberStyles.Any, CultureInfo.InvariantCulture, out result);

        return result;
    }
}