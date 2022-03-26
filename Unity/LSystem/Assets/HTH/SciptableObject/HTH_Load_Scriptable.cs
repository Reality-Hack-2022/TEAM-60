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
        public GameObject TreePrefab;
        public List<GameObject> AllTreeInstances = new List<GameObject>();
        
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
        private void OnGitHubCommitResponse(object sender, Dictionary<string, List<CommitData>> data)
        {
            //cache data locally
            //CommitData = data;

            //destroy the trees
            for (int i=0;i< AllTreeInstances.Count; i++)
            {
                Destroy(AllTreeInstances[i]);
            }
            AllTreeInstances.Clear();

            //process commit data and then go generate 1 tree per team
            List<LSystemExecutor> ListOfExectuors = new List<LSystemExecutor>();
            
            Debug.LogWarning($"Number of Teams {data.Count}");
            //there are plenty of other ways to do this...
            var allDictionaryKeys = data.Keys.ToList();
            int maxNumCommits = data.Max(x => x.Value.Count);
            Debug.LogWarning($"Max Commits by a team: {maxNumCommits}");
            for (int j = 0; j < allDictionaryKeys.Count; j++)
            {

                var aKey = allDictionaryKeys[j]; ;
                var oneTeam = data[aKey];
                
                Debug.LogWarning($"Building the {aKey} tree");
                var adataTree = ScriptableObject.CreateInstance<HTH_Tree_Data>();
                AllGeneratedScriptableObjects.Add(adataTree);
                //generate the visual placeholder - move it on the z axis 1.25 meters forward
                var aTree = GameObject.Instantiate(TreePrefab, this.transform.position + new Vector3(0, 0, 1.25f*j), Quaternion.identity);

                aTree.gameObject.name = aKey;
                LSystemExecutor Executor = aTree.GetComponent<LSystemExecutor>();
                LSystemInterpreter Interpreter = aTree.GetComponent<LSystemInterpreter>();
                //generate the data we don't have as a default
                Interpreter.LeafContainer.Prefabs = LeafPrefabs;
                Interpreter.BranchContainer.Prefabs = BranchPrefabs;

                //Start making up some placeholders for the data
                adataTree.TeamName = aKey;
                adataTree.Angle = Mathf.Min(Mathf.Max(MinMax_TreeAngle.x, oneTeam.Count), MinMax_TreeAngle.y);
                //AllGeneratedScriptableObjects.Add(adataTree);
                //time span from first to last commit
                float hoursFirstTOLast = 0;
                var averageBetweenCommits = AverageTimeBetweenCommits(oneTeam, ref hoursFirstTOLast);
                var segmentWidthNormalized = (float)averageBetweenCommits / MaxSecondsBetweenCommits;
                adataTree.SegmentWidth = Mathf.Clamp(segmentWidthNormalized, Clamp_SegmentWidth.x, Clamp_SegmentWidth.y);
                adataTree.Derivations = oneTeam.Count % ModulusValue;
                if (adataTree.Derivations == 0)
                {
                    adataTree.Derivations = ModulusValue;
                }
                AllTreeInstances.Add(aTree);
                if (Executor != null)
                {
                    Executor.rule = adataTree.Rule;
                    Executor.axiom = adataTree.Axiom;
                    Executor.derivations = adataTree.Derivations;
                }
                if (Interpreter != null)
                {
                    Interpreter.angle = adataTree.Angle;
                    Interpreter.segmentAxisSamples = adataTree.SegmentAxisSamples;
                    Interpreter.segmentRadialSamples = adataTree.SegmentRadialSamples;
                    Interpreter.segmentWidth = adataTree.SegmentWidth;
                    Interpreter.segmentHeight = adataTree.SegmentHeight;
                    Interpreter.leafSize = adataTree.LeafSize;
                    Interpreter.leafAxialDensity = adataTree.LeafAxialDensity;
                    Interpreter.leafRadialDensity = adataTree.LeafRadialDensity;
                    Interpreter.useFoliage = adataTree.UseFoliage;
                    Interpreter.narrowBranches = adataTree.NarrowBranches;
                }
                ListOfExectuors.Add(Executor);
            }
            
            for (int x = 0; x < ListOfExectuors.Count; x++)
            {
                ListOfExectuors[x].Rebuild();
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
