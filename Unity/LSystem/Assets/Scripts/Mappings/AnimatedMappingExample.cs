using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedMappingExample : MonoBehaviour
{
    public Mapping mapping;

    void Start()
    {
        StartCoroutine(UpdateCount());
        StartCoroutine(UpdateStrength());
    }


    public IEnumerator UpdateCount()
    {
        while(mapping.Count < 800)
        {
            yield return new WaitForSeconds(0.3f);
            mapping.Count += Random.Range(10, 100);
        }
    }
    public IEnumerator UpdateStrength()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            mapping.Strength = Random.Range(0.1f, 0.8f);
        }
    }

}
