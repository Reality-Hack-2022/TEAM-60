using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartContainer : MonoBehaviour
{
    public string Name;
    public List<GameObject> Prefabs;

    private List<GameObject> InactiveInstances = new List<GameObject>();

    void Start()
    {
    }



    public GameObject GetInstance()
    {
        if(InactiveInstances.Count > 0)
        {
            var instance = InactiveInstances[0];
            instance.SetActive(true);
            InactiveInstances.RemoveAt(0);
            return instance;
        }

        var newPart = GameObject.Instantiate(GetRandomPrefab());
        newPart.transform.parent = transform;
        return newPart;

    }

    private GameObject GetRandomPrefab()
    {
        var idx = Random.Range(0, Prefabs.Count);
        return Prefabs[idx];
    }


    public void DisableAllItems()
    {
        InactiveInstances.Clear();
        for(int i = 0; i < transform.childCount; i++)
        {
            var item = transform.GetChild(i);
            item.gameObject.SetActive(false);
            InactiveInstances.Add(item.gameObject);
        }
    }
}
