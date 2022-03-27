using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HackTheHack
{
    /// <summary>
    /// Scriptable Object to hold the placevalues to have some data manipulate at runtime
    /// </summary>
    [CreateAssetMenu(fileName = "HTH_Discord_Data", menuName = "ScriptableObjects/HTH", order =2)]
    public class HTH_Discord_Data : ScriptableObject
    {
        public string RawJSON;
    }

}
