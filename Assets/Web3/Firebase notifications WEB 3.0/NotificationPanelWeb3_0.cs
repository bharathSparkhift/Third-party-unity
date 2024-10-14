using LobbyInGameUi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class NotificationPanelWeb3_0 : MonoBehaviour
{
    [field: SerializeField] public TMP_Text title_TMP_Text { get; private set; }
    [field: SerializeField] public TMP_Text body_TMP_Text { get; private set; }
    [field: SerializeField] public TMP_Text date_Time_TMP_Text { get; private set; }

    public void Update_Notification_Panel(string title, string body, string receivedDateTime)
    {
        title_TMP_Text.text = title;
        body_TMP_Text.text = body;
        date_Time_TMP_Text.text = receivedDateTime;
    }
    
}
