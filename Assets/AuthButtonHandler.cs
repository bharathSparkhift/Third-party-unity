using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthButtonHandler : MonoBehaviour
{


    GoogleLoginHandler googleLoginHandler;

    private void Awake()
    {
        googleLoginHandler = new GoogleLoginHandler();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoogleLoginButton()
    {
        googleLoginHandler.Sigin();
    }

    public void GoogleLogoutButton()
    {
        googleLoginHandler.OnSignOut();
    }
}
