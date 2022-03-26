using UnityEngine;
using System;
using System.Collections.Generic;

public class LSystemInterpreter : MonoBehaviour
{
    public struct Turtle
    {
        public Quaternion direction;
        public Vector3 position;
        public Vector3 step;

        public Turtle(Turtle other)
        {
            this.direction = other.direction;
            this.position = other.position;
            this.step = other.step;
        }

        public Turtle(Quaternion direction, Vector3 position, Vector3 step)
        {
            this.direction = direction;
            this.position = position;
            this.step = step;
        }

        public void Forward()
        {
            position += direction * step;
        }

        public void RotateX(float angle)
        {
            direction *= Quaternion.Euler(angle, 0, 0);
        }

        public void RotateY(float angle)
        {
            direction *= Quaternion.Euler(0, angle, 0);
        }

        public void RotateZ(float angle)
        {
            direction *= Quaternion.Euler(0, 0, angle);
        }

    }

    public float angle = 22.5f;
    /// <summary>
    /// number of triangles per length of branch, no visual effect
    /// </summary>
    [HideInInspector]
    public int segmentAxisSamples = 3;
    /// <summary>
    /// number of triangles around branch, no visual effect as long as there are enough
    /// </summary>
    [HideInInspector]
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

    public PartContainer LeafContainer;
    public PartContainer BranchContainer;

    Stack<Turtle> stack = null;
    Turtle current;
    int currentIdx = 0;

    public void Start()
    {
        
    }


    private void CreateSegment(Turtle turtle, int nestingLevel)
    {
        var branch = BranchContainer.GetInstance();
        branch.transform.localPosition = turtle.position;
        branch.transform.localRotation = turtle.direction;

        float thickness = (narrowBranches) ? segmentWidth * (0.5f / (nestingLevel + 1)) : segmentWidth * 0.5f;

        branch.transform.localScale = new Vector3(thickness, segmentHeight, thickness);


    }

    private void CreateSegmentProcedural(
        Material trunkMaterial,
        Turtle turtle,
        int nestingLevel,
        ref Mesh currentMesh,
        ref int chunkCount,
        GameObject trunk,
        Dictionary<int, Mesh> segmentsCache)
    {
        Vector3[] newVertices;
        Vector3[] newNormals;
        Vector2[] newUVs;
        int[] newIndices;

        Mesh segment;
        if (segmentsCache.ContainsKey(nestingLevel))
            segment = segmentsCache[nestingLevel];
        else
        {
            float thickness = (narrowBranches) ? segmentWidth * (0.5f / (nestingLevel + 1)) : segmentWidth * 0.5f;
            segment = ProceduralMeshes.CreateCylinder(segmentAxisSamples, segmentRadialSamples, thickness, segmentHeight);
            segmentsCache[nestingLevel] = segment;
        }

        newVertices = segment.vertices;
        newNormals = segment.normals;
        newUVs = segment.uv;
        newIndices = segment.triangles;

        if (currentMesh.vertices.Length + newVertices.Length > 65000)
        {
            CreateNewChunk(currentMesh, ref chunkCount, trunkMaterial, trunk);
            currentMesh = new Mesh();
        }

        int numVertices = currentMesh.vertices.Length + newVertices.Length;
        int numTriangles = currentMesh.triangles.Length + newIndices.Length;

        Vector3[] vertices = new Vector3[numVertices];
        Vector3[] normals = new Vector3[numVertices];
        int[] indices = new int[numTriangles];
        Vector2[] uvs = new Vector2[numVertices];

        Array.Copy(currentMesh.vertices, 0, vertices, 0, currentMesh.vertices.Length);
        Array.Copy(currentMesh.normals, 0, normals, 0, currentMesh.normals.Length);
        Array.Copy(currentMesh.triangles, 0, indices, 0, currentMesh.triangles.Length);
        Array.Copy(currentMesh.uv, 0, uvs, 0, currentMesh.uv.Length);

        int offset = currentMesh.vertices.Length;
        for (int i = 0; i < newVertices.Length; i++)
            vertices[offset + i] = turtle.position + (turtle.direction * newVertices[i]);

        int trianglesOffset = currentMesh.vertices.Length;
        offset = currentMesh.triangles.Length;
        for (int i = 0; i < newIndices.Length; i++)
            indices[offset + i] = (trianglesOffset + newIndices[i]);

        Array.Copy(newNormals, 0, normals, currentMesh.normals.Length, newNormals.Length);
        Array.Copy(newUVs, 0, uvs, currentMesh.uv.Length, newUVs.Length);

        currentMesh.vertices = vertices;
        currentMesh.normals = normals;
        currentMesh.triangles = indices;
        currentMesh.uv = uvs;

        currentMesh.Optimize();
    }

