
using UnityEngine;

[CreateAssetMenu(fileName = "GitData", menuName = "Data/GitData", order = 1)]
public class GitDataScriptableObject : ScriptableObject
{
    public string sourceName;

    public int CommitCount;
    public int CodeLineCount;

}
