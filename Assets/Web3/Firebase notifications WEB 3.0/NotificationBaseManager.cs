using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationBaseManager : MonoBehaviour
{

    public string title;
    public string body;
    public string img;
    public string receivedData;

    public NotificationBaseManager(string title, string body, string img, string receivedData)
    {
        this.title = title;
        this.body = body;
        this.img = img;
        this.receivedData = receivedData;
    }
}
