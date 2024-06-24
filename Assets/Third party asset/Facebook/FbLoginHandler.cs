using Facebook.Unity;
//using LobbyInGameUi;
//using Facebook.Unity.Example;
// using SocialAuthentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Unity.Services.Authentication;


namespace Authentication
{
    [Serializable]
    class JsonFbParent
    {
        public string id;
        public string last_name;
        public string first_name;
        public string url;
        public JsonFbData picture;

    }

    [Serializable]
    class JsonFbData
    {
        public byte height;
        public bool is_silhouette;
        public string url;
        public byte width;
        public string[] arr;
    }

    /// <summary>
    /// FB Login 
    /// 
    /// Note : Attach this to the Canvas game object in the hierarchy.
    /// </summary>
    public class FbLoginHandler : MonoBehaviour
    {

        /// <summary>
        /// Query Fields Enum
        /// </summary>
        public enum QueryFieldsEnum
        {
            id,
            name,
            picture,
            email
        }

        #region Serialize field
        [SerializeField] private Transform _profilePicImage;
        /// <summary>
        /// Login panel
        /// </summary>
        [SerializeField] private Transform _loginPanel;
        /// <summary>
        /// Lobby panel
        /// </summary>
        [SerializeField] private Transform _lobbyPanel;
        /*/// <summary>
        /// 3D Logo Transform.
        /// </summary>
        [SerializeField] private Transform _3dLogo;*/
        /// <summary>
        /// Profile Name/Nick name
        /// </summary>
        [SerializeField] private TMP_Text _lobbyPanelProfilePicName;
        /*[SerializeField] private CinemachineBlendingHandler _cinemachineBlendingHandler;*/
        #endregion

        #region Private fields
        /// <summary>
        /// Facebook dictionary result
        /// </summary>
        Dictionary<string, string> _fbResult = new Dictionary<string, string>();
        /// <summary>
        /// Query fields
        /// </summary>
        string _queryFields = $"/me?fields={QueryFieldsEnum.id.ToString()},{QueryFieldsEnum.name.ToString()}"; // ,{QueryFieldsEnum.picture.ToString()}
                                                                                                               // string _queryFields = $"https://graph.facebook.com/me?fields={QueryFieldsEnum.id.ToString()},{QueryFieldsEnum.name.ToString()},{QueryFieldsEnum.picture.ToString()}";

        string _picQueryField = "/me?fields=picture";
        string _mailQueryField = "/me?fields=email";

        RawImage _rawImage;
        #endregion

        #region Public fields
        /*public string QueryFields { get { return _queryFields; } private set { value = _queryFields; } }*/
        public Dictionary<string, string> FbResult { get; private set; }

        #endregion

        #region Monobehaviour Callbacks

        private void Awake()
        {
            _rawImage = _profilePicImage.GetComponent<RawImage>();

            // Task signInCachedUserAsync = SignInCachedUserAsync();

            // StartCoroutine(SignInCachedUserAsync);

            // Initialize the Facebook SDK
            if (!FB.IsInitialized)
            {
                FB.Init(() =>
                {
                    if (FB.IsInitialized)
                        Debug.Log("---------------- Facebook SDK initialized ----------------------------- ");
                    else
                        Debug.LogError("----------------------- Failed to initialize the Facebook SDK ----------------------------");
                });
            }
            else
                Debug.Log($"Failed to Initialise the facebook or facebook already initialised.........");
            Debug.Log($"------------------------------------------------{nameof(Start)}-----------------------------------------------------------");
        }

        // Start is called before the first frame update
        void Start()
        {
            FB.Android.RetrieveLoginStatus(LoginStatusCallback);
        }
        #endregion

        #region Private methods

        private void LoginStatusCallback(ILoginStatusResult result)
        {
            Debug.Log($"{nameof(LoginStatusCallback)} ILoginStatusResult {result}");
            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.Log("Error: " + result.Error);
            }
            else if (result.Failed)
            {
                Debug.Log("Failure: Access Token could not be retrieved");
            }
            else
            {
                // Successfully logged user in
                // A popup notification will appear that says "Logged in as <User Name>"
                Debug.Log("Success: " + result.AccessToken.UserId);
            }
        }

