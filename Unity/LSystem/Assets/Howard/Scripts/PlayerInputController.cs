using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Quit Application on Escape
        if (Input.GetKeyDown (KeyCode.Escape)) {
            Application.Quit ();
        }  
    }
}
