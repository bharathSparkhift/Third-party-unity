using Google;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Profiling;

public class GoogleLoginHandler 
{
    public void Sigin()
    {
        GoogleSignInConfiguration config = new GoogleSignInConfiguration()
        {
            RequestEmail = true,
            RequestIdToken = true,
            RequestProfile = true,
            RequestAuthCode = true,
            UseGameSignIn = false,
            WebClientId = "435353700219-lf0et9hehgvt12vpmdheaf1t5i2vcq9i.apps.googleusercontent.com"
        };

        GoogleSignIn.Configuration = config;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
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
            
            Debug.LogFormat("user.DisplayName: {0}, user.UserId: {1}, user.Email: {2}, user.ImageUrl: {3}, user.AuthCode: {4}, user.IdToken: {5}", user.DisplayName, user.UserId, user.Email, user.ImageUrl, user.AuthCode, user.IdToken);
            
        }
    }

    public void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();

    }
}
