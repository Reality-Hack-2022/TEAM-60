using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Mapping : MonoBehaviour
{
    public GameObject TreePrefab;

    private LSystemExecutor lsystem;
    private LSystemInterpreter lsystemRenderer;
    private GameObject tree;

    void Awake()
    {
        tree = Instantiate(TreePrefab, transform);
        lsystem = tree.GetComponent<LSystemExecutor>();
        lsystemRenderer = tree.GetComponent<LSystemInterpreter>();

        lsystem.ImmediateRendering = false;
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
}
