using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

namespace HackTheHack
{
    /// <summary>
    /// Delegate model with EventHandler to help publish information for subscribers who want to know
    /// Currently only have one really working which is GitHubCommit based
    /// </summary>
    public class HTH_Publisher
    {
        //setup delegate
        public delegate void EventHandler(GitHubResponseArgs message);

        //event definition by data needs
        public event EventHandler<GitHubResponseArgs> HTH_AzureGitHubResponse;
        public event EventHandler<Dictionary<string, List<CommitData>>> HTH_GitHubCommitResponse;
    
        //event GitHub Locally Finished Grabbing Data
        public void Call_HTH_GitHubCommitResponse(Dictionary<string,List<CommitData>> commitData)
        {
            HTH_GitHubCommitResponse?.Invoke(this, commitData);
        }
        //event Azure Finished GitHub Storage call

        public void Call_HTH_AzureGitHubResponse(GitHubResponseArgs aMessage) {
            HTH_AzureGitHubResponse?.Invoke(this, aMessage);
        } 
        /// <summary>
        /// Run a singleton pattern
        /// </summary>
        private static HTH_Publisher instance;
        protected HTH_Publisher() { }
        public static HTH_Publisher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HTH_Publisher();
                }
                return instance;
            }
        }
        

    }
    /// <summary>
    /// If we need to modify this message later this allows us a little bit more flex
    /// </summary>
    public class GitHubResponseArgs : EventArgs
    {
        public HttpResponseMessage httpResponse;
        public string messageContent;
        public string method;
    }
}
