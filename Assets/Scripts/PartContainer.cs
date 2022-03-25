using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartContainer : MonoBehaviour
{
    public string Name;
    public GameObject Prefab;

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

        var newPart = GameObject.Instantiate(Prefab);
        newPart.transform.parent = transform;
        return newPart;

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
