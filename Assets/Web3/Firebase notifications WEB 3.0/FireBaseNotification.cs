using Firebase.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LobbyInGameUi
{

    [Serializable]
    public class NotificationData
    {
        public string title;
        public string body;
        public string img;
        public string receivedData;
    }

    [Serializable]
    public class NotificationDataList
    {
        public List<NotificationData> notifications;
    }

    public class FireBaseNotificationHandler : MonoBehaviour
    {
        string filePath => Application.persistentDataPath + "/FirebaseNotification.txt";

        #region Monobehaviour callbacks
        void Start()
        {
                
        }
        #endregion

        // Read notifications from the JSON file
        public NotificationDataList ReadMessageFromJsonFile()
        {
            NotificationDataList notificationList = new NotificationDataList();

            // Check if the file exists
            if (File.Exists(filePath))
            {
                string jsonFile = File.ReadAllText(filePath);
                notificationList = JsonUtility.FromJson<NotificationDataList>(jsonFile); // "{\"notifications\":" + jsonFile + "}"
                //Debug.Log($"{nameof(FireBaseNotificationHandler)} \t {nameof(ReadMessageFromJsonFile)} \t Files Exits \t Notification List {notificationList} \n {jsonFile}");
            }
            else
            {
                // Create the file if it doesn't exist
                File.Create(filePath).Dispose();  // Dispose to avoid file lock
                //Debug.Log("<color=green>File Created</color>");
            }
            return notificationList;
        }

        // Write a new notification to the file
        public void WriteDataToFile(NotificationData notificationData)
        {

            // First, read the existing data
            List<NotificationData> existingData = ReadDataFromFile();

            // Add the new notification to the list
            existingData.Add(notificationData);

            // Convert the updated list back to JSON
            string updatedMessageData = JsonUtility.ToJson(new NotificationDataList { notifications = existingData }, true);

            try
            {
                // Write the updated data back to the file
                using (StreamWriter writer = new StreamWriter(filePath, false))  // 'false' overwrites the file
                {
                    writer.Write(updatedMessageData);
                }
                Debug.Log("Data successfully written to " + filePath);
            }
            catch
            {
                Debug.Log("<color=red>Failed to write to the file.</color>");
            }
        }

        // Helper method to read existing notifications from the file
        private List<NotificationData> ReadDataFromFile()
        {
            List<NotificationData> notifications = new List<NotificationData>();

            // Check if the file exists
            if (File.Exists(filePath))
            {
                string jsonFile = File.ReadAllText(filePath);

                if (!string.IsNullOrEmpty(jsonFile))
                {
                    // Deserialize the existing JSON data into a list
                    NotificationDataList notificationList = JsonUtility.FromJson<NotificationDataList>("{\"notifications\":" + jsonFile + "}");
                    if (notificationList != null)
                    {
                        notifications = notificationList.notifications;
                    }
                }
            }
            return notifications;
        }




    }
}

