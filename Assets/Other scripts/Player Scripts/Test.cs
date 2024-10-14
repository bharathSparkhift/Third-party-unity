using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Check Prime Number")]
    public void CheckForPrimeNumber()
    {
        int n = 4;
        if(n < 2)
        {
            Debug.Log("Not a prime number");
        }
        
        for (int i = 2; i <= Mathf.Sqrt(n); i++) {
            if (1 % 2 == 0)
                Debug.Log("Not a prime number");
        }
        Debug.Log("Is prime number");
    }
}
