using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public FlyCamera cameraController;
    public EditorSimulation simulator;
    private static PlayerInputController _instance;
    public static PlayerInputController instance {get {return _instance;}}

    // Start is called before the first frame update
    private void Awake() {
        if (_instance == null) {
            _instance = this;
        }

        else {
            Destroy (this);
        }

        cameraController = GetComponent <FlyCamera> ();
        simulator = GetComponent <EditorSimulation> ();
    }

    // Update is called once per frame
    void Update()
    {
        // Quit Application on Escape
        if (Input.GetKeyDown (KeyCode.Escape)) {
            Application.Quit ();
        }  
    }

    public void ToggleCameraController (bool enable) {
        if (cameraController) {
            cameraController.enabled = enable;
        }
    }

    public void ToggleSimulator (bool enable) {
        if (simulator) {
            simulator.enabled = enable;
        }
    }    
}