        /// <summary>
        /// Login Graph
        /// </summary>
        /// <param name="result"></param>
        private void LoginGraph(IGraphResult result)
        {
            Debug.Log($"---------------------{nameof(LoginGraph)}---------------------------------");
            Debug.Log(result.ResultDictionary["id"]);
            Debug.Log(result.ResultDictionary["name"]);

            _fbResult[QueryFieldsEnum.id.ToString()] = result.ResultDictionary[QueryFieldsEnum.id.ToString()].ToString();
            _fbResult[QueryFieldsEnum.name.ToString()] = result.ResultDictionary[QueryFieldsEnum.name.ToString()].ToString();
            Debug.Log($"{_fbResult["name"]}");




            ToggleLoginPanel(false);
            ToggleLobbyPanel(true);
            
            
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


        void EmailGraph(IGraphResult result)
        {
            var email = result.ResultDictionary["email"];
            Debug.Log($"{nameof(EmailGraph)}: Email {email}");
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

            if(www.responseCode == 200)
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

        /// <summary>
        /// Convert data to JSON format 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="pictureUrl"></param>
        private void ConvertToJson(string id, string name, string pictureUrl)
        {
            JsonFbParent _parent = new JsonFbParent();
            _parent.id = id;
            _parent.first_name = name;
            _parent.last_name = name;
            _parent.url = pictureUrl;

            var picture = new JsonFbData
            {
                height = 50,
                is_silhouette = false,
                url = "https://facebook.com",
                width = 50,
                arr = new string[] { "a", "b", "c" }
            };

            _parent.picture = picture;

            // Converted Json format
            string convertedjson = JsonUtility.ToJson(_parent);

            Debug.Log($"--------------------- JSON UTILITY {convertedjson} --------------------------");

            JsonFbParent out_ = JsonUtility.FromJson<JsonFbParent>(convertedjson);
            Debug.Log($"-------------------First name from JSON   {out_.first_name} ---------------------");
        
        }

        /// <summary>
        /// Enable or disable the login panel 
        /// </summary>
        /// <param name="value"></param>
        private void ToggleLoginPanel(bool value)
        {
            if(_loginPanel == null)
            {
                return;
            }
            Debug.Log($"---------------------------- {nameof(ToggleLoginPanel)} : -------------------------");
            _loginPanel.gameObject.SetActive(value);
        }

        /// <summary>
        /// Enable or disable the lobby panel.
        /// </summary>
        /// <param name="value"></param>
        private void ToggleLobbyPanel(bool value)
        {
            if(_lobbyPanel == null)
            {
                return;
            }
            Debug.Log($"--------------------- {nameof(ToggleLobbyPanel)} : ----{_fbResult[QueryFieldsEnum.name.ToString()]}-----------------");
            _lobbyPanel.gameObject.SetActive(value);
            if(value)
            {
                // JsonFbParent _parent = new JsonFbParent();
                SetProfilePicNameInLobbyPanel(_fbResult[QueryFieldsEnum.name.ToString()]);
            }
        }

        /*void Toggle3DLogo(bool value)
        {
            // _3dLogo.gameObject.SetActive(value);    
        }*/

        /// <summary>
        /// Set the profile pic name in the lobby panel.
        /// </summary>
        private void SetProfilePicNameInLobbyPanel(string name)
        {
            Debug.Log($"---------------- {nameof(SetProfilePicNameInLobbyPanel)} :---------------------- ");
            
            _lobbyPanelProfilePicName.text = name; // _fbResult[QueryFieldsEnum.name.ToString()];

        }

        /// <summary>
        /// Authorisation callback
        /// </summary>
        /// <param name="result"></param>
        public void AuthCallback(ILoginResult result)
        {
            Debug.Log($"{nameof(AuthCallback)} . ");
            if (FB.IsLoggedIn)
            {
                
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                // Print current access token's User ID
                Debug.Log($"FB Logged In {FB.IsLoggedIn} \n User ID {aToken.UserId} \n Token {aToken.TokenString}");

                if (!String.IsNullOrEmpty(aToken.TokenString))
                {
                    // Task linkWithFacebookAsync = LinkWithFacebookAsync(aToken.TokenString);
                    _ = SignInWithFacebookAsync(aToken.TokenString);
                }
            }
            else
            {
                Debug.LogError("Facebook login failed: " + result.Error);
            }
        }

        async Task SignInCachedUserAsync()
        {
            Debug.Log($"----{nameof(SignInCachedUserAsync)}---- \n Session Token Exists {AuthenticationService.Instance.SessionTokenExists}");
            // Check if a cached player already exists by checking if the session token exists
            if (!AuthenticationService.Instance.SessionTokenExists)
            {
                // if not, then do nothing
                return;
            }

            // Sign in Anonymously
            // This call will sign in the cached player.
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Sign in anonymously succeeded!");

                // Shows how to get the playerID
                Debug.Log($"{nameof(SignInCachedUserAsync)} \n PlayerID: {AuthenticationService.Instance.PlayerId}");
            }
            catch (Unity.Services.Authentication.AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (Unity.Services.Core.RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }


        #region Facebook Login using unity authentication services.
        async Task LinkWithFacebookAsync(string accessToken)
        {
            Debug.Log($"----{nameof(LinkWithFacebookAsync)}----");
            try
            {
                await AuthenticationService.Instance.LinkWithFacebookAsync(accessToken);

                Debug.Log("Link is successful.");
                Task signInWithFacebookAsync = SignInWithFacebookAsync(accessToken);
                
            }
            catch (Unity.Services.Authentication.AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
            {
                // Prompt the player with an error message.
                Debug.LogError("This user is already linked with another account. Log in instead.");
            }
            catch (Unity.Services.Authentication.AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.Log($"---{nameof(LinkWithFacebookAsync)}----");
                Debug.LogException(ex);
            }
            catch (Unity.Services.Core.RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.Log($"---{nameof(LinkWithFacebookAsync)}----");
                Debug.LogException(ex);
            }
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
            SignInOptions signInOptions = new SignInOptions();

            //Debug.Log("SignIn is successful..............");
            await AuthenticationService.Instance.SignInWithFacebookAsync(accessToken);
            Debug.Log("SignIn is successful..............");
            FB.API(_queryFields, HttpMethod.GET, LoginGraph);
            FB.API(_picQueryField, HttpMethod.GET, ProfilePicGraph);
            FB.API(_mailQueryField, HttpMethod.GET, EmailGraph);

           
        }
        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Login
        /// 
        /// Note : Attach this to the Login button.
        /// </summary>
        public void Login()
        {
            var perms = new List<string>() { "openid", "public_profile", "email" };
            FB.LogInWithReadPermissions( perms, AuthCallback );
        }


        /// <summary>
        /// Logout 
        /// </summary>
        public void Logout()
        {
            Debug.Log($"{nameof(Logout)} : ");
            // Check if the User is logged In
            if (FB.IsLoggedIn)
            {
                // Logout.
                FB.LogOut();

                Debug.Log($"FB Logged In {FB.IsLoggedIn}");
            }
            
        }
        #endregion
    }

}