    private void AddFoliageAt(
        Turtle turtle)
    {
        float xAngleStep = -70 / (float)leafAxialDensity,
            xAngle = xAngleStep * (leafAxialDensity - 1) - 20,
            yAngle = 0,
            yAngleStep = 360 / (float)leafRadialDensity,
            y = 0,
            yStep = -segmentHeight / (float)leafAxialDensity;
        for (int i = 0; i < leafAxialDensity; i++, xAngle -= xAngleStep, y -= yStep)
        {
            for (int j = 0; j < leafRadialDensity; j++, yAngle += yAngleStep)
            {

                var leaf = LeafContainer.GetInstance();
                leaf.transform.localPosition = Vector3.zero;
                leaf.transform.localRotation = turtle.direction;

                //GameObject leaf = (GameObject)GameObject.Instantiate(leafBillboard, Vector3.zero, turtle.direction);
                //leaf.transform.parent = leaves.transform;
                leaf.transform.localPosition = turtle.position - (turtle.direction * new Vector3(0, y, 0));
                leaf.transform.Rotate(new Vector3(xAngle, yAngle, 0));
                leaf.transform.localScale = new Vector3(leafSize, leafSize, leafSize);
            }
        }
    }

    private void CreateNewChunk(Mesh mesh, ref int count, Material trunkMaterial, GameObject trunk)
    {
        GameObject chunk = new GameObject("Chunk " + (++count));
        chunk.transform.parent = trunk.transform;
        chunk.transform.localPosition = Vector3.zero;
        chunk.AddComponent<MeshRenderer>().material = trunkMaterial;
        chunk.AddComponent<MeshFilter>().mesh = mesh;
    }

    private static GameObject CreateLeafBillboard(float leafSize, Material leafMaterial)
    {
        GameObject leafBillboard = new GameObject("Leaf");
        leafBillboard.AddComponent<MeshRenderer>().sharedMaterial = leafMaterial;
        leafBillboard.AddComponent<MeshFilter>().sharedMesh = ProceduralMeshes.CreateXZPlane(leafSize, leafSize, 1, 1, new Vector3(-leafSize, 0, leafSize * 0.5f));
        return leafBillboard;
    }


    public void Interpret(string moduleString)
    {
        stack = new Stack<Turtle>();
        current = new Turtle(Quaternion.identity, Vector3.zero, new Vector3(0, segmentHeight, 0));
        currentIdx = 0;
        InterpretContinue(moduleString, moduleString.Length);
    }


    public void InterpretContinue(string moduleString, int count)
    { 
        if(stack == null)
        {
            stack = new Stack<Turtle>();
            current = new Turtle(Quaternion.identity, Vector3.zero, new Vector3(0, segmentHeight, 0));
            currentIdx = 0;
        }


        var endIdx = currentIdx + count;
        if (endIdx > moduleString.Length)
            endIdx = moduleString.Length;

        for (int i = currentIdx; i < endIdx; i++)
        {
            string module = moduleString[i] + "";
            if (module == "F")
            {
                CreateSegment(
                    current,
                    stack.Count);
                current.Forward();
            }
            else if (module == "+")
            {
                current.RotateZ(angle);
            }
            else if (module == "-")
            {
                current.RotateZ(-angle);
            }
            else if (module == "&")
            {
                current.RotateX(angle);
            }
            else if (module == "^")
            {
                current.RotateX(-angle);
            }
            else if (module == "\\")
            {
                current.RotateY(angle);
            }
            else if (module == "/")
            {
                current.RotateY(-angle);
            }
            else if (module == "|")
            {
                current.RotateZ(180);
            }
            else if (module == "[")
            {
                stack.Push(current);
                current = new Turtle(current);
            }
            else if (module == "]")
            {
                if (useFoliage)
                    AddFoliageAt(
                        current);
                current = stack.Pop();
            }
        }

        currentIdx = endIdx;
        //CreateNewChunk(currentMesh, ref chunkCount, trunkMaterial, trunk);
        //GameObject.Destroy(leafBillboard);
    }

}
