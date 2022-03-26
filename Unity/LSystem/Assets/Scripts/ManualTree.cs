using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Tree is manually defined, not by an L-system.
/// It uses the same string definition as the L-system.
/// </summary>
public class ManualTree : MonoBehaviour
{
    public string StartTree;
    public LSystemInterpreter Interpreter;


    private string treeDefinition;
    public string TreeDefinition;


    void Start()
    {
        TreeDefinition = StartTree;
    }


    public void AddBranch()
    {
        TreeDefinition += "F";
        UpdateRendering();
    }

    public void PushStack()
    {
        TreeDefinition += "[";

    }
    public void PopStack()
    {
        TreeDefinition += "]";
        UpdateRendering();
    }

    public void RotateX(bool plus = true)
    {
        TreeDefinition += plus ? "&" : "^";
    }
    public void RotateY(bool plus = true)
    {
        TreeDefinition += plus ? "\\" : "/";
    }
    public void RotateZ(bool plus = true)
    {
        TreeDefinition += plus ? "+" : "-";
    }

    public void AddString(string module)
    {
        TreeDefinition += module;
        UpdateRendering();
    }


    private void UpdateRendering()
    {
        DeleteTree();


        Interpreter.Interpret( TreeDefinition);

    }

    private void DeleteTree()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
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
}
