using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetPanel : MonoBehaviour
{
    public TMP_Text NetStatus;

    private bool isOpen = false;

    void Update()
    {
        if (GameManager.main.isNetworkWorking)
        {
            NetStatus.text = "Connect";
        }
        else
        {
            NetStatus.text = "Disconnect";
        }
    }

    public void ShowNetPanel()
    {
        if (!isOpen)
        {
            transform.position = new Vector3(transform.position.x, 215, transform.position.z);
            isOpen = true;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, -150, transform.position.z);
            isOpen = false;
        }
    }
}
