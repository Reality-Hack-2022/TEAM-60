using UnityEngine;
using System.Collections;

namespace HackTheHack.Simulation

{
    /// <summary>
    /// Created by John Shull
    /// For Use in the Reality Hack 2022 Hacking the Hack
    /// Twitter: @TheJohnnyFuzz
    /// </summary>
    public class FoundHome : MonoBehaviour
    {

        public float totalFood = 0;
        public GameObject gameBoardController;

        void OnTriggerEnter(Collider other)
        {
            //if ant type enters  = ant status to having food
            //update parent by removing food quantity
            if (other.CompareTag("Ant"))
            {
                //reduce food and change status
                if (other.gameObject.GetComponent<Insect>().foodStatus)
                {
                    totalFood = totalFood + other.gameObject.GetComponent<Insect>().foodValue;
                    other.gameObject.GetComponent<Insect>().Finding_Food(false, 0);
                    other.gameObject.GetComponent<Insect>().FoodSource.SetActive(false);
                }
            }
        }
    }
}

