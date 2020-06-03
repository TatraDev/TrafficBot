using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections;

public class Network : MonoBehaviour
{
    [SerializeField]
    private string connectionIP = "127.0.0.1";

    [SerializeField]
    private int connectionPort = 25001;

    private Thread mThread;
    private TcpListener listener;

    private TcpClient client;
    private NetworkStream nwStream;

    private bool stop;

    private void Start()
    {
        ThreadStart threadStart = new ThreadStart(ThreadWhile);
        mThread = new Thread(threadStart);
        mThread.Start();

    }

    private void Connect()
    {
        listener = new TcpListener(IPAddress.Parse(connectionIP), connectionPort);
        listener.Start();
    }

    private void ThreadWhile()
    {
        GameManager.main.isNetworkWorking = false;

        Connect();

        while (!stop)
        {
            client = listener.AcceptTcpClient();
            nwStream = client.GetStream();

            Receive();

            UnityMainThreadDispatcher.Instance().Enqueue(Send());

            GameManager.main.isNetworkWorking = true;
        }

        listener.Stop();
    }

    private void StringToTrainsSpeed(string sList)
    {
        string[] sArray = sList.Split(',');

        for (int i = 0; i < sArray.Length; i++)
        {
            if (sArray[i] == "1")
                GameManager.main.trains[i].Speed += Random.Range(1.5f, 2f);
            else if (sArray[i] == "0")
                GameManager.main.trains[i].Speed -= Random.Range(2.5f, 3.5f);
        }
    }

    private void Receive()
    {
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        if (dataReceived != null && dataReceived.Length > 0)
        {
            if (dataReceived == "reset")
            {
                UnityMainThreadDispatcher.Instance().EnqueueAsync(() => GameEvents.current.AddLog());
                UnityMainThreadDispatcher.Instance().EnqueueAsync(() => GameEvents.current.GameRestart());
            }
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => StringToTrainsSpeed(dataReceived));
            }
        }
    }

    private IEnumerator Send()
    {
        yield return null;

        byte[] data = Encoding.UTF8.GetBytes(GameManager.main.Info());
        nwStream.Write(data, 0, data.Length);

        nwStream.Close();
        client.Close();
    }
}