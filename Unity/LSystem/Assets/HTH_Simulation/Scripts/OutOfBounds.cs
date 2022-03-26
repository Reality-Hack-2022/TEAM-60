using UnityEngine;
using System.Collections;

namespace HackTheHack.Simulation
{
    /// <summary>
    /// Created by John Shull
    /// For Use in the Reality Hack 2022 Hacking the Hack
    /// Twitter: @TheJohnnyFuzz
    /// </summary>
    public class OutOfBounds : MonoBehaviour
    {
        public float highY;
        void OnTriggerEnter(Collider other)
        {

            if (other.CompareTag("Ant"))
            {
                Vector3 updatePos = other.transform.position;
                other.gameObject.transform.position = new Vector3(updatePos.x, highY, updatePos.z);
                other.gameObject.transform.rotation = Quaternion.identity;
            }
        }

    }
}

