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
    public List<Train> trains;

    public string text = "none";

    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;

    private GameManager manager;
    private Thread mThread;
    private TcpListener listener;

    public bool running;
    private float randSpeed;

    private void Update()
    {
        randSpeed = UnityEngine.Random.Range(1.5f, 2f);
    }

    public void Inst(GameManager manager)
    {
        this.manager = manager;

        StartCoroutine(Listen());

        trains[0].Speed = 0;
    }

    IEnumerator Listen()
    {
        listener = new TcpListener(IPAddress.Parse(connectionIP), connectionPort);
        listener.Start();

        running = true;
        while (running)
        {
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream nwStream = client.GetStream();

            Receive(client, nwStream);

            yield return new WaitForEndOfFrame();

            Send(client, nwStream);

            nwStream.Close();
            client.Close();
        }
        listener.Stop();
    }

    void Receive(TcpClient client, NetworkStream nwStream)
    {

        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Debug.Log("Data received " + dataReceived);

        if (dataReceived != null && dataReceived.Length > 0)
        {
            if (dataReceived == "start")
            {
                manager.starting = true;
                manager.reset = true;
            }

            if(dataReceived == "stop")
            {
                running = false;
            }

            if (manager.starting == true)
            {
                StringToTrainsSpeed(dataReceived);
            }
        }
    }
    void Send(TcpClient client, NetworkStream nwStream)
    {
        byte[] data = Encoding.UTF8.GetBytes(manager.Info());
        nwStream.Write(data, 0, data.Length);

        if (manager.reset == true)
        {
            manager.bonus = 0;
            manager.time = 0;
            manager.reset = false;
        }
    }

    private void StringToTrainsSpeed(string sList)
    {
        string[] sArray = sList.Split(',');

        Debug.Log("Length received " + sArray.Length + "Length trains " + trains.Count);

        for (int i = 0; i < sArray.Length; i++)
        {
            if (sArray[i] == "1")
                trains[i].Speed += randSpeed;
            else if (sArray[i] == "0")
                trains[i].Speed -= randSpeed;

            Debug.Log("train_" + i + " speed = " + trains[i].Speed);
        }
    }
}