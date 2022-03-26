using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace HackTheHack.Simulation {
    /// <summary>
    /// Created by John Shull
    /// For Use in the Reality Hack 2022 Hacking the Hack
    /// Twitter: @TheJohnnyFuzz
    /// </summary>
    public class HTHGrid : MonoBehaviour
    {

        float foodValue;
        public Material HeatMapMatInst;
        public MeshRenderer HeatMapQuad;
        Vector2 myGridLocation;
        public List<GameObject> neighbor8 = new List<GameObject>();
        public GameObject strongestNeighborHomeSmell;
        public GameObject strongestNeighborChemical;
        bool foundStrongNeighbor = false;
        public int width;
        public int height;
        public bool edge;
        public GameObject BoardControl_ref;
        public float homeSmell;
        public float totalChemical;
        private float evapRate;
        private float chemTrail = 100;
        private float foodSource = 100f;
        //public GameObject myColorPanel;
        //public GameObject myCanvas;
        public Color GridColor;


        void Start()
        {
            evapRate = BoardControl_ref.GetComponent<HTHSetupGrid>().evaporationRate;
            //myCanvas.GetComponent<RectTransform>().localScale = this.GetComponent<BoxCollider>().size;
            //need to make Y be Z
            HeatMapQuad.GetComponent<Transform>().localScale = new Vector3(this.GetComponent<BoxCollider>().size.x, this.GetComponent<BoxCollider>().size.z, this.GetComponent<BoxCollider>().size.z);
            //myColorPanel.GetComponent<RectTransform>().localScale = this.GetComponent<BoxCollider>().size;
            // myCanvas.GetComponent<RectTransform>().localScale = new Vector3(myCanvas.GetComponent<RectTransform>().localScale.x, myCanvas.GetComponent<RectTransform>().localScale.x, myCanvas.GetComponent<RectTransform>().localScale.z);
            HeatMapMatInst = HeatMapQuad.material;
        }

        public void BuildNeighbors()
        {

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                    {

                    }
                    else
                    {
                        if ((myGridLocation.x > 0 && myGridLocation.x < width - 1) && (myGridLocation.y > 0 && myGridLocation.y < height - 1))
                        {
                            //NON-EDGES

                            neighbor8.Add(NeighborAdd(myGridLocation.x, myGridLocation.y, i, j));
                        }
                        else
                        {
                            //EDGES
                            edge = true;
                            if (myGridLocation.x == 0 && (myGridLocation.y > 0 && myGridLocation.y < height - 1))
                            {
                                //left edge
                                if (i >= 0)
                                {

                                    neighbor8.Add(NeighborAdd(myGridLocation.x, myGridLocation.y, i, j));
                                }
                            }

                            else
                            {
                                //right edge
                                if (myGridLocation.x == width - 1 && (myGridLocation.y > 0 && myGridLocation.y < height - 1))
                                {
                                    if (i <= 0)
                                    {
                                        neighbor8.Add(NeighborAdd(myGridLocation.x, myGridLocation.y, i, j));
                                    }
                                }
                                else
                                {
                                    //bottoms and tops
                                    if (myGridLocation.y == 0 && (myGridLocation.x > 0 && myGridLocation.x < width - 1))
                                    {
                                        //bottom
                                        if (j >= 0)
                                        {
                                            neighbor8.Add(NeighborAdd(myGridLocation.x, myGridLocation.y, i, j));
                                        }
                                    }
                                    else
                                    {
                                        //top
                                        if ((myGridLocation.y == height - 1) && (myGridLocation.x > 0 && myGridLocation.x < width - 1))
                                        {
                                            if (j <= 0)
                                            {
                                                neighbor8.Add(NeighborAdd(myGridLocation.x, myGridLocation.y, i, j));
                                            }
                                        }
                                        else
                                        {
                                            //CORNERS
                                            if (myGridLocation.x == 0 && myGridLocation.y == 0)
                                            {
                                                //bot left
                                                if (i >= 0 && j >= 0)
                                                {
                                                    neighbor8.Add(NeighborAdd(myGridLocation.x, myGridLocation.y, i, j));
                                                }
                                            }
                                            else
                                            {
                                                if (myGridLocation.x == 0 && myGridLocation.y == height - 1)
                                                {
                                                    //top left
                                                    if (i >= 0 && j <= 0)
                                                    {
                                                        neighbor8.Add(NeighborAdd(myGridLocation.x, myGridLocation.y, i, j));
                                                    }
                                                }
                                                else
                                                {
                                                    if (myGridLocation.x == width - 1 && myGridLocation.y == 0)
                                                    {
                                                        //bot right
                                                        if (i <= 0 && j >= 0)
                                                        {
                                                            neighbor8.Add(NeighborAdd(myGridLocation.x, myGridLocation.y, i, j));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //top right
                                                        if (myGridLocation.x == width - 1 && myGridLocation.y == height - 1)
                                                        {
                                                            if (i <= 0 && j <= 0)
                                                            {
                                                                neighbor8.Add(NeighborAdd(myGridLocation.x, myGridLocation.y, i, j));
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }

                }

            }

        }

        GameObject NeighborAdd(float meX, float meZ, int x, int z)
        {
            string myGrid = "Grid_" + Mathf.RoundToInt(meX + x).ToString() + "_" + Mathf.RoundToInt(meZ + z).ToString();
            return GameObject.Find(myGrid);
        }
        public void Location(int w, int h)
        {
            myGridLocation = new Vector2(w, h);
        }

        void OnTriggerStay(Collider other)
        {
            //if ant type enters  = ant status to having food
            //update parent by removing food quantity
            if (!edge)
            {
                if (other.CompareTag("AntForward"))
                {
                    other.GetComponent<ForwardCollision>().forwardClear = true;

                }
                if (other.CompareTag("HomeBase"))
                {
                    // other.gameObject.GetComponent<_foundHome>()
                    if (other.gameObject.GetComponent<HomeMark>().homeReady)
                    {
                        other.gameObject.GetComponent<HomeMark>().myGrid = myGridLocation;
                        if (!BoardControl_ref.GetComponent<HTHSetupGrid>().homeSmellSetup)
                        {
                            BoardControl_ref.GetComponent<HTHSetupGrid>().SetupHomeSmell(other.gameObject);
                        }

                    }

                }
                
                if (other.CompareTag("FoodSource"))
                {
                    if (other.GetComponent<Food>())
                    {
                        if (other.GetComponent<Food>().foodAmount > 0)
                        {
                            //have food here
                            //totalChemical += foodSource;
                        }
                    }
                }
                
                if (other.CompareTag("Ant"))
                {
                    other.gameObject.GetComponent<Insect>().currentGrid = this.gameObject;
                    if (other.gameObject.GetComponent<Insect>().foodStatus)
                    {
                        //if carrying food leave some chemical behind
                        totalChemical += chemTrail;
                    }
                }
            }
            else
            {
                if (other.CompareTag("AntForward"))
                {
                    other.GetComponent<ForwardCollision>().forwardClear = false;
                }
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (edge)
            {
                if (other.CompareTag("AntForward"))
                {
                    other.GetComponent<ForwardCollision>().forwardClear = false;
                }
            }

        }

        public GameObject StrongestHomeScent()
        {
            //ask my neighbors who has strongest home scent
            if (!foundStrongNeighbor)
            {
                float maxHome = -10;

                foreach (GameObject aN8 in neighbor8)
                {
                    if (aN8.GetComponent<HTHGrid>().homeSmell > maxHome)
                    {
                        maxHome = aN8.GetComponent<HTHGrid>().homeSmell;
                        strongestNeighborHomeSmell = aN8;
                    }
                }
                foundStrongNeighbor = true;
            }
            return strongestNeighborHomeSmell;

        }

        void FixedUpdate()
        {
            Evaporate();
        }
        public void Evaporate()
        {
            if (totalChemical > 0.2)
            {
                totalChemical = totalChemical - ((totalChemical * ((100 - evapRate) / (float)100)) * Time.fixedDeltaTime);

            }
            else
            {
                totalChemical = 0;
            }
        }
        public void UpdateAlpha(float alpha)
        {
            float newAlpha = 0;
            if (alpha > 100)
            {
                newAlpha = 1;
            }
            else
            {
                newAlpha = alpha / 100f;
            }
            Color UpdateColor = new Color(GridColor.r, GridColor.g, GridColor.b, newAlpha);

            //myColorPanel.GetComponent<Image>().color = UpdateColor;
            HeatMapMatInst.color = UpdateColor;
        }

    }

}

