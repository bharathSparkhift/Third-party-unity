using System.Collections.Generic;
using UnityEngine;

// Other needed dependencies
using Facebook.Unity;
using TMPro;
using System;
using Unity.Services.Authentication;
using static Authentication.FbLoginHandler;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Threading.Tasks;

public class FacebookExampleScript : MonoBehaviour
{
    public string Token;
    public string Error;

    [SerializeField] private Transform profilePicImage;
    [SerializeField] private TMP_Text id;
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text email;

    public TMP_InputField TokenInputField;
    public TMP_Text ErrorTextField;

    /// <summary>
    /// Facebook dictionary result
    /// </summary>
    Dictionary<string, string> _fbResult = new Dictionary<string, string>();

    /// <summary>
    /// Query fields
    /// </summary>
    string _queryFields = $"/me?fields=id,name,email"; 
    string _picQueryField = "/me?fields=picture";
    string _mailQueryField = "/me?fields=email";

    RawImage _rawImage;

    // Awake function from Unity's MonoBehavior
    void Awake()
    {
        _rawImage = profilePicImage.GetComponent<RawImage>();

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void Login()
    {
        // Define the permissions
        var perms = new List<string>() { "openid", "public_profile", "email" };

        FB.LogInWithReadPermissions(perms, result =>
        {
            if (FB.IsLoggedIn)
            {
                Token = AccessToken.CurrentAccessToken.TokenString;
                TokenInputField.text = Token;
                Debug.Log($"Facebook Login token: {Token}");
                if (!String.IsNullOrEmpty(Token))
                {
                    // Task linkWithFacebookAsync = LinkWithFacebookAsync(aToken.TokenString);
                    _ = SignInWithFacebookAsync(Token);
                }
            }
            else
            {
                Error = "User cancelled login";
                ErrorTextField.text = Error;
                Debug.Log("[Facebook Login] User cancelled login");
            }
        });
    }

    /// <summary>
    /// If no Unity Authentication player in your project is associated with the credentials, SignInWithFacebookAsync creates a new player. 
    /// If a Unity Authentication player in your project is associated with the credentials, SignInWithFacebookAsync signs into that player's account. 
    /// This function doesn't consider the cached player, and SignInWithFacebookAsync replaces the cached player.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    async Task SignInWithFacebookAsync(string accessToken)
    {
        Debug.Log($"----- {nameof(SignInWithFacebookAsync)} ----- \n {accessToken}");
        Debug.Log("SignIn is successful..............");
        FB.API(_queryFields, HttpMethod.GET, LoginGraph);
        FB.API(_picQueryField, HttpMethod.GET, ProfilePicGraph);
        FB.API(_mailQueryField, HttpMethod.GET, EmailGraph);
        SignInOptions signInOptions = new SignInOptions();
        await AuthenticationService.Instance.SignInWithFacebookAsync(accessToken);
        


    }

    /// <summary>
    /// Login Graph
    /// </summary>
    /// <param name="result"></param>
    private void LoginGraph(IGraphResult result)
    {
        Debug.Log($"---------------------{nameof(LoginGraph)}---------------------------------");
        /*Debug.Log(result.ResultDictionary["id"]);
        Debug.Log(result.ResultDictionary["name"]);*/
        Debug.Log($"Raw result {result.RawResult}");
        id.text = result.ResultDictionary["id"].ToString();
        name.text = result.ResultDictionary["name"].ToString();
        // email.text = result.ResultDictionary["email"].ToString();
        
        //Debug.Log($"{_fbResult["name"]}");
    }

    void ProfilePicGraph(IGraphResult result)
    {
        var pictureData = result.ResultDictionary["picture"] as IDictionary<string, object>;
        var picurl = ((IDictionary<string, object>)pictureData["data"])["url"].ToString();
        Debug.Log($"Picture URL {picurl}");

        if (result.Texture != null)
        {
            StartCoroutine(LoadProfilePicture(picurl));
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    /// <summary>
    /// Load the profile picture from the URL
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator LoadProfilePicture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.responseCode == 200)
        {
            try
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                _rawImage.texture = myTexture;
                Debug.Log($"{nameof(LoadProfilePicture)} Texture {myTexture}");
            }
            catch
            {
                Debug.LogError("Failed to download the profile pic from the URL....");
            }
        }
        else
        {
            Debug.LogError($"Failed to load the URL. Response code {www.responseCode}");
        }

        www.Dispose();
    }
    void EmailGraph(IGraphResult result)
    {
        //var email = result.ResultDictionary["email"];
        //Debug.Log($"{nameof(EmailGraph)}: Email {email}");
    }
}