using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Collections;
using System.Net.Http;
using HackTheHack;
using UnityEngine;
using SimpleJSON;
using System;
/// <summary>
/// Helps pull down some GitHub Data
/// In order to use this class for 2022
/// you will have to modify orgDirectory & orgRepos
/// </summary>
public class GithubRequester : MonoBehaviour
{
    [SerializeField]
    public List <string> repoNames = new List <string>();
    [SerializeField]
    public Dictionary <string, List <CommitData>> githubDataDict = new Dictionary<string, List<CommitData>> ();
    [Tooltip("You will need your own GitHub PAT")]//https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token
    public string githubToken;
    public string githubUsername;
    [Tooltip("Change this to match the recent Hackathon")]
    public string orgDirectory = "https://api.github.com/orgs/MIT-Reality-Hack-2020/";
    [Tooltip("Change this to match the recent Hackathon")]
    public string orgRepos = "https://api.github.com/repos/MIT-Reality-Hack-2020/";
    //Testing Variables
    [TextArea]
    public string jsonResponse;
    // Start is called before the first frame update
    async Task Start()
    {
        // Load Data on start
        await LoadGithubData ();
    }
    async Task LoadGithubData () {
        ResetData();
        await GetRepositoryDataHTTP();
        for(int i = 0; i < repoNames.Count; i++)
        {
            await GetCommitDataHTTP(repoNames[i]);
        }
        HTH_Publisher.Instance.Call_HTH_GitHubCommitResponse(githubDataDict);
        await DebugGitHubRateLimitHTTP();
    }

    public void ResetData () {
        repoNames.Clear();
        githubDataDict.Clear();
    }

    public Dictionary <string, List <CommitData>> GetGithubData () {
        if (githubDataDict.Count > 0) {
            return githubDataDict;
        }
        return null;
    }

