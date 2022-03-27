using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HackTheHack
{
    /// <summary>
    /// Scriptable Object to hold the placevalues to have some data manipulate at runtime
    /// </summary>
    [CreateAssetMenu(fileName = "HTH_Discord_Data", menuName = "ScriptableObjects/HTH_Discord", order =2)]
    public class HTH_Discord_Data : ScriptableObject
    {
        [SerializeField]
        public string DateOfDataPull;
        [TextArea(minLines:4,maxLines:20)]
        public string RawJSON;
        [Space]
        public List<string> DiscordHandles;
        public Dictionary<string,int> NumberMessages;
        [SerializeField]
        public Dictionary<string,Dictionary<string, int>> NumberMentions;
        [Space]
        public List<string> DiscrodIP;
        [Space]
        public List<string> SpecialHandles;

    }

}
