using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDataMapping : MonoBehaviour
{
    public TextMesh label;
    public LSystemExecutor lsystem;
    public LSystemInterpreter lsystemRenderer;

    void Awake()
    {
        lsystem = transform.GetComponent<LSystemExecutor>();
        lsystemRenderer = transform.GetComponent<LSystemInterpreter>();

        lsystem.ImmediateRendering = true;
        InitLabel();
    }

    private int count = 0;
    public int Count
    {
        get { return count; }
        set
        {
            Debug.Log("New Count " + value);
            lsystemRenderer.InterpretContinue(lsystem.ModuleString, value);
            count = value;
        }
    }

    private float strength = 0;
    /// <summary>
    /// Sets the leaf size
    /// </summary>
    public float Strength
    {
        get { return strength; }
        set {
            Debug.Log("new strength " + value);
            strength = value;
            lsystemRenderer.leafSize = value;
        }
    }

    private float size = 0;
    /// <summary>
    /// Defines overall size of tree by changing the length of branches
    /// </summary>
    public float Size
    {
        get { return size; }
        set
        {
            Debug.Log("new size " + value);
            size = value;
            lsystemRenderer.segmentHeight = value;
        }
    }


    public void InitLabel () {
        if (label == null) {
            return;
        }

        label.text = gameObject.name;
        label.transform.position += new Vector3 (0, CalculateTreeBounds ().y, 0);
    }

    public Vector3 CalculateTreeBounds () {
        return new Vector3 (0,0,0);
    }
}
