using LobbyInGameUi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationPanelWeb3_0 : MonoBehaviour
{

    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text body;
    [SerializeField] TMP_Text deliveredDateTime;

    

    public void UpdateNotificationPanelContent(FirebaseNotificationMessageData firebaseNotificationMessageData)
    {
        title.text = firebaseNotificationMessageData.Title;
        body.text = firebaseNotificationMessageData.Body;
        deliveredDateTime.text = firebaseNotificationMessageData.MessagedeliveredDateTime.ToString();
        
    }

    
}
