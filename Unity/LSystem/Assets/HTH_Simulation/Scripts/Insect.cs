using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// This is work by John Shull
/// Free to use for the MIT Reality Hackathon 2022
/// </summary>
namespace HackTheHack.Simulation
{
    public class Insect : MonoBehaviour
    {

        [Range(0, 45)]
        public int turnDeg;
        [Range(0.00001f, 0.005f)]
        public float moveForward;
        public bool foodStatus;
        public GameObject forwardCol;
        public float foodValue;
        public GameObject currentGrid;
        public GameObject nextGrid;
        public float gridSize;
        public List<GameObject> GridFOV = new List<GameObject>();
        public float fieldOfViewAngle = 110f;
        public GameObject FoodSource;
        private bool _spawned;
        private System.Random RngControl;
        public AudioSource pickupSound;

        void Start()
        {
            foodStatus = false;
        }
        public void UpdateRNG(int seed)
        {
            RngControl = new System.Random(seed);
            _spawned = true;
        }
        public void RandomRNG()
        {
           
            RngControl = new System.Random(UnityEngine.Random.Range(0, 420000));
            _spawned = true;
        }
        public void Finding_Food(bool foodS, float foodA)
        {
            if (pickupSound) {
                pickupSound.Play();
            }

            if (foodS)
            {
                foodStatus = true;
                foodValue = foodA;
            }
            else
            {
                foodStatus = false;
                foodValue = 0;
            }
        }

        void Update()
        {
            if (!_spawned)
            {
                return;
            }
            //if I don't have food
            //look for food
            //zero out the xZ rotation
            //transform.rotation = transform.rotation * Quaternion.Euler(Vector3.up);
            if (!foodStatus)
            {
                LookForFood();

            }
            else
            {
                LookForHome();
                //drop chemical
            }
            MoveForward();
        }
        void LookForFood()
        {
            //if the grid we are on has some chemical value then check all of the surrounding grids
            if (currentGrid.GetComponent<HTHGrid>().totalChemical > 0)
            {
                //<JOHN> Dont look at the patch but look in the forward direction and grab the best within that area
                for (int j = 0; j < currentGrid.GetComponent<HTHGrid>().neighbor8.Count; j++)
                {
                    Vector3 dir = currentGrid.GetComponent<HTHGrid>().neighbor8[j].transform.position - transform.position;
                    float ang = Vector3.Angle(dir, transform.forward);
                    if (ang < fieldOfViewAngle * 0.5f)
                    {
                        GridFOV.Add(currentGrid.GetComponent<HTHGrid>().neighbor8[j]);
                    }
                }
                float maxChem = 0;

                for (int i = 0; i < GridFOV.Count; i++)
                {
                    if (GridFOV[i].GetComponent<HTHGrid>().totalChemical > maxChem)
                    {
                        nextGrid = GridFOV[i];
                        maxChem = GridFOV[i].GetComponent<HTHGrid>().totalChemical;
                    }
                }

                //transform.rotation = Quaternion.Lerp(transform.rotation, nextGrid.transform.rotation, Time.deltaTime);
                transform.LookAt(nextGrid.transform);
                AdjustDirection();
                GridFOV.Clear();
            }
            else
            {
                AdjustDirection();
            }


        }

        void LookForHome()
        {
            //face this direction then do some wiggling
            this.transform.LookAt(currentGrid.GetComponent<HTHGrid>().StrongestHomeScent().transform);
            AdjustDirection();
        }

        GameObject RayCastDirection(Vector3 myDir)
        {

            RaycastHit myHit;
            Ray myRay = new Ray(transform.position, myDir);
            if (Physics.Raycast(myRay, out myHit, gridSize * 10))
            {
                if (myHit.collider.CompareTag("grid"))
                {
                    GameObject GridHit = myHit.transform.gameObject;
                    return GridHit;
                }
            }
            return null;
        }
        void AdjustDirection()
        {
            int myRand = RngControl.Next(-1 * turnDeg, turnDeg);
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(myRand, Vector3.up), Time.deltaTime);
            transform.rotation *= Quaternion.AngleAxis(myRand, Vector3.up);


        }
        void MoveForward()
        {

            //check a gridsize in my forward direction
            //adjust my forward collision box
            if (forwardCol.GetComponent<ForwardCollision>().forwardClear)
            {
                transform.position = transform.position + (transform.forward * moveForward);
            }
            else
            {
                int myRand2 = RngControl.Next(145, 180);
                this.transform.Rotate(new Vector3(0, myRand2, 0));
            }

        }



    }
}

