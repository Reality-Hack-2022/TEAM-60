using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http;
using UnityEngine.Events;
namespace HackTheHack
{
    /// <summary>
    /// Test Script Dont use this
    /// </summary>
    public class HTH_Subscriber : MonoBehaviour
    {
        /*
        [Tooltip("Event to invoke when Azure Data has been updated with GitHub data")]
        public UnityEvent AzureGitHubResponseEvent;
        [Tooltip("Event to invoke when we've locally grabbed the GitHub data")]
        public UnityEvent GitHubCommitResponseEvent;
        */
        //Data placeholders
        private GitHubResponseArgs gitHubResponse;
        //Data placeholders
        private Dictionary<string, List<CommitData>> CommitData;
        private void OnEnable()
        {
            HTH_Publisher.Instance.HTH_AzureGitHubResponse += (sender,e)=>OnGitHubResponse(sender,e);
            HTH_Publisher.Instance.HTH_GitHubCommitResponse += (sender, e) => OnGitHubCommitResponse(sender, e);
            
        }
        private void OnDisable()
        {
            HTH_Publisher.Instance.HTH_AzureGitHubResponse -= OnGitHubResponse;
            HTH_Publisher.Instance.HTH_GitHubCommitResponse -= (sender, e) => OnGitHubCommitResponse(sender, e);
        }
        #region CallBacks
        private void OnGitHubResponse(object sender, GitHubResponseArgs response)
        {
            //process GitHub Data
            gitHubResponse = response;
            if (response.httpResponse.IsSuccessStatusCode)
            {
                Debug.Log($"{response.method}: SUCCESS\n    {response.messageContent}\n\n");
                //AzureGitHubResponseEvent.Invoke();
            }
            else
            {
                Debug.LogWarning($"{response.method}: FAILED -> {(int)response.httpResponse.StatusCode}: {response.httpResponse.ReasonPhrase}.\n    {response.messageContent}\n\n");
            }
        }
        /// <summary>
        /// Callback to when we finish getting data locally from the commits (not going to the cloud)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void OnGitHubCommitResponse(object sender,Dictionary<string,List<CommitData>> data)
        {
            CommitData = data;
            //GitHubCommitResponseEvent.Invoke();
        }
        #endregion
        /// <summary>
        /// Public return our data after we have it
        /// returns null if we don't have any
        /// </summary>
        /// <returns></returns>
        public GitHubResponseArgs ReturnGHResponse()
        {
            if (gitHubResponse != null)
            {
                return gitHubResponse;
            }
            return null;
        }
        /// <summary>
        /// Public return on our commit data
        /// returns null if we don't have any
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,List<CommitData>> ReturnLocalCommitData()
        {
            if (CommitData != null)
            {
                return CommitData;
            }
            return null;
        }
    }

}
