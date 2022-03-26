using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GitDataExampleMapping : MonoBehaviour
{
    public GitDataScriptableObject GitData;
    public Mapping Mapping;
    

    void Start()
    {
        StartCoroutine(UpdateData());
    }

    private IEnumerator UpdateData()
    {
        //wait a little bit to make sure everything is initialized
        yield return new WaitForSeconds(0.1f);
        Mapping.Size = 0.01f * GitData.CodeLineCount / (float)GitData.CommitCount;
        Mapping.Strength = 0.2f * Mapping.Size;
        Mapping.Count = 10 * GitData.CommitCount;
    }

}
