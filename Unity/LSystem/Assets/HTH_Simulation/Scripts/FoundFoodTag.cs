using UnityEngine;
using System.Collections;

namespace HackTheHack.Simulation
{
    /// <summary>
    /// Created by John Shull
    /// For Use in the Reality Hack 2022 Hacking the Hack
    /// </summary>
    public class FoundFoodTag : MonoBehaviour
    {
        [Range(0.1f, 10.0f)]
        public float foodValue;
        public GameObject totalFood;
        void OnTriggerEnter(Collider other)
        {
            //if Insect enters  = ant status to having food
            //update parent by removing food quantity
            if (other.CompareTag("Ant"))
            {
                //reduce food and change status
                if (!other.gameObject.GetComponent<Insect>().foodStatus)
                {

                    if (totalFood.GetComponent<Food>().foodAmount > 0)
                    {
                        totalFood.GetComponent<Food>().foodAmount = totalFood.GetComponent<Food>().foodAmount - foodValue;
                        other.gameObject.GetComponent<Insect>().Finding_Food(true, foodValue);
                        other.gameObject.GetComponent<Insect>().FoodSource.SetActive(true);
                    }
                }

            }
        }


    }

}
