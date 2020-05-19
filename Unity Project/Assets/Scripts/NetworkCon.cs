using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections;

public class NetworkCon : MonoBehaviour
{
    public List<Train> trains;

    public string text = "none";

    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;

    private GameManager manager;
    private Thread mThread;
    private TcpListener listener;

    TcpClient client;
    NetworkStream nwStream;

    public bool received;
    private float randSpeed;
    private bool stop;

    private void FixedUpdate()
    {
        randSpeed = UnityEngine.Random.Range(1.5f, 2f);
    }

    public void Inst(GameManager manager)
    {
        this.manager = manager;

        ThreadStart threadStart = new ThreadStart(ReceiveThreadWhile);
        mThread = new Thread(threadStart);
        mThread.Start();

        StartCoroutine(SendRutineWhile());

        trains[0].Speed = 0;
    }

    public void Connect()
    {
        listener = new TcpListener(IPAddress.Parse(connectionIP), connectionPort);
        listener.Start();
    }

    private void ReceiveThreadWhile()
    {
        Connect();

        received = false;

        while (!stop)
        {
            if (!received)
            {
                client = listener.AcceptTcpClient();
                nwStream = client.GetStream();

                Receive();

                received = true;
            }
        }
        listener.Stop();
    }

    private IEnumerator SendRutineWhile()
    {
        while (!stop)
        {
            yield return null;

            if (received)
            {
                Send();

                nwStream.Close();
                client.Close();
            }

            received = false;
        }
        listener.Stop();
    }

    private void Receive()
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

            if (manager.starting == true)
            {
                StringToTrainsSpeed(dataReceived);
            }
        }
    }
    private void Send()
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

    private void OnDisable()
    {
        if (mThread != null)
        {
            stop = true;
            listener.Stop();
            mThread.Abort();
        }
    }

    private void OnDestroy()
    {
        if (mThread != null)
        {
            stop = true;
            listener.Stop();
            mThread.Abort();
        }
    }
}