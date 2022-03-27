using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HackTheHack
{
    [Serializable]
    public class HTH_Discord_Raw
    {
        [SerializeField]
        public HTH_Discord_TextPayload[] undefined;
    }
    [Serializable]
    public class HTH_Discord_TextPayload
    {
        [SerializeField]
        public string AuthorTag;
        [SerializeField]
        public string Text;
    }

}
