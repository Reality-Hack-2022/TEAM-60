using UnityEngine;
using System.Collections;

namespace HackTheHack.Simulation {
    /// <summary>
    /// Created by John Shull
    /// For Use in the Reality Hack 2022 Hacking the Hack
    /// Twitter: @TheJohnnyFuzz
    /// </summary>
    public class HomeMark : MonoBehaviour
    {

        public GameObject myHome_Vis_ref;
        public Transform antSpawnHome;
        public bool homeReady;
        public Vector2 myGrid;
        public GameObject GameBoardControl_ref;
        public GameObject AntPrefab_ref;
        //if we want to control the Seed for all randomness tied to Insects
        public bool SIMRepeatability;
        public int RNGSeed = 42424242;

        public void SpawnAnts()
        {
            //establish home strength

            foreach (GameObject aGrid in GameBoardControl_ref.GetComponent<HTHSetupGrid>().myGrid)
            {
                aGrid.GetComponent<HTHGrid>().StrongestHomeScent();
            }
            for (int i = 0; i < GameBoardControl_ref.GetComponent<HTHSetupGrid>().NumberInsects; i++)
            {
                GameObject newAnt = GameObject.Instantiate(AntPrefab_ref, antSpawnHome.position, Quaternion.identity) as GameObject;
                newAnt.GetComponent<Insect>().currentGrid = GameObject.Find("Grid_0_0");
                newAnt.transform.SetParent(this.transform);
                newAnt.GetComponent<Insect>().gridSize = GameBoardControl_ref.GetComponent<HTHSetupGrid>().gridSize / (float)1000f;
                newAnt.name = "Ant_" + i.ToString();
                newAnt.GetComponent<Insect>().nextGrid = newAnt.GetComponent<Insect>().currentGrid;
                newAnt.GetComponent<Insect>().moveForward = Random.Range(0.001f, 0.004f);
                if (SIMRepeatability)
                {
                    newAnt.GetComponent<Insect>().UpdateRNG(RNGSeed);
                    if (RNGSeed - 1 >= 0)
                    {
                        RNGSeed--;
                    } 
                }
                else
                {
                    newAnt.GetComponent<Insect>().RandomRNG();
                }
               
            }
        }
    }
}


