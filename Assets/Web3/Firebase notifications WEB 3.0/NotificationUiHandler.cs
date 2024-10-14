using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


namespace LobbyInGameUi
{
    public class NotificationUiHandler : MonoBehaviour
    {

        FireBaseNotificationHandler fireBaseNotificationHandler;

        [SerializeField] Transform notificationPanel_Parent;
        [SerializeField] NotificationPanelWeb3_0 notificationPanel_Prefab;


        private List<NotificationPanelWeb3_0> activeNotifications = new List<NotificationPanelWeb3_0>();


        // Start is called before the first frame update
        void Start()
        {
            fireBaseNotificationHandler = new FireBaseNotificationHandler();

            InvokeRepeating(nameof(ReadNotificationFromJsonFile),0, 2);
        }

        [ContextMenu("Read the notification from TXT file.")]
        public void ReadNotificationFromJsonFile()
        {
            fireBaseNotificationHandler = new FireBaseNotificationHandler();
            NotificationDataList notificationDataList = fireBaseNotificationHandler.ReadMessageFromJsonFile();
            if(notificationDataList.notifications.Count == 0) 
                return;
            Update_Or_Instantiate_Notification_Panel_UI(notificationDataList);
            
            Debug.Log($"{nameof(ReadNotificationFromJsonFile)}");
        }
        
        // int notificationCount = 0;
        void Update_Or_Instantiate_Notification_Panel_UI(NotificationDataList notificationDataList)
        {
            DateTime currentDate = DateTime.Now;

            for(var i = 0; i<notificationDataList.notifications.Count; i++) // foreach (var item in notificationDataList.notifications)
            {
                // notificationCount++;

                // Parse the received date of the notification
                DateTime receivedDate;
                if (!DateTime.TryParse(notificationDataList.notifications[i].receivedData, out receivedDate))
                {
                    Debug.LogError($"Invalid date format for notification: {notificationDataList.notifications[i].title}");
                    continue; 
                }

                // Calculate the difference between the current date and the received date
                TimeSpan timeDifference = currentDate - receivedDate;

                // Only process notifications that are less than 30 days old
                if (timeDifference.TotalDays <= 30)
                {
                    Debug.Log($"<color=red>Active notifications count : {activeNotifications.Count}</color>");
                    // Check if a notification with the same title and body is already displayed in the UI

                    // Instantiate a new notification panel if no duplicate is found
                    NotificationPanelWeb3_0 notificationPanelWeb3_0 = Instantiate(notificationPanel_Prefab, notificationPanel_Parent);
                    notificationPanelWeb3_0.Update_Notification_Panel(title: notificationDataList.notifications[i].title, body: notificationDataList.notifications[i].body, receivedDateTime: notificationDataList.notifications[i].receivedData);

                    // Add to the list of active notifications for future reference
                    activeNotifications.Add(notificationPanelWeb3_0);
                }
                else
                {
                    Debug.Log($"Notification skipped as it is older than 30 days: {notificationDataList.notifications[i].title}");
                }
            }
        }



        // Update is called once per frame
        void Update()
        {

        }
    }
}

