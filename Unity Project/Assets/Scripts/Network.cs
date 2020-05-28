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

    private float randSpeed = 0;
    private bool isStart;
    private bool isReceive;
    private bool stop;

    private void Update()
    {
        randSpeed = Random.Range(1.5f, 2f);
    }

    public void Inst()
    {
        ThreadStart threadStart = new ThreadStart(ReceiveThreadWhile);
        mThread = new Thread(threadStart);
        mThread.Start();

        StartCoroutine(SendRutineWhile());
    }

    public void Connect()
    {
        listener = new TcpListener(IPAddress.Parse(connectionIP), connectionPort);
        listener.Start();
    }

    private void ReceiveThreadWhile()
    {
        Connect();

        isReceive = false;

        while (!stop)
        {
            if (!isReceive)
            {
                client = listener.AcceptTcpClient();
                nwStream = client.GetStream();

                Receive();

                isReceive = true;
            }
        }
        listener.Stop();
    }

    private IEnumerator SendRutineWhile()
    {
        float i = 0;

        while (!stop)
        {
            yield return null;

            if (i < 6)
            {
                i += Time.deltaTime;
            }
            else if (!isReceive)
            {
                Debug.Log("Disconnect");
            }

            if (isReceive)
            {
                Send();

                nwStream.Close();
                client.Close();

                i = 0;
            }

            isReceive = false;
        }
        listener.Stop();
    }

    private void Receive()
    {
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        if (dataReceived != null && dataReceived.Length > 0)
        {
            if (dataReceived == "start")
            {
                GameManager.main.ResetScore();
                isStart = true;
            }

            if (isStart && GameManager.main.isReady)
            {
                StringToTrainsSpeed(dataReceived);
            }
        }
    }
    private void Send()
    {
        byte[] data = Encoding.UTF8.GetBytes(GameManager.main.Info());
        nwStream.Write(data, 0, data.Length);

        if (GameManager.main.reset == true)
        {
            GameManager.main.reset = false;
            GameManager.main.ResetScore();
        }
    }

    private void StringToTrainsSpeed(string sList)
    {
        string[] sArray = sList.Split(',');

        for (int i = 0; i < sArray.Length; i++)
        {
            if (sArray[i] == "1")
                GameManager.main.trains[i].Speed += randSpeed;
            else if (sArray[i] == "0")
                GameManager.main.trains[i].Speed -= randSpeed;
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