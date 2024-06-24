using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeepLinkeHandler : MonoBehaviour
{

    // [SerializeField] Button deepLinkButton;

    private void OnEnable()
    {
        // deepLinkButton.onClick.AddListener( (Application.absoluteURL) = OnDeepLinkButtonClicked );
    }

    private void OnDisable()
    {
        // deepLinkButton.onClick.RemoveAllListeners();
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.deepLinkActivated += OnDeepLinkButtonClicked;
    }

    public void OnDeepLinkButtonClicked(string url)
    {
        if (url.Contains("google.com"))
        {
            Application.OpenURL($"{Application.absoluteURL}");  // https://www.google.com
            Debug.Log($"{nameof(OnDeepLinkButtonClicked)} \t");
        }
    }
}
