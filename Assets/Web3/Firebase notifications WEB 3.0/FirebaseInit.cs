using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Messaging;
using Firebase;
using Firebase.Analytics;
using System.Text;
using UnityEngine.UI;
using System;
using Firebase.Extensions;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
//using Firebase.Extensions;

namespace LobbyInGameUi
{

    [Serializable]
    public class FirebaseNotificationListWrapper
    {
        public List<FirebaseNotificationMessageData> notifications = new List<FirebaseNotificationMessageData>();

    }

    [Serializable]
    public class FirebaseNotificationMessageData
    {
        public string MessageId;
        public string Title;
        public string Body;
        public string Img;
        public string MessagedeliveredDateTime;
        public string MessageTobeDeletedDateTime;
        public override string ToString()
        {
            return $"Message Id \t {MessageId} \n  Title \t {Title} \n Body \t {Body} \n Message Delivered Date Time \t {MessagedeliveredDateTime} \n Message Deleting Date Time \t {MessageTobeDeletedDateTime}";
        }
    }



    public class FirebaseInit : MonoBehaviour
    {

        [SerializeField] Transform notificationPanelParentTransform;
        [SerializeField] NotificationPanelWeb3_0 notificationPanel;
        [SerializeField] TMP_Text output;


        #region Private fields
        string FirebaseDataFilePath => Application.persistentDataPath + "/FirebaseData.txt";
        bool _notificationPanelInstantiated = false;
        FirebaseNotificationListWrapper firebaseNotificationListWrapper;
        List<NotificationPanelWeb3_0> notificationPool = new List<NotificationPanelWeb3_0>();
        #endregion


        private void Awake()
        {

            firebaseNotificationListWrapper = new FirebaseNotificationListWrapper();
        }


