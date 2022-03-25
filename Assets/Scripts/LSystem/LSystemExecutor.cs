using UnityEngine;
using System.Collections.Generic;

public class LSystemExecutor : MonoBehaviour
{
    public string rule;
    public string axiom;
    public float angle = 22.5f;
    public int derivations = 3;
    /// <summary>
    /// number of triangles per length of branch, no visual effect
    /// </summary>
    public int segmentAxialSamples = 3;
    /// <summary>
    /// number of triangles around branch, no visual effect as long as there are enough
    /// </summary>
    public int segmentRadialSamples = 3;
    /// <summary>
    /// Thickness of branches
    /// </summary>
    public float segmentWidth = 0.5f;
    /// <summary>
    /// Length of branches
    /// </summary>
    public float segmentHeight = 2.0f;
    /// <summary>
    /// Size of leaves
    /// </summary>
    public float leafSize;
    /// <summary>
    /// Number of leaves along one branch
    /// </summary>
    public int leafAxialDensity = 1;
    /// <summary>
    /// Number of leaves around one branch
    /// </summary>
    public int leafRadialDensity = 1;
    /// <summary>
    /// Are leaves rendered?
    /// </summary>
    public bool useFoliage;
    /// <summary>
    /// Branches get narrower further down the hierarchy
    /// </summary>
    public bool narrowBranches = true;
    public Material trunkMaterial;
    public Material leafMaterial;
    public bool useColliders = false;


    public LSystemInterpreter Interpreter;
    public TMPro.TextMeshProUGUI DebugInfo;


    private int currentDerivations;
    private string moduleString;

    void Start()
    {
        DeriveFromScratch();
        BuildTree();
    }


    public void Rebuild()
    {
        Debug.Log("Rebuild tree");
        DeriveFromScratch();
        DeleteTree();
        BuildTree();
    }

    public void Derive()
    {
        DeriveOneStep();
        DeleteTree();
        BuildTree();
    }

    private void DeriveOneStep()
    {

        currentDerivations += 1;
        var productions = LSystemParser.CreateRules(rule);
        moduleString = LSystemDeriver.DeriveOneStep(moduleString, angle, productions);

        DeleteTree();
    }

    private void DeriveFromScratch()
    {
        currentDerivations = derivations;
        var productions = LSystemParser.CreateRules(rule);
        moduleString = LSystemDeriver.Derive(axiom, angle, derivations, productions);
    }

    private void DeleteTree()
    {
        for(int i = transform.childCount-1; i>=0; i--)
        {
            var child = transform.GetChild(i);
            var container = child.GetComponent<PartContainer>();
            if (container != null)
            {
                container.DisableAllItems();
            }
            else
            {

                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    private void BuildTree()
    {
        GameObject leaves, trunk;
        Interpreter.Interpret(
            segmentAxialSamples,
            segmentRadialSamples,
            segmentWidth,
            segmentHeight,
            leafSize,
            leafAxialDensity,
            leafRadialDensity,
            useFoliage,
            narrowBranches,
            leafMaterial,
            trunkMaterial,
            angle,
            moduleString,
            out leaves,
            out trunk);

        leaves.transform.parent = transform;
        leaves.transform.localPosition = Vector3.zero;
        trunk.transform.parent = transform;
        trunk.transform.localPosition = Vector3.zero;

        if(useColliders)
        {
            UpdateColliderBounds(trunk);
        }

        DebugInfo.text = $"Rule: {rule}\n" +
            $"Axiom: {axiom}\n" +
            $"Derivations: {currentDerivations}\n" +
            $"Angle: {rule}\n" +
            $"Segment Axial Samples: {segmentAxialSamples}\n" +
            $"Rule: {segmentRadialSamples}\n" +
             $"Segment Axial Samples: {segmentAxialSamples}\n" +
             $"Segment Width/Height: {segmentWidth} {segmentHeight}\n" +
             $"Leaf Size: {leafSize}\n" +
             $"Leaf Axial Density: {leafAxialDensity}\n";
    }



    void UpdateColliderBounds(GameObject trunk)
    {
        // Calculate AABB
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        for (int i = 0; i < trunk.transform.childCount; i++)
        {
            Transform chunk = trunk.transform.GetChild(i);
            min.x = Mathf.Min(min.x, chunk.GetComponent<Renderer>().bounds.min.x);
            min.y = Mathf.Min(min.y, chunk.GetComponent<Renderer>().bounds.min.y);
            min.z = Mathf.Min(min.z, chunk.GetComponent<Renderer>().bounds.min.z);
            max.x = Mathf.Max(max.x, chunk.GetComponent<Renderer>().bounds.max.x);
            max.y = Mathf.Max(max.y, chunk.GetComponent<Renderer>().bounds.max.y);
            max.z = Mathf.Max(max.z, chunk.GetComponent<Renderer>().bounds.max.z);
        }

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);

        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        if (collider == null)
            return;
        collider.center = bounds.center - transform.position;
        collider.size = 2 * bounds.extents;
    }

}
