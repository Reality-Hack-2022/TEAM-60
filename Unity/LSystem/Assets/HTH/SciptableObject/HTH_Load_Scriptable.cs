using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace HackTheHack
{
    /// <summary>
    /// Example of how you can listen to for the GitHub data to then process and generate 'n' trees
    /// </summary>
    public class HTH_Load_Scriptable : MonoBehaviour
    {
        /// <summary>
        /// This is for testing...
        /// </summary>
        public HTH_Tree_Data DataPlaceholder;
        public List<HTH_Tree_Data> AllGeneratedScriptableObjects = new List<HTH_Tree_Data>();
        private HTH_Tree_Data _dataFromCommit;
        [Tooltip("List to store the Prefabs for the data generation defaults")]
        public List<GameObject> BranchPrefabs = new List<GameObject>();
        [Tooltip("List to store the Prefabs for the data generation defaults")]
        public List<GameObject> LeafPrefabs = new List<GameObject>();
        [Space]
        [Header("Data Variables to set min/max")]
        [Tooltip("MinMax for Angle of Tree")]
        public Vector2 MinMax_TreeAngle=new Vector2(12f,45f);
        [Tooltip("Maximum for Time Between Commits, total seconds in a day = 86400")]
        public float MaxSecondsBetweenCommits = 86400;
        [Tooltip("Modulus value")]
        public int ModulusValue = 6;
        [Space]
        [Tooltip("MinMax Clamp Values for Segment Width")]
        public Vector2 Clamp_SegmentWidth = new Vector2(0.25f, 2f);
        public GameObject [] TreePrefabs;
        public GameObject TreePrefab;
        public Dictionary <string, GameObject> AllTreeInstances = new Dictionary<string, GameObject> ();


        public float min_Angle = 25f;
        public float max_Angle = 45f;
        public float min_SegmentWidth = 0.2f;
        public float max_SegmentWidth = 2f;
        public float min_SegmentHeight = 0.5f;
        public float max_SegmentHeight = 3f;
        public float min_LeafSize = 0.5f;
        public float max_LeafSize = 1.5f;
        public float min_SpawnRadius = 4f;
        public float max_SpawnRadius = 8f;


        
        //Dictionary<string, List<CommitData>> CommitData;
        /// <summary>
        /// This is an example I am whipping up really quick
        /// </summary>
        private List<HTH_Tree_Data> _treeData = new List<HTH_Tree_Data>();
        private void OnEnable()
        {
            HTH_Publisher.Instance.HTH_GitHubCommitResponse += (sender, e) => OnGitHubCommitResponse(sender, e);

        }
        private void OnDisable()
        {
            HTH_Publisher.Instance.HTH_GitHubCommitResponse -= (sender, e) => OnGitHubCommitResponse(sender, e);
        }
        #region CallBacks
       
        /// <summary>
        /// Callback to when we finish getting data locally from the commits (not going to the cloud)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private async void OnGitHubCommitResponse(object sender, Dictionary<string, List<CommitData>> data)
        {
            //cache data locally
            //CommitData = data;

            //destroy the trees
            // for (int i=0;i< AllTreeInstances.Count; i++)
            // {
            //     Destroy(AllTreeInstances[i]);
            // }
            // AllTreeInstances.Clear();

            if (data.Count <= 0) {
                foreach (string key in AllTreeInstances.Keys) {
                    AllTreeInstances [key].SetActive (false);
                }

                return;
            }

            UIManager.instance.UpdateTitleText ("Loading Data...");

            bool shouldSpawn = AllTreeInstances.Count > 0 ? false : true;

            //process commit data and then go generate 1 tree per team
            List<LSystemExecutor> ListOfExectuors = new List<LSystemExecutor>();
            
            Debug.LogWarning($"Number of Teams {data.Count}");
            //there are plenty of other ways to do this...
            var allDictionaryKeys = data.Keys.ToList();
            int maxNumCommits = data.Max(x => x.Value.Count);
            Debug.LogWarning($"Max Commits by a team: {maxNumCommits}");
            for (int j = 0; j < allDictionaryKeys.Count; j++)
            {

                string aKey = allDictionaryKeys[j]; ;
                List <CommitData> oneTeam = data[aKey];
                
                Debug.LogWarning($"Building the {aKey} tree");

                if (shouldSpawn) {
                    //generate the visual placeholder - scatter the flowers in a 10x10m area.
                    
                    float x = UnityEngine.Random.Range (-1f, 1f);
                    float z = UnityEngine.Random.Range (-1f, 1f);

                    float radius = UnityEngine.Random.Range (min_SpawnRadius, max_SpawnRadius);
                    Vector2 offset = new Vector2 (x, z).normalized * radius;

                    float yRot = UnityEngine.Random.Range (0, 360);
                    int prefabIndex = UnityEngine.Random.Range (0, TreePrefabs.Length); 

                    var aTree = GameObject.Instantiate(TreePrefabs [prefabIndex], this.transform.position + new Vector3(offset.x, 0, offset.y), Quaternion.Euler (0, yRot, 0));
                    AllTreeInstances.Add(aKey, aTree);

                    aTree.gameObject.name = aKey;     
                    AllTreeInstances [aKey].GetComponent <TreeDataMapping> ().InitLabel ();
                }
                TreeDataMapping mapping = AllTreeInstances [aKey].GetComponent <TreeDataMapping> ();
                
                // Calculate Mapping Weights
                float commitWeight = DataProcessor.instance.NormalizeCommitWeight (oneTeam.Count);
                float contributorWeight = DataProcessor.instance.NormalizeContributors (oneTeam);
                float messageWeight = DataProcessor.instance.NormalizeCommitMessages (oneTeam);

                Debug.Log (aKey + "'s commitWeight: " + commitWeight + ", contributorWeight: " + contributorWeight + ", messageWeight: " + messageWeight);

                // Create a Tree Data Scriptable Object instance.
                var adataTree = ScriptableObject.CreateInstance<HTH_Tree_Data>();
                AllGeneratedScriptableObjects.Add(adataTree);

                // Interpreter.LeafContainer.Prefabs = LeafPrefabs;
                // Interpreter.BranchContainer.Prefabs = BranchPrefabs;

                // Apply the mapping weights to the tree parameters
                adataTree.TeamName = aKey;

                // Set the Angle based on the total number of commits relative to the highest commit number
                adataTree.Angle = min_Angle + ((max_Angle - min_Angle) * commitWeight);
                // adataTree.Angle = Mathf.Min(Mathf.Max(MinMax_TreeAngle.x, oneTeam.Count), MinMax_TreeAngle.y);

                // // Set the Axiom based on the ???
                // adataTree.Axiom = "";

                // Set the Segment Width & Height based on the number of contributors
                adataTree.SegmentWidth = min_SegmentWidth + ((max_SegmentWidth - min_SegmentWidth) * contributorWeight);
                adataTree.SegmentHeight = min_SegmentWidth + ((max_SegmentWidth - min_SegmentWidth) * contributorWeight);

                // Set the Leaf Size based on the quality of commit messages
                adataTree.LeafSize = min_LeafSize + ((max_LeafSize - min_LeafSize) * messageWeight);

                // // Turn on thicker tree branches once the timeline has passed the hackathon 1/3 point.
                // adataTree.NarrowBranches = time > 0.3f ? false : true;

                // // Turn on tree foliage once the timeline has passed the hackathon 2/3 point.
                // adataTree.UseFoliage = time > 0.5f ? true : false;
                


                // //time span from first to last commit
                // float hoursFirstTOLast = 0;
                // var averageBetweenCommits = AverageTimeBetweenCommits(oneTeam, ref hoursFirstTOLast);
                // var segmentWidthNormalized = (float)averageBetweenCommits / MaxSecondsBetweenCommits;


                if (mapping.lsystem != null)
                {
                    // mapping.lsystem.rule = adataTree.Rule;
                    // mapping.lsystem.axiom = adataTree.Axiom;
                    // mapping.lsystem.derivations = adataTree.Derivations;
                }
                if (mapping.lsystemRenderer != null)
                {
                    mapping.lsystemRenderer.angle = adataTree.Angle;
                    // mapping.lsystemRenderer.segmentAxisSamples = adataTree.SegmentAxisSamples;
                    // mapping.lsystemRenderer.segmentRadialSamples = adataTree.SegmentRadialSamples;
                    mapping.lsystemRenderer.segmentWidth = adataTree.SegmentWidth;
                    mapping.lsystemRenderer.segmentHeight = adataTree.SegmentHeight;
                    mapping.lsystemRenderer.leafSize = adataTree.LeafSize;
                    // mapping.lsystemRenderer.leafAxialDensity = adataTree.LeafAxialDensity;
                    // mapping.lsystemRenderer.leafRadialDensity = adataTree.LeafRadialDensity;
                    mapping.lsystemRenderer.useFoliage = adataTree.UseFoliage;
                    mapping.lsystemRenderer.narrowBranches = adataTree.NarrowBranches;
                }
                
                ListOfExectuors.Add(mapping.lsystem);

                UIManager.instance.UpdateTitleText ("Rebuilding Trees...");                
                mapping.lsystem.Rebuild();
                mapping.RebuildSubmeshes();
            }

            // // Rebuild the Trees with Updated parameters
            // for (int x = 0; x < ListOfExectuors.Count; x++)
            // {
            //     ListOfExectuors[x].Rebuild();
            // }

            // Hide all other Trees
            foreach (string key in AllTreeInstances.Keys) {
                if (data.ContainsKey (key)) {
                    AllTreeInstances [key].SetActive (true);
                }

                else {
                    AllTreeInstances [key].SetActive (false);
                }
            }
        }
        
        private double AverageTimeBetweenCommits(List<CommitData> teamCommits, ref float totalHourSpan)
        {
            List<DateTime> allTimes = new List<DateTime>();
            List<TimeSpan> allSpans = new List<TimeSpan>();
            try
            {
                for (int i = 0; i < teamCommits.Count; i++)
                {
                    var aCommit = teamCommits[i];
                    //convert time back
                    var curDtime = ReturnDateTimeFromISO(aCommit.time);
                    allTimes.Add(curDtime);
                    if (i > 0 && i < teamCommits.Count - 1)
                    {
                        //get next time
                        var nextTime = ReturnDateTimeFromISO(teamCommits[i + 1].time);
                        var tCommitSpan = nextTime - curDtime;
                        allSpans.Add(tCommitSpan);
                    }
                }

                //total span of commit time from last to first
                if (allTimes.Count > 1)
                {
                    var tTimeSpan = allTimes[allTimes.Count - 1] - allTimes[0];
                    totalHourSpan = tTimeSpan.Hours;
                }
                //average time spans between commits
                var averageSpanBetweenCommitsSeconds = allSpans.Average(x => x.TotalSeconds);

                return averageSpanBetweenCommitsSeconds;
            }
            catch(Exception ex)
            {
                Debug.LogError($"Error on span {ex.Message}");
                return 10000;
            }
            

        }
        private DateTime ReturnDateTimeFromISO(string isoTime)
        {
            return DateTime.Parse(isoTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }
        #endregion
       
        
    }

}
