using Firebase.Extensions;
using Google;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GoogleSignInManager : MonoBehaviour
{
    [SerializeField] string google_webclient_ID = "";

    [SerializeField] TMP_Text googleSignInText;
    [Space(20)]
    [SerializeField] TMP_InputField id;
    [SerializeField] TMP_InputField token;
    [SerializeField] TMP_InputField name_;
    [SerializeField] TMP_InputField mail;
    [SerializeField] TMP_InputField profileUrl;
    [SerializeField] RawImage profileImage;
    [SerializeField] TMP_InputField authCode;
    [SerializeField] Button googleSiginInButton;
    [SerializeField] Button googleSignOutButton;

#if UNITY_STANDALONE_WIN
    private string redirectUri = "http://localhost:8080";
    private string authorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";
    private string tokenEndpoint = "https://oauth2.googleapis.com/token";
    private string scope = "profile email";

#endif


    private void Awake()
    {
        
    }


    private void Start()
    {
            
    }

    public void OnClick_GoogleLoginIn()
    {
#if UNITY_ANDROID
        GoogleSignInConfiguration config = new GoogleSignInConfiguration()
        {
            RequestEmail = true,
            RequestIdToken = true,
            RequestProfile = true,
            RequestAuthCode = true,
            UseGameSignIn = false,
            WebClientId = google_webclient_ID
        };

        GoogleSignIn.Configuration = config;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationFinished);

#endif
#if UNITY_STANDALONE_WIN
        string authorizationRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}",
                authorizationEndpoint, Uri.EscapeDataString(scope), Uri.EscapeDataString(redirectUri), google_webclient_ID);

        Application.OpenURL(authorizationRequest);
        StartLocalServer();

#endif
    }

private void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
#if UNITY_ANDROID
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Canceled");
        }
        else
        {
            GoogleSignInUser user = task.Result;
            id.text = user.UserId.ToString();
            token.text = user.IdToken.ToString();
            name_.text = user.DisplayName.ToString();
            mail.text = user.Email.ToString();
            profileUrl.text = user.ImageUrl.ToString();

            StartCoroutine(GetTexture());

            authCode.text = user.AuthCode.ToString();   
            Debug.LogFormat("user.DisplayName: {0}, user.UserId: {1}, user.Email: {2}, user.ImageUrl: {3}, user.AuthCode: {4}, user.IdToken: {5}", user.DisplayName, user.UserId, user.Email, user.ImageUrl, user.AuthCode, user.IdToken);
            googleSignInText.text = "Google sign in successfull";
            googleSiginInButton.gameObject.SetActive(false);
            googleSignOutButton.gameObject.SetActive(true);
        }
#endif

    }

    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(profileUrl.text);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            profileImage.texture = myTexture;
        }
    }


    public void OnSignOut()
    {
#if UNITY_ANDROID
        GoogleSignIn.DefaultInstance.SignOut();
        googleSignInText.text = "Google sign out successfull";
        id.text = string.Empty;
        name_.text = string.Empty;
        mail.text = string.Empty;
        profileUrl.text = string.Empty;
        token.text = string.Empty;   
        profileImage.texture = null;
        authCode.text = string.Empty;
        googleSiginInButton.gameObject.SetActive(true);
        googleSignOutButton.gameObject.SetActive(false);
#endif
    }

    private async void StartLocalServer()
    {
#if UNITY_STANDALONE_WIN

        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(redirectUri + "/");
        listener.Start();

        var context = await listener.GetContextAsync();
        var code = context.Request.QueryString.Get("code");

        // await ExchangeCodeForTokens(code);

        listener.Stop();
#endif

    }
#if UNITY_STANDALONE_WIN
    /*private async Task ExchangeCodeForTokens(string code)
    {

        WWWForm form = new WWWForm();
        form.AddField("code", code);
        form.AddField("redirect_uri", redirectUri);
        form.AddField("client_id", google_webclient_ID);
        form.AddField("client_secret", "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");
        form.AddField("grant_type", "authorization_code");

        using (UnityWebRequest www = UnityWebRequest.Post(tokenEndpoint, form))
        {
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError(www.error);
            }
        }
    }*/
#endif


}
