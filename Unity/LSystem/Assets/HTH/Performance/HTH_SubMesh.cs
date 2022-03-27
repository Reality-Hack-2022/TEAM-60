using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
namespace HackTheHack
{
    public class HTH_SubMesh : MonoBehaviour
    {
        public GameObject SubMeshObject;
        public Transform Parent;
        private Vector3 oldLocation;
        private Quaternion oldRotation;
        private Vector3 oldScale;
        public int ChildNum;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (SubMeshObject == null)
                {
                    SubMeshObject = Parent.GetChild(ChildNum).gameObject;
                }
                SubMeshOptimization(SubMeshObject);
            }
        }
        public void SubMeshOptimization(GameObject RenderParent)
        {
            oldLocation = Parent.position;
            oldRotation = Parent.rotation;
            oldScale = Parent.localScale;
            Parent.position = Vector3.zero;
            Parent.rotation = Quaternion.identity;
            Parent.localScale = Vector3.one;
            MeshFilter[] meshFilters = RenderParent.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            if (meshFilters.Length == 0)
            {
                Debug.LogWarning($"Mesh Filters Length came back zero...{Parent.name}");
            }
            this.transform.GetComponent<MeshFilter>().mesh = new Mesh();
            this.transform.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            this.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            Parent.position = oldLocation;
            Parent.rotation = oldRotation;
            Parent.localScale = oldScale;

            UIManager.instance.UpdateTitleText ("Trees Built!");
            //StartCoroutine(WaitEndFrameResetMesh(RenderParent));
        }
        IEnumerator WaitEndFrameResetMesh(GameObject RenderParent)
        {
            yield return new WaitForEndOfFrame();
           
            MeshFilter[] meshFilters = RenderParent.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }

            this.transform.GetComponent<MeshFilter>().mesh = new Mesh();
            this.transform.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            this.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        }

    }
}

