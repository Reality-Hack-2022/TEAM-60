using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HackTheHack.Simulation {

    /// <summary>
    /// Created by John Shull
    /// For Use in the Reality Hack 2022 Hacking the Hack
    /// Setup the Grid
    /// </summary>
    public class HTHSetupGrid : MonoBehaviour
    {
        //holds my corner gameObjects
        public bool ShowFakeFloor;
        public List<GameObject> Corners = new List<GameObject>();
        public List<GameObject> myGrid = new List<GameObject>();
        public int gridSize; //mm
        [SerializeField]
        GameObject GridPrefab;
        [SerializeField]
        GameObject floor;
        [SerializeField]
        GameObject floorZone;
        [SerializeField]
        GameObject antHome;
        bool homeSetup;
        public bool homeSmellSetup;
        [Range(1, 95)]
        public float evaporationRate;
        [Range(2, 250)]
        public int NumberInsects;
        void Awake()
        {
            floor.SetActive(false);
            homeSmellSetup = false;
        }
        void Update()
        {
            //check distance
            if (Corners[0].GetComponent<GridCorner>().visibleStatus)
            {
                if (Corners[1].GetComponent<GridCorner>().visibleStatus)
                {
                    //both are visible now check distance

                }
            }
        }
        void LateUpdate()
        {
            //diffuse();
            diffuseTwo();
        }
        public void GridStatus()
        {
            //checks each corner to see if it's active
            int activeCorners = 0;
            foreach (GameObject aC in Corners)
            {
                if (aC.GetComponent<GridCorner>().visibleStatus)
                {
                    activeCorners++;
                }
            }
            if (activeCorners == Corners.Count)
            {
                //grid is ready
                BuildGrid();
                SetupNeighbors();
                SetupHome();
            }
            else
            {
                DestroyGrid();
            }
        }

        void BuildGrid()
        {
            //establish rectangle
            floor.SetActive(true);

            floor.GetComponent<MeshRenderer>().enabled = ShowFakeFloor;
            floorZone.SetActive(true);
            float disX = Mathf.Abs(Corners[0].GetComponent<GridCorner>().myVisual.transform.position.x - Corners[1].GetComponent<GridCorner>().myVisual.transform.position.x);
            float disZ = Mathf.Abs(Corners[0].GetComponent<GridCorner>().myVisual.transform.position.z - Corners[1].GetComponent<GridCorner>().myVisual.transform.position.z);
            floor.transform.localScale = new Vector3(disX / (float)10, 1, disZ / (float)10);
            floorZone.transform.localScale = new Vector3(disX, 1, disZ);
            //grid size is in mm measurement above is in m
            disX *= 1000;
            disZ *= 1000;
            int width = 0;
            int height = 0;
            if (gridSize <= disX)
            {
                width = Mathf.RoundToInt(disX / gridSize);
            }
            if (gridSize <= disZ)
            {
                height = Mathf.RoundToInt(disZ / gridSize);
            }
            //world coordinate
            Vector3 midPoint = new Vector3((Corners[0].GetComponent<GridCorner>().myVisual.transform.position.x + Corners[1].GetComponent<GridCorner>().myVisual.transform.position.x) / (float)2, Corners[0].GetComponent<GridCorner>().myVisual.transform.position.y, (Corners[0].GetComponent<GridCorner>().myVisual.transform.position.z + Corners[1].GetComponent<GridCorner>().myVisual.transform.position.z) / (float)2);
            //establish our floor
            floor.transform.position = midPoint;
            floorZone.transform.position = midPoint;
            floorZone.transform.position = new Vector3(midPoint.x, midPoint.y - 0.6f, midPoint.z);
            floorZone.GetComponent<OutOfBounds>().highY = midPoint.y + 0.1f;
            Vector3 startingPos = Vector3.zero;
            Vector3 gridScale = new Vector3(gridSize / 1000f, 1, gridSize / 1000f);
            float shiftAmount = (gridSize / (float)2000);
            for (int i = 0; i < height; i++)
            {
                startingPos = new Vector3(Corners[0].GetComponent<GridCorner>().myVisual.transform.position.x, startingPos.y, Corners[0].GetComponent<GridCorner>().myVisual.transform.position.z + shiftAmount + ((gridSize / (float)1000) * i));
                for (int j = 0; j < width; j++)
                {

                    startingPos = new Vector3((Corners[0].GetComponent<GridCorner>().myVisual.transform.position.x + shiftAmount) + ((gridSize / (float)1000) * j), startingPos.y, startingPos.z);
                    GameObject Grid1 = GameObject.Instantiate(GridPrefab, startingPos, Quaternion.identity) as GameObject;
                    Grid1.GetComponent<HTHGrid>().BoardControl_ref = this.gameObject;
                    Grid1.GetComponent<BoxCollider>().size = gridScale;
                    Grid1.name = "Grid_" + j.ToString() + "_" + i.ToString();
                    Grid1.GetComponent<HTHGrid>().Location(j, i);
                    Grid1.GetComponent<HTHGrid>().width = width;
                    Grid1.GetComponent<HTHGrid>().height = height;
                    Grid1.transform.SetParent(transform);
                    myGrid.Add(Grid1);
                }
            }
        }
        void DestroyGrid()
        {
            foreach (GameObject oneG in myGrid)
            {
                Destroy(oneG);
            }
            myGrid.Clear();
            GameObject[] AllAnts = GameObject.FindGameObjectsWithTag("Ant");
            foreach (GameObject anAnt in AllAnts)
            {
                Destroy(anAnt);
            }
            floor.SetActive(false);
            homeSmellSetup = false;
        }
        void SetupNeighbors()
        {
            foreach (GameObject grid in myGrid)
            {
                grid.GetComponent<HTHGrid>().BuildNeighbors();
            }
        }

        void SetupHome()
        {
            //first need to see if we can find the home
            //if we find the home then check to see if it's within our grid
            //if both are true then activate it
            if (antHome.GetComponent<HomeMark>().myHome_Vis_ref.activeInHierarchy)
            {
                GameObject myAntHomeRef = antHome.GetComponent<HomeMark>().myHome_Vis_ref;
                if (((myAntHomeRef.transform.position.x > Corners[0].GetComponent<GridCorner>().myVisual.transform.position.x) && (myAntHomeRef.transform.position.x < Corners[1].GetComponent<GridCorner>().myVisual.transform.position.x)) && ((myAntHomeRef.transform.position.z > Corners[0].GetComponent<GridCorner>().myVisual.transform.position.z) && (myAntHomeRef.transform.position.z < Corners[1].GetComponent<GridCorner>().myVisual.transform.position.z)))
                {
                    //in between the workable space
                    antHome.GetComponent<HomeMark>().homeReady = true;
                }
                else
                {
                    antHome.GetComponent<HomeMark>().homeReady = false;
                    Debug.Log("Home not in bounds");
                }
            }

        }
        public void SetupHomeSmell(GameObject homeLocation)
        {
            float maxSmell = 100;
            if (!homeSmellSetup)
            {
                foreach (GameObject aGrid in myGrid)
                {
                    //add a value of home smell to grid with stronger values being closer to home

                    float myDist = Vector3.Distance(homeLocation.transform.position, aGrid.transform.position);
                    if (myDist < 0.1)
                    {
                        //max smell
                        aGrid.GetComponent<HTHGrid>().homeSmell = maxSmell * maxSmell;
                    }
                    else
                    {
                        aGrid.GetComponent<HTHGrid>().homeSmell = maxSmell / (float)myDist;
                    }
                }
                homeSmellSetup = true;
                //spawn ants!
                antHome.GetComponent<HomeMark>().SpawnAnts();
            }
        }

        public void diffuse()
        {
            //diffuse value out
            foreach (GameObject oneGrid in myGrid)
            {

                float totalChemical = oneGrid.GetComponent<HTHGrid>().totalChemical;
                if (totalChemical > 0.2)
                {
                    if (totalChemical > 0.2 && totalChemical < 100)
                    {
                        float halfChem = totalChemical / (float)2;
                        foreach (GameObject aN8 in oneGrid.GetComponent<HTHGrid>().neighbor8)
                        {

                            float oneE = halfChem / (float)8;
                            aN8.GetComponent<HTHGrid>().totalChemical += oneE;
                        }

                        oneGrid.GetComponent<HTHGrid>().totalChemical = totalChemical - halfChem;
                    }
                    else
                    {
                        oneGrid.GetComponent<HTHGrid>().totalChemical = 99;
                    }
                }
                else
                {
                    totalChemical = 0;
                    oneGrid.GetComponent<HTHGrid>().totalChemical = totalChemical;
                }

                oneGrid.GetComponent<HTHGrid>().UpdateAlpha(totalChemical);
            }

        }

        public void diffuseTwo()
        {
            for (int i = 0; i < myGrid.Count; i++)
            {
                GameObject oneGrid = myGrid[i];
                float totalChemical = oneGrid.GetComponent<HTHGrid>().totalChemical;
                if (totalChemical > 0.2)
                {
                    if (totalChemical > 0.2 && totalChemical < 100)
                    {
                        float halfChem = totalChemical / (float)2;
                        foreach (GameObject aN8 in oneGrid.GetComponent<HTHGrid>().neighbor8)
                        {

                            float oneE = (halfChem / (float)8) * Time.deltaTime;
                            aN8.GetComponent<HTHGrid>().totalChemical += oneE;
                        }

                        oneGrid.GetComponent<HTHGrid>().totalChemical = totalChemical - (halfChem * Time.deltaTime);
                    }
                    else
                    {
                        oneGrid.GetComponent<HTHGrid>().totalChemical = 99;
                    }
                }
                else
                {
                    totalChemical = 0;
                    oneGrid.GetComponent<HTHGrid>().totalChemical = totalChemical;
                }

                oneGrid.GetComponent<HTHGrid>().UpdateAlpha(totalChemical);
            }
        }

    }

}

