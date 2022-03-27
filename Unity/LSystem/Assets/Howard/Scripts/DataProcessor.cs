using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataProcessor : MonoBehaviour
{
    //historical comparison number
    public int maxCommits = 30;
    //historical comparison number
    public int maxCodeLineCount = 13000;
    //max team size limit
    public int maxContributorCount = 5;
    //commits should usually compose more than 3 words.
    public int minCommitMessageLength = 10;


    private static DataProcessor _instance;
    public static DataProcessor instance {get {return _instance;}}

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        }
    }

    // Calculates the ratio of total commits relative to the largest commit observed.
    public float NormalizeCommitWeight (int commitNum) {

        return Mathf.Clamp ((float) commitNum / (float) maxCommits, 0f, 1.0f);
    }

    // Calculates the ratio of total code relative to the longest lines of code observed.
    public float NormalizeCodeWeight (int codeLineNum) {
        
        return Mathf.Clamp ((float) codeLineNum / (float) maxCodeLineCount, 0f, 1.0f);
    }

    // Calculates the diversity of contributors against the team size limit.
    public float NormalizeContributors (List <CommitData> data) {
        List <string> contributors = new List<string> ();

        foreach (CommitData d in data) {

            if (!contributors.Contains (d.author)) {
                contributors.Add (d.author);
            }
        }

        return Mathf.Clamp ((float) contributors.Count / (float) maxContributorCount, 0f, 1.0f);
    }

    // Calculates the proportion of total commit messages that are considered "good"
    public float NormalizeCommitMessages (List <CommitData> data) {
        
        int validMessageCount = 0;

        foreach (CommitData d in data) {
            
            if (d.message.Length >= minCommitMessageLength) {
                validMessageCount++;
            }
        }

        return Mathf.Clamp ((float) validMessageCount / (float) data.Count, 0f, 1.0f);        
    }


}
