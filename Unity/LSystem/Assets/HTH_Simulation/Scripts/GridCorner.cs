using UnityEngine;
using System.Collections;

namespace HackTheHack.Simulation {

    /// <summary>
    /// Created by John Shull
    /// For Use in the Reality Hack 2022 Hacking the Hack
    /// Twitter: @TheJohnnyFuzz
    /// </summary>
    public class GridCorner : MonoBehaviour
    {
        [Header("Make sure these are Left/Bottom & Right/Up on the X-Z")]
        public bool visibleStatus;
        bool messageSent;
        public GameObject myVisual;
        public GameObject controlRef;

       
        void Start()
        {
            LiveGridCheck();
            myVisual.SetActive(false);
        }
        /// <summary>
        /// If you wanted to continuously update the grid you need to also run it in update. Commenting out to save on scripts running
        /// </summary>
        /*
        private void Update()
        {
            LiveGridCheck();
        }
        */
        private void LiveGridCheck()
        {
            //report back status
            visibleStatus = myVisual.activeInHierarchy;
            if (visibleStatus && !messageSent)
            {
                //send command up

                controlRef.GetComponent<HTHSetupGrid>().GridStatus();
                messageSent = true;
            }
            else
            {
                if (!visibleStatus && !messageSent)
                {
                    controlRef.GetComponent<HTHSetupGrid>().GridStatus();
                }
            }
            if (!visibleStatus)
            {
                messageSent = false;
            }
        }
    }

}
