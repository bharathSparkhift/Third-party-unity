using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeepLinkeHandler : MonoBehaviour
{


    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        Application.deepLinkActivated += OnDeepLinkButtonClicked;
    }

    public void OnDeepLinkButtonClicked(string url)
    {
        // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
        // deeplinkURL = url;

        // Decode the URL to determine action. 
        // In this example, the application expects a link formatted like this:
        // unitydl://mylink?scene1
        // http://thirdparty?Thirdparty
        string sceneName = url.Split('?')[1];
        bool validScene;
        switch (sceneName)
        {
            case "scene1":
                validScene = true;
                break;
            case "scene2":
                validScene = true;
                break;
            default:
                validScene = false;
                break;
        }
        if (validScene) SceneManager.LoadScene(sceneName);

        Debug.Log($"{nameof(OnDeepLinkButtonClicked)}");
    }
}
