using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackTheHack;

public class EditorSimulation : MonoBehaviour
{
    public int currCommitIndex = 0;

    public GithubRequester requester;
    
    private void Update() {
        
        if (Input.GetKeyDown (KeyCode.RightArrow) && requester != null) {
            HTH_Publisher.Instance.Call_HTH_GitHubCommitResponse (GetCommitDataFromIndex (currCommitIndex));
            Debug.Log ("Curr Commit Index: " + currCommitIndex);
            currCommitIndex++;
        }

        if (Input.GetKeyDown (KeyCode.LeftArrow) && requester != null) {
            HTH_Publisher.Instance.Call_HTH_GitHubCommitResponse (GetCommitDataFromIndex (currCommitIndex));
            Debug.Log ("Curr Commit Index: " + currCommitIndex);
            currCommitIndex--;
        }
    }


    public Dictionary <string, List <CommitData>> GetCommitDataFromIndex (int index) {
        Dictionary<string, List<CommitData>> dict = requester.githubDataDict;
        Dictionary<string, List<CommitData>> newDict = new Dictionary<string, List<CommitData>> ();

        foreach (string key in dict.Keys) {

            List <CommitData> data = dict [key];

            // Check if the index is greater than the current commit count
            if (!(index > data.Count - 1)) {
                List <CommitData> newData = new List<CommitData> ();
                int localIndex = 0;

                // If it isn't, bisect the commitdata list up until the index and add it to the new dictionary with repo key
                while (localIndex <= index) {

                    newData.Add (data [localIndex]);
                    localIndex++;
                }

                newDict.Add (key, newData);
            }
        }
        return newDict;
    }


}
