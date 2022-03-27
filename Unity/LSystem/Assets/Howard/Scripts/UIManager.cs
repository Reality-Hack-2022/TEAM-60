using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager instance { get {return _instance;}}
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI commitIndexText;
    public TextMeshProUGUI requestErrorText;
    public TMP_InputField usernameInputField;
    public TMP_InputField requestTokenInputField;
    public RectTransform requestButton;
    public EditorSimulation editorSim;
    public GithubRequester requester;


    private void Awake() {
        if (_instance == null) {
            _instance = this;
        }
        else {
            Destroy (this);
        }
    }

    public void OnCommitButtonClicked (bool next) {
        if (editorSim) {
            editorSim.UpdateCommitIndex (next);

            UpdateCommitIndexText (editorSim.currCommitIndex);
        }
    }

    public void UpdateCommitIndexText (int index) {
        if (commitIndexText) {
            commitIndexText.text = "Commit #: " + index.ToString();
        }
    }

    public void UpdateTitleText (string text) {
        if (titleText) {
            titleText.text = text;
        }
    }

    public void UpdateRequestResultText (string text) {
        if (requestErrorText) {
            requestErrorText.text = text;
        }
    }

    
    public void OnRequestButtonClicked () {
        if (!requester && !requestTokenInputField && !usernameInputField) {
            return;
        }

        if (usernameInputField.text.Length <= 0) {
            UpdateRequestResultText ("Please add github username");
            return;
        }

        if (requestTokenInputField.text.Length <= 0) {
            UpdateRequestResultText ("Please add personal token");
            return;
        }

        requester.StartLoadGithubData (usernameInputField.text, requestTokenInputField.text);
    }

    public void ToggleLoginInfo (bool enable) {
        requestErrorText.gameObject.SetActive (enable);
        usernameInputField.gameObject.SetActive (enable);
        requestTokenInputField.gameObject.SetActive (enable);
        requestButton.gameObject.SetActive (enable);
    }
}
