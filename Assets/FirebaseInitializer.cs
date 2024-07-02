using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    [SerializeField] TMP_Text outPutStatus;
    [SerializeField] private TMP_InputField firebaseTokenOnReceivedText;


    private void Awake()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        outPutStatus.text = "output\n";
    }

    private void OnDestroy()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
    }

    // Start is called before the first frame update
    void Start()
    {

        
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            // var dependencyStatus = 
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                InitializeAnalytics();
                FirebaseHandler.OnFirebaseDelegateInvoked?.Invoke();  
                outPutStatus.text += $"<color=green>{task.Result}</color>";
            }
            else
            {

                outPutStatus.text += $"<color=red>{task.Result}</color>";
            }
        });
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        try
        {
            Debug.Log($"{e.Message}");
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        

    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs e)
    {
        try
        {
            Debug.Log($"{e.Token}");
            firebaseTokenOnReceivedText.text = e.Token;
        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }
        
    }

    void InitializeAnalytics()
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        outPutStatus.text += $"<color=green>Firebase analytics initialized</color>";
    }

   
}
