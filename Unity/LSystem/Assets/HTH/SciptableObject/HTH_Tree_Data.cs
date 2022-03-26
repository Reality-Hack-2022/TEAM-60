using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Scriptable Object to hold the placevalues to have some data manipulate at runtime
/// </summary>
[CreateAssetMenu(fileName = "HTH_Tree_Data", menuName = "ScriptableObjects/HTH", order = 1)]
public class HTH_Tree_Data : ScriptableObject
{
    public string TeamName;
    public List<CommitData> GitHubData;
    public List<GameObject> BranchContainerPrefab;
    public List<GameObject> LeafContainerPrefab;
    public string Rule = "F=(1)F[-&^F][^++&F]||F[--&^F][+&F]";
    public string Axiom = "F";
    
    public float Angle = 22.5f;
    public int Derivations = 3;
    public int SegmentAxisSamples = 6;
    public int SegmentRadialSamples = 6;
    public float SegmentWidth = 0.5f;
    public float SegmentHeight = 2f;
    public float LeafSize = 1f;
    public int LeafAxialDensity = 1;
    public int LeafRadialDensity = 1;
    public bool UseFoliage = true;
    public bool NarrowBranches = true;
}
