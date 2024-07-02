using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnalyticsHandler
{
    
    public void SceneLoadViews(string sceneName, double value)
    {
        FirebaseAnalytics.LogEvent($"{sceneName}","views",value);
        Debug.Log($"<color=blue>{nameof(SceneLoadViews)}</color>");
    }
    
    public void AnalyticsLogin()
    {
        
    }

}

public class FirebaseHandler : MonoBehaviour
{

    public delegate void FirebaseHandlerDelegate();
    public static FirebaseHandlerDelegate OnFirebaseDelegateInvoked;

    AnalyticsHandler analyticsHandler;

    private void Awake()
    {
        OnFirebaseDelegateInvoked += InvokeAnalytics;
    }

    private void OnDestroy()
    {
        OnFirebaseDelegateInvoked -= InvokeAnalytics;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    async void InvokeAnalytics()
    {
        analyticsHandler = new AnalyticsHandler();
        await Task.Delay(1000);
        string value = GetPlayerPrefsValue(SceneManager.GetActiveScene().name);
        double count;
        if (value == null || value == string.Empty)
        {
            count = 0;
        }
        else
        {
            count = Double.Parse(value);
        }
        
        analyticsHandler.SceneLoadViews(SceneManager.GetActiveScene().name, count + 1);
        Debug.Log($"{nameof(FirebaseHandler)} \t {nameof(Start)}");
    }


    /// <summary>
    /// Returns PlayerPrefs Value
    /// </summary>
    /// <param name="keyName"></param>
    /// <returns></returns>
    string GetPlayerPrefsValue(string keyName)
    {
        if (!PlayerPrefs.HasKey(keyName))
        {
            PlayerPrefs.SetString(keyName, string.Empty);
        }
        Debug.Log($"{nameof(GetPlayerPrefsValue)}");
        return PlayerPrefs.GetString(keyName);
        
    }

    
}