    #region HTTPClient Repository GETS
    /// <summary>
    /// For API information see 
    /// https://docs.github.com/en/rest/reference/rate-limit
    /// </summary>
    /// <returns></returns>
    async Task DebugGitHubRateLimitHTTP()
    {
        using (var client = new HttpClient())
        {
            var method = HttpMethod.Get;
            client.DefaultRequestHeaders.Accept.Clear();
            client.BaseAddress = new Uri("https://api.github.com/rate_limit");
            client.DefaultRequestHeaders.Add("User-Agent", githubUsername);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization","token " +githubToken);
            var httpRequest = new HttpRequestMessage { Method = method };
            var httpResponse = await client.SendAsync(httpRequest);
          
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                Debug.LogWarning($"Success: { responseContent}");
                JSONNode rateLimitInfo = JSON.Parse(responseContent);

                string curUsed = rateLimitInfo["rate"]["used"];
                string curLeft = rateLimitInfo["rate"]["remaining"];
                Debug.Log($"Used: {curUsed}, Left: {curLeft}");
            }
            else
            {
                Debug.LogError($"Failure: {httpResponse.StatusCode}");
            }
        }
    }
    /// <summary>
    /// For API information see
    /// https://docs.github.com/en/rest/reference/repos
    /// </summary>
    /// <returns></returns>
    async Task GetRepositoryDataHTTP()
    {
        using (var client = new HttpClient())
        {
            var method = HttpMethod.Get;
            client.DefaultRequestHeaders.Accept.Clear();
            client.BaseAddress = new Uri(orgDirectory+"repos?per_page=100");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            
            client.DefaultRequestHeaders.Add("User-Agent", githubUsername);
            client.DefaultRequestHeaders.Add("Authorization", "token " + githubToken);
            
            var httpRequest = new HttpRequestMessage { Method = method };
         
            var httpResponse = await client.SendAsync(httpRequest);
           
            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                jsonResponse = responseContent;
                JSONNode orgInfo = JSON.Parse(responseContent);
                Debug.LogWarning($"Success: { responseContent}");
                Debug.Log($"Repo Count in Org: {orgInfo.Count}");

                for (int i = 0; i < orgInfo.Count; i++)
                {
                    repoNames.Add(orgInfo[i]["name"]);
                    Debug.Log($"Adding repo: {orgInfo[i]["name"]}");
                }
            }
            else
            {
                Debug.LogError($"Failure: {httpResponse.StatusCode}");
            }
        }
    }
    /// <summary>
    /// For API information see
    /// https://docs.github.com/en/rest/reference/commits
    /// </summary>
    /// <param name="repoName"></param>
    /// <returns></returns>
    async Task GetCommitDataHTTP(string repoName)
    {
        using (var client = new HttpClient())
        {
            var method = HttpMethod.Get;
            client.DefaultRequestHeaders.Accept.Clear();
            client.BaseAddress = new Uri(orgRepos + repoName + "/commits");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", githubUsername);
            client.DefaultRequestHeaders.Add("Authorization", "token " + githubToken);
            client.DefaultRequestHeaders.Add("page", "1");
            client.DefaultRequestHeaders.Add("per_page", "75");
            var httpRequest = new HttpRequestMessage { Method = method };
            var httpResponse = await client.SendAsync(httpRequest);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                Debug.LogWarning($"Success: { responseContent}, {httpResponse.RequestMessage}");
                JSONNode commitInfo = JSON.Parse(responseContent);

                List<CommitData> dataset = new List<CommitData>();

                for (int i = 0; i < commitInfo.Count; i++)
                {
                    JSONNode currInfo = commitInfo[i];
                    string commitMessage = currInfo["commit"]["message"];
                    string commitAuthor = currInfo["commit"]["author"]["name"];
                    string commitTime = currInfo["commit"]["author"]["date"];
                    string commitSha = currInfo["sha"];
                    string commitAvatarUrl = currInfo["author"]["avatar_url"];
                    string commitUrl = currInfo["url"];
                    List<string> commitParentUrls = new List<string>();
                    List<string> commitParentShas = new List<string>();

                    for (int j = 0; j < currInfo["parents"].Count; j++)
                    {
                        commitParentUrls.Add(currInfo["parents"][j]["url"]);
                        commitParentShas.Add(currInfo["parents"][j]["sha"]);
                    }

                    // Store parsed info into data container object
                    CommitData commitData = new CommitData();
                    commitData.time = commitTime;
                    commitData.message = commitMessage;
                    commitData.author = commitAuthor;
                    commitData.avatar_url = commitAvatarUrl;
                    commitData.commit_url = commitUrl;
                    commitData.parent_url = commitParentUrls;
                    commitData.sha = commitSha;
                    commitData.parent_sha = commitParentShas;
                    commitData.DebugCommitData();

                    dataset.Add(commitData);
                }
                //remove duplicates
                //var result = dataset.Distinct(new ItemEqualityComparer());
                
                // Store commit data set in a dictionary with the repo name as the key.
                if (!githubDataDict.ContainsKey(repoName))
                {
                    githubDataDict.Add(repoName, dataset);
                }

                // remove the repo name from the list.
               
            }
            else
            {

                //Debug.LogError($"Failure: {httpResponse.StatusCode} with message: {httpResponse.RequestMessage}");
            }
        }

    }
    #endregion

    #region Unity WebRequest Methods - not being used
    IEnumerator DebugGithubRateLimit () {
        UnityWebRequest request = UnityWebRequest.Get ("https://api.github.com/rate_limit");
        request.SetRequestHeader ("User-Agent", githubUsername);
        request.SetRequestHeader ("Authorization", "token " + githubToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError (request.error);
            yield break;
        }

        JSONNode rateLimitInfo = JSON.Parse (request.downloadHandler.text);

        string currLimit = rateLimitInfo ["rate"] ["used"];

        Debug.Log ("Current remaining rate: " + currLimit);
    }

    IEnumerator GetRepositoryData () {

        UnityWebRequest request = UnityWebRequest.Get (orgDirectory + "repos");
        request.SetRequestHeader ("User-Agent", githubUsername);
        request.SetRequestHeader ("Authorization", "token " + githubToken);
        // request.SetRequestHeader ("per_page", "100");

        yield return request.SendWebRequest();


        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError (request.error);
            yield break;
        }

        // Parse specific parameters from the parsed json
        JSONNode orgInfo = JSON.Parse (request.downloadHandler.text);

        Debug.Log ("Repo Count in Org: " + orgInfo.Count);

        for (int i = 0; i < orgInfo.Count; i++) {
            repoNames.Add (orgInfo [i] ["name"]);
            Debug.Log ("Adding repo: " + orgInfo [i] ["name"]);
        }

        StartCoroutine (GetCommitData (repoNames[0]));
    }


        // Potential Relevant Data
        // During-Hackathon
        // "/repos/{owner}/{repo}" = get high-level repository information
        // "/repos/{owner}/{repo}/languages" = breakdown of different languages in repo
        // "/repos/{owner}/{repo}/tags" = list tags
        // "/repos/{owner}/{repo}/releases" = list releases
        // "/repos/{owner}/{repo}/pulls" = list pull requests
        // "/repos/{owner}/{repo}/pulls/{pull_number}/commits" = list commits on a pull request
        // "/repos/{owner}/{repo}/pulls/{pull_number}/files" = list pull requests files
        // "/repos/{owner}/{repo}/pulls/{pull_number}/comments" = list review comments on a pull request
        // "/orgs/{org}" = get organization info
        // "/repos/{owner}/{repo}/collaborators" = list repo collaborators
        // "/repos/{owner}/{repo}/branches" = list branches in a repo
        // "/repos/{owner}/{repo}/subscribers" = list watchers of a repo

        // Post-Hackathon
        // "/repos/{owner}/{repo}/stats/contributors" = total number of additions,deletions,commits
        // "/repos/{owner}/{repo}/stats/punch_card" = hourly number of commits (Day + Hour stamp)
        // "/repos/{owner}/{repo}/traffic/views" = page views
        // "/repos/{owner}/{repo}/traffic/clones" = total number of clones, timestamped
        // "/repos/{owner}/{repo}/stargazers" = lists the people that have favorited the repo

    IEnumerator GetCommitData (string repoName) {

        Debug.Log ("Requesting Commit data from: " + repoName);
        UnityWebRequest request = UnityWebRequest.Get (orgDirectory + repoName + "/commits");
        request.SetRequestHeader ("User-Agent", githubUsername);
        request.SetRequestHeader ("Authorization", "token " + githubToken);

        yield return request.SendWebRequest();

        
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError (request.error);
            yield break;
        }

        // Parse specific parameters from the parsed json
        JSONNode commitInfo = JSON.Parse (request.downloadHandler.text);

        List <CommitData> dataset = new List<CommitData> ();

        for (int i = 0; i < commitInfo.Count; i++) {
            JSONNode currInfo = commitInfo [i];
            string commitMessage = currInfo ["commit"] ["message"];
            string commitAuthor = currInfo ["commit"] ["author"] ["name"];
            string commitTime = currInfo ["commit"] ["author"] ["date"];
            string commitSha = currInfo ["sha"];
            string commitAvatarUrl = currInfo ["author"] ["avatar_url"];
            string commitUrl = currInfo ["url"];
            List <string> commitParentUrls = new List<string> ();
            List <string> commitParentShas = new List<string> ();

            for (int j = 0; j < currInfo ["parents"].Count; j++) {
                commitParentUrls.Add (currInfo ["parents"] [j] ["url"]);
                commitParentShas.Add (currInfo ["parents"] [j] ["sha"]);
            }

            // Store parsed info into data container object
            CommitData commitData = new CommitData ();
            commitData.time = commitTime;
            commitData.message = commitMessage;
            commitData.author = commitAuthor;
            commitData.avatar_url = commitAvatarUrl;
            commitData.commit_url = commitUrl;
            commitData.parent_url = commitParentUrls;
            commitData.sha = commitSha;
            commitData.parent_sha = commitParentShas;
            commitData.DebugCommitData ();

            dataset.Add (commitData);
        }

        // Store commit data set in a dictionary with the repo name as the key.
        if (!githubDataDict.ContainsKey (repoName)) {
            githubDataDict.Add (repoName, dataset);
        }

        // remove the repo name from the list.
        repoNames.Remove (repoName);

        // Continue iterating through the remaining repository names, otherwise stop the loop.
        if (repoNames.Count > 0) {
            StartCoroutine (GetCommitData (repoNames [0]));
        }
    }
#endregion
}

/// <summary>
/// One approach to how you can do a custom compare with Linq - not being used
/// </summary>
class ItemEqualityComparer : IEqualityComparer<CommitData>
{
    public bool Equals(CommitData x, CommitData y)
    {
        // Two items are equal if their keys are equal.
        return x.sha == y.sha;
    }

    public int GetHashCode(CommitData obj)
    {
        return obj.sha.GetHashCode();
    }
}
/// <summary>
/// Data class to store basic commit data
/// </summary>
public class CommitData {
    public string time;
    public string message;
    public string author;
    public string commit_url;
    public string avatar_url;
    public List <string> parent_url;
    public string sha;
    public List <string> parent_sha;

    public void DebugCommitData () {
        Debug.Log ("Commit Time: " + time);
        Debug.Log ("Commit Message: " + message);
        Debug.Log ("Commit Author: " + author);
        Debug.Log ("Commit Url: " + commit_url);
        Debug.Log ("Commit Author Avatar Url: " + avatar_url);     
        foreach (string url in parent_url) {
            Debug.Log ("Parent Url: " + url);
        }
        Debug.Log ("Commit sha: " + sha);           

        foreach (string sha in parent_sha) {
            Debug.Log ("Parent Sha " + sha);
        }   
    }
}