using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataVisMapping : MonoBehaviour
{

    public ManualTree Tree;

    void Start()
    {
        StartCoroutine(RunExample());

    }


    private IEnumerator RunExample()
    {
        for (int i = 0; i < 5; i++)
        {
            Tree.AddString("F[-&^F][^++&F]||F[--&^F][+&F]");
            yield return new WaitForSeconds(0.2f);
            Tree.AddString("F[-&^F]");
            yield return new WaitForSeconds(0.2f);
            Tree.AddString("F[+&F]");
            yield return new WaitForSeconds(0.2f);
        }

    }

}
