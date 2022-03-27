using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TreeDataMapping : MonoBehaviour
{
    public Camera playerCamera;
    public TextMeshPro label;
    public float labelHeight = 0.1f;
    public float labelForwardOffset = 0.05f;
    public bool labelFaceCamera = false;
    public AudioSource treeGrowSound;

    public GameObject branchObject;
    public GameObject leafObject;
    public HackTheHack.HTH_SubMesh branchSubmesh;
    public HackTheHack.HTH_SubMesh leafSubmesh;
    

    public LSystemExecutor lsystem;
    public LSystemInterpreter lsystemRenderer;


    void Awake()
    {
        lsystem = transform.GetComponent<LSystemExecutor>();
        lsystemRenderer = transform.GetComponent<LSystemInterpreter>();

        lsystem.ImmediateRendering = true;
        // InitLabel ();
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

        Vector3 labelOffset = transform.forward * labelForwardOffset + new Vector3 (0, labelHeight, 0);
        label.transform.position += labelOffset;

        if (treeGrowSound) {
            treeGrowSound.Play();
        }
        // label.transform.position += new Vector3 (0, CalculateTreeBounds ().extents.y, 0);
    }

    public void RebuildSubmeshes () {
        StartCoroutine (StartRebuildSubmeshes (0.5f));
    }

    IEnumerator StartRebuildSubmeshes (float delay) {
        yield return new WaitForSeconds (delay);

        if (branchSubmesh && branchObject) {
            branchSubmesh.SubMeshOptimization (branchObject);
        }

        if (leafSubmesh && leafObject) {
            leafSubmesh.SubMeshOptimization (leafObject);
        }        
    }

    public Bounds CalculateTreeBounds () {
        Bounds bounds = new Bounds (transform.position, Vector3.one);
        Renderer[] renderers = GetComponentsInChildren<Renderer> ();

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate (renderer.bounds);
        }

        Debug.Log ("Bound Y calculated: " + bounds.extents.y);

        return bounds;
    }

    private void Update() {
        if (labelFaceCamera && label != null && playerCamera != null) {
            label.transform.LookAt (playerCamera.transform.position, Vector3.up);
        }
    }


}