        private void Start()
        { 
            
            Debug.Log("Initializing firebase...");
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                // var dependencyStatus = task.Result;
                if (task.Result == Firebase.DependencyStatus.Available)
                {
                    Debug.Log("Firebase initialized successfully!");
                    output.text = $"<color=green>Firebase initialized successfully!</color>";

                    // InstantiateNotificationPanel();

                    // Check for file
                    if (!File.Exists(FirebaseDataFilePath))
                    {
                        File.Create(FirebaseDataFilePath);
                        output.text += "\n<color=green>New file created....</color>";
                        Debug.Log("New file created....");
                    }
                    /*else
                    {
                        FetchNotificationsFromFile();
                    }*/
                    InitializeFirebaseCloudMessaging();
                    InitializeFirebaseAnalytics();
       
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", task.Result));
                    output.text = $"<color=red>Could not resolve all Firebase dependencies: {task.Result}</color>";
                }
            });

            
        }

        private void OnEnable()
        {
            FetchNotificationsFromFile();
        }

        private bool InitializeFirebaseCloudMessaging()
        {

            bool out_ = false;

            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

            Debug.Log($"{nameof(InitializeFirebaseCloudMessaging)}");

            Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(
                task =>
                {
              
                    if (task.IsCompleted)
                    {
                        Debug.Log("Messaging permission granted");
                        output.text = $"<color=green>Messaging permission granted</color>";
                        return true;
                    }
                    else
                    {
                        Debug.LogError("Messaging permission request failed");
                        output.text = $"<color=red>Messaging permission request failed</color>";
                        return false;
                    }
                });
            return out_;

        }

        private void InitializeFirebaseAnalytics()
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            Debug.Log($"{nameof(InitializeFirebaseAnalytics)}");
        }


        private void OnDestroy()
        {
            Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }

        


        private void OnTokenReceived(object sender, TokenReceivedEventArgs e)
        {
            try
            {
                Debug.Log($"Received remote notification token: ");
                Debug.Log($"{e.Token}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
                output.text = $"<color=red>{ex.Message}</color>";
            }
        }
       
        /// <summary>
        /// Firebase message callbacks event.
        /// Invoked when clicked on notification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.Log($"Received remote message: {JsonConvert.SerializeObject(e)} \n Message type {e.Message.MessageType}");

            FirebaseNotificationMessageData firebaseNotificationMessageData = new FirebaseNotificationMessageData();

            firebaseNotificationMessageData.MessageId = e.Message.MessageId;
            firebaseNotificationMessageData.Title = e.Message.Data["title"];
            firebaseNotificationMessageData.Body = e.Message.Data["body"];
            firebaseNotificationMessageData.Img = e.Message.Data["img"];
            firebaseNotificationMessageData.MessagedeliveredDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            firebaseNotificationMessageData.MessageTobeDeletedDateTime = DateTime.Now.AddMinutes(4).ToString($"yyyy-MM-ddTHH:mm:ssZ");

            

            firebaseNotificationListWrapper.notifications.Add(firebaseNotificationMessageData);
            string firebaseMessageJson = JsonUtility.ToJson(firebaseNotificationListWrapper, true);

            
            output.text = firebaseMessageJson;

            File.WriteAllText(FirebaseDataFilePath, firebaseMessageJson);

            FetchNotificationsFromFile();   

        }

        /// <summary>
        /// Clear all notifications on button click
        /// </summary>
        public void ClearAllNotifications()
        {
            Debug.Log($"clear all notifications on button click");
            firebaseNotificationListWrapper.notifications = null;
            string firebaseMessageJson = JsonUtility.ToJson(firebaseNotificationListWrapper, true);
            output.text = firebaseMessageJson;
            File.WriteAllText(FirebaseDataFilePath, firebaseMessageJson);

            FetchNotificationsFromFile();
            
            
        }



        void FetchNotificationsFromFile()
        {
            if (!File.Exists(FirebaseDataFilePath))
                return;

            // read the file content
            string fileContent = File.ReadAllText(FirebaseDataFilePath);
            if (!string.IsNullOrEmpty(fileContent))
            {
                firebaseNotificationListWrapper = JsonUtility.FromJson<FirebaseNotificationListWrapper>(fileContent);

                // List<FirebaseNotificationMessageData> messageDataTobeDisplayed = firebaseNotificationListWrapper.notifications.Select(i => i).ToList();

                if (notificationPanelParentTransform.childCount > 0)
                {
                    /*// notificationPanelParentTransform.Select(i => i)
                    // Hide and reuse existing panels
                    foreach (Transform child in notificationPanelParentTransform)
                    {
                        child.gameObject.SetActive(false);
                    }*/

                    notificationPanelParentTransform.Cast<Transform>().ToList().ForEach(child => { child.gameObject.SetActive(false); });
                    
                }

                // Instantiatea the notification panel if the count is greater than the child count.
                if (firebaseNotificationListWrapper.notifications.Count > notificationPanelParentTransform.childCount)
                {
                    var diff = firebaseNotificationListWrapper.notifications.Count - notificationPanelParentTransform.childCount;
                    for (int i = 0; i < diff; i++)
                    {
                        Instantiate(notificationPanel, notificationPanelParentTransform);
                    }
                }

                for (int i = 0; i < firebaseNotificationListWrapper.notifications.Count; i++)
                {
                    var notificationPanelWeb3_0 = notificationPanelParentTransform.GetChild(i).GetComponent<NotificationPanelWeb3_0>();
                    notificationPanelWeb3_0.UpdateNotificationPanelContent(firebaseNotificationListWrapper.notifications[i]);

                    // Calculate difference in timings
                    var differenceInTimings = DateTime.Now - DateTime.Parse(firebaseNotificationListWrapper.notifications[i].MessagedeliveredDateTime);

                    notificationPanelWeb3_0.gameObject.SetActive(true);

                    Debug.Log($"Message Id : {firebaseNotificationListWrapper.notifications[i].MessageId} \t Message Title {firebaseNotificationListWrapper.notifications[i].Title} \t Message Body {firebaseNotificationListWrapper.notifications[i].Body} \t Message delivery difference {differenceInTimings}");
                }
            }
            else
            {
                output.text = "<color=yellow>File is empty</color>";
                firebaseNotificationListWrapper = new FirebaseNotificationListWrapper();
            }
        }

        void InstantiateNotificationPanel()
        {

            PlayerPrefs.GetString("FirebaseNotificationPanelInstantiated");

            if (!_notificationPanelInstantiated)
            {
                for(int i = 0; i<5; i++)
                {
                    var panel = Instantiate(notificationPanel, notificationPanelParentTransform);
                    notificationPool.Add(panel);
                    panel.gameObject.SetActive(false);
                }
                
                _notificationPanelInstantiated = true;
            }

            Debug.Log($"<color=green>Notification panel instantiated...</color>");
            
        }
    }
}

