using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualTree : MonoBehaviour
{
    public string StartTree;
    public float Angle;
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


        GameObject leaves, trunk;
        Interpreter.Interpret(
            null, null,
            Angle,
            TreeDefinition,
            out leaves,
            out trunk);

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
