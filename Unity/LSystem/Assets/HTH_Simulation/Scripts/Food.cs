using UnityEngine;
using System.Collections;
namespace HackTheHack.Simulation
{
    /// <summary>
    /// Created by John Shull
    /// For Use in the Reality Hack 2022 Hacking the Hack
    /// Twitter: @TheJohnnyFuzz
    /// </summary>
    public class Food : MonoBehaviour
    {
        [Header("Make Sure this item is within the two endpoints and on the grid")]
        [Range(1, 10000)]
        public float foodAmount;

        public GameObject foodRef_Visual;
        public GameObject model_1_p;
        public GameObject model_2_p;
        public GameObject model_3_p;
        private float startFood;
        private bool model_2;
        private bool model_3;
        private bool foodDone;
        void Start()
        {
            startFood = foodAmount;
            model_3_p.SetActive(false);
            model_2_p.SetActive(false);
            model_1_p.SetActive(true);
        }
        void Update()
        {
            if (!foodDone)
            {
                float foodR = foodAmount / (float)startFood;
                if (foodR <= 0.5f && !model_2)
                {
                    //switch model
                    model_2_p.SetActive(true);
                    model_1_p.SetActive(false);
                    model_2 = true;
                }
                if (foodR <= 0.3f && !model_3)
                {
                    model_2_p.SetActive(false);
                    model_3_p.SetActive(true);
                    model_3 = true;
                }
                if (foodR <= 0.05)
                {
                    model_3_p.SetActive(false);
                    foodDone = true;
                    foodAmount = 0;
                }
            }


        }
    }
}

