using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
public class LSystemExecutor : MonoBehaviour
{
    public string rule;
    public string axiom;
    public int derivations = 3;
    


    public bool ImmediateRendering = true;

    //public bool useColliders = false;


    public LSystemInterpreter Interpreter;
    public TMPro.TextMeshProUGUI DebugInfo;


    private int currentDerivations;
    private string moduleString;

    void Start()
    {
        DeriveFromScratch();
        if (ImmediateRendering)
        {
            BuildTree();
        }
    }

    public string ModuleString
    {
        get { return moduleString; }
    }


    public void Rebuild()
    {
        Debug.Log("Rebuild tree");
        DeriveFromScratch();
        if (ImmediateRendering)
        {
            DeleteTree();
            BuildTree();
        }
    }

    public void Derive()
    {
        DeriveOneStep();
        if (ImmediateRendering)
        {
            DeleteTree();
            BuildTree();
        }
    }


    private void DeriveOneStep()
    {

        currentDerivations += 1;
        var productions = LSystemParser.CreateRules(rule);
        moduleString = LSystemDeriver.DeriveOneStep(moduleString, productions);

        DeleteTree();
    }

    private void DeriveFromScratch()
    {
        currentDerivations = derivations;
        var productions = LSystemParser.CreateRules(rule);
        moduleString = LSystemDeriver.Derive(axiom, derivations, productions);
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
                if (!transform.GetChild(i).gameObject.CompareTag("SubMesh"))
                {
                    if (!transform.GetChild(i).gameObject.CompareTag("FoodSource"))
                    {
                        if (!transform.GetChild(i).gameObject.CompareTag("Label")) {
                            Destroy(transform.GetChild(i).gameObject);
                        }
                    }
                    
                }
                
            }
        }
    }

    private void BuildTree()
    {
        Interpreter.Interpret(moduleString);

        //if(useColliders)
        //{
        //    UpdateColliderBounds(trunk);
        //}

        if (DebugInfo != null)
        {
            DebugInfo.text = $"Rule: {rule}\n" +
                $"Axiom: {axiom}\n" +
                $"Derivations: {currentDerivations}\n" +
                $"Angle: {rule}\n" +
                $"Rule: {Interpreter.segmentRadialSamples}\n" +
                 $"Segment Axial Samples: {Interpreter.segmentAxisSamples}\n" +
                 $"Segment Width/Height: {Interpreter.segmentWidth} {Interpreter.segmentHeight}\n" +
                 $"Leaf Size: {Interpreter.leafSize}\n" +
                 $"Leaf Axial Density: {Interpreter.leafAxialDensity}\n\n" +
                 $"{moduleString}";
        }
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
