using Firebase;
using Firebase.Extensions;
using Firebase.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    [SerializeField] private TMP_InputField firebaseTokenOnReceivedText;
    // [SerializeField] private TMP_InputField firebaseMessageOnReceivedText;

    private FirebaseApp app;


    // Start is called before the first frame update
    void Start()
    {

        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log($"{e.Message}");
        // firebaseMessageOnReceivedText.text = e.Message.ToString();
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs e)
    {
        Debug.Log($"{e.Token}");
        firebaseTokenOnReceivedText.text = e.Token;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
