using System.Security.AccessControl;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using System.Net.Http;
using UnityEngine;
using HackTheHack;
using System.Linq;
using System.Net;
using System;

namespace HackTheHack
{
    /// <summary>
    /// JFuzz -> Adapting code to work within Unity from Azure provided scripts via a REST approach
    /// Singleton pattern - only have one per Unity scene!
    /// Converted this from https://github.com/Azure-Samples/cosmos-db-rest-samples/blob/main/Program.cs
    /// </summary>
    public class HTH_Azure : MonoBehaviour
    {
        public static HTH_Azure Instance { get; private set; }
        private HttpClient httpClient;
        public string baseUrl = "https://mitrealityhackcosmodb.documents.azure.com:443/";
        public string cosmosKey;

        public string dbRoot = "MITHTH2022";

        public string gitHubContainerID = "HTH_GitHub";
        public string discordIntroductionID = "HTH_Discord_Introduction";
        public string responseInfo = "";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else { 
                Instance = this; 
            }
        }
        
        public async void Start()
        {
            await TestAzureServices();
        }
        #region Test Methods
        async Task TestAzureServices()
        {
            httpClient = new HttpClient();
            await GetContainer(dbRoot, gitHubContainerID);
            /*
            var jsonContainerTemplate = $@"{{
""id"":""{discordIntroductionID}"",
 ""partitionKey"": {{  
    ""paths"": [
      ""/partitionKey""  
    ],  
    ""kind"": ""Hash"",
     ""Version"": 2
  }}  
}}";
            await CreateContainer(dbRoot, jsonContainerTemplate);
            //await CreateDatabase(dbRoot);
            //await ListContainers(dbBaseID);
            
                        await ListDatabases();
                        await GetContainer(dbBaseID, containerTest);

                        var jsonContainerTemplate = $@"{{
            ""id"":""{gitHubContainerID}"",
             ""partitionKey"": {{  
                ""paths"": [
                  ""/partitionKey""  
                ],  
                ""kind"": ""Hash"",
                 ""Version"": 2
              }}  
            }}";
                        await CreateContainer(dbBaseID, jsonContainerTemplate);
                        List<HTH_Message> TestMessages = new List<HTH_Message>();
                        DateTime curTime = DateTime.Now.ToUniversalTime();
                        TestMessages.Add(ReturnMessageExample());
                        var example = ReturnSoftwareExample(TestMessages);
                        var jsonD = JsonConvert.SerializeObject(example);
                        Debug.LogWarning($"JSON: {jsonD}");
                        await CreateSoftwareDocument(dbBaseID, gitHubContainerID, example.PartionKey, jsonD);
            */
        }

        /// <summary>
        /// Used to test and return a manual new HTH_Message
        /// </summary>
        /// <returns></returns>
        private HTH_Message ReturnDiscordExample()
        {
            DateTime curTime = DateTime.Now.ToUniversalTime();
            return new HTH_Discord()
            {
                MessageKey = "JFuzz.HTH_Bee." + curTime,
                SenderHandle = "JFuzz",
                ServiceName = HTH_Service.GitHub,
                ServiceReferenceID = "123456789",
                ServiceURL = "http://github.com/JShull",
                MessageTStamp = curTime,
                Mentions = new string[0],
                TeamName = "HTH_Bee",
                ReceiverHandle = "",
                Tags = new string[0],
                Processed = false,
                Thread = ""
            };
        }
        private HTH_GitHub ReturnGitHubExample()
        {
            DateTime curTime = DateTime.Now.ToUniversalTime();
            return new HTH_GitHub()
            {
                MessageKey = "JShull.HTH_Bee." + curTime,
                UserName = "JShull",
                Name = "John Shull",
                Sha = "cb699b899ca40a5d38aea31b437678da22e149e8",
                Avatar_Url = "https://avatars.githubusercontent.com/u/12342337?v=4",
                RepoName = "PuzzlePot",
                CommitMessage = "Puzzle-Pot Initialization * Getting the Puzzle - Pot Work up for distribution.* Added MIT License * Added link and compressed zip to release version 1.0.0",
                BranchName = "Main",
                CommitURL = "https://github.com/JShull/PuzzlePot/commit/cb699b899ca40a5d38aea31b437678da22e149e8",
                ParentUrl = "",
                ParentSha = "",
            };
        }
        /// <summary>
        /// Used to test and return a manual new HTH_Software 
        /// </summary>
        /// <param name="TestMessages"></param>
        /// <returns></returns>
        private HTH_Software ReturnSoftwareExample(List<HTH_Message> TestMessages)
        {
            HTH_Software testSoft = new HTH_Software()
            {
                TotalActions = 0,
                Id = "HTH_Bee.01",
                PartionKey = "HTH_Bee",
                ServiceName = HTH_Service.GitHub,
                PayLoad = TestMessages.ToArray(),
            };
            return testSoft;
        }
        #endregion
        async Task ListDatabases()
        {
            var method = HttpMethod.Get;
            var resourceLink = $"";
            var requestDateString = DateTime.UtcNow.ToString("r");
            var auth = GenerateMasterKeyAuthorizationSignature(method, "dbs", resourceLink, requestDateString, cosmosKey);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("authorization", auth);
            httpClient.DefaultRequestHeaders.Add("x-ms-date", requestDateString);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");

            var requestUri = new Uri($"{baseUrl}/dbs");
            var httpRequest = new HttpRequestMessage { Method = method, RequestUri = requestUri };
            var httpResponse = await httpClient.SendAsync(httpRequest);

            await ReportOutput($"List Databases:", httpResponse);
        }
        async Task GetDatabase(string databaseId)
        {
            var method = HttpMethod.Get;

            //var resourceType = ResourceType.dbs;
            var resourceLink = $"dbs/{databaseId}";
            var requestDateString = DateTime.UtcNow.ToString("r");
            var auth = GenerateMasterKeyAuthorizationSignature(method, "dbs", resourceLink, requestDateString, cosmosKey);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("authorization", auth);
            httpClient.DefaultRequestHeaders.Add("x-ms-date", requestDateString);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");

            var requestUri = new Uri($"{baseUrl}/{resourceLink}");
            var httpRequest = new HttpRequestMessage { Method = method, RequestUri = requestUri };

            var httpResponse = await httpClient.SendAsync(httpRequest);
            await ReportOutput($"Get Database with id: '{databaseId}' :", httpResponse);
        }
        async Task CreateDatabase(string databaseId)
        {
            var method = HttpMethod.Post;

            //var resourceType = ResourceType.DSObject;
            var resourceLink = $"";
            var requestDateString = DateTime.UtcNow.ToString("r");
            var auth = GenerateMasterKeyAuthorizationSignature(method, "dbs", resourceLink, requestDateString, cosmosKey);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("authorization", auth);
            httpClient.DefaultRequestHeaders.Add("x-ms-date", requestDateString);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
            httpClient.DefaultRequestHeaders.Add("x-ms-offer-throughput", "400");
            
            var requestUri = new Uri($"{baseUrl}/dbs");
            var requestBody = $"{{\"id\":\"{databaseId}\"}}";
            var requestContent = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
            Debug.LogWarning($"Request String: {requestContent}");
            var httpRequest = new HttpRequestMessage { Method = method, Content = requestContent, RequestUri = requestUri };

            var httpResponse = await httpClient.SendAsync(httpRequest);
            await ReportOutput($"Create Database with thoughput mode x-ms-offer-throughput, 400:", httpResponse);
        }
        /// <summary>
        /// Pull back the containers within a passed DB ID
        /// </summary>
        /// <param name="databaseId"></param>
        /// <returns></returns>
        async Task ListContainers(string databaseId)
        {
            var method = HttpMethod.Get;
            var resourceLink = $"dbs/{databaseId}/colls";
            var requestDateString = DateTime.UtcNow.ToString("r");
            var auth = GenerateMasterKeyAuthorizationSignature(method, "dbs", resourceLink, requestDateString, cosmosKey);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("authorization", auth);
            httpClient.DefaultRequestHeaders.Add("x-ms-date", requestDateString);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");

            var requestUri = new Uri($"{baseUrl}/{resourceLink}");
            var httpRequest = new HttpRequestMessage { Method = method, RequestUri = requestUri };
            var httpResponse = await httpClient.SendAsync(httpRequest);

            await ReportOutput($"List Containers:", httpResponse);
        }
        async Task GetContainer(string databaseId, string containerId)
        {
            var method = HttpMethod.Get;

            //var resourceType = ResourceType.colls;
            var resourceLink = $"dbs/{databaseId}/colls/{containerId}";
            var requestDateString = DateTime.UtcNow.ToString("r");
            var auth = GenerateMasterKeyAuthorizationSignature(method, "colls", resourceLink, requestDateString, cosmosKey);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("authorization", auth);
            httpClient.DefaultRequestHeaders.Add("x-ms-date", requestDateString);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");

            var requestUri = new Uri($"{baseUrl}/{resourceLink}");
            var httpRequest = new HttpRequestMessage { Method = method, RequestUri = requestUri };

            var httpResponse = await httpClient.SendAsync(httpRequest);
            await Response_GetContainer($"Get Container with id: '{databaseId}' :", httpResponse);
        }
        async Task Response_GetContainer(string methodName, HttpResponseMessage httpResponse)
        {
            
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            var ghResponse = new GitHubResponseArgs() { httpResponse = httpResponse, messageContent = responseContent, method=methodName };
            //inform our event handler...
            HTH_Publisher.Instance.Call_HTH_AzureGitHubResponse(ghResponse);
            /*
            if (httpResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"{methodName}: SUCCESS\n    {responseContent}\n\n");
                Debug.LogWarning($"{methodName}: SUCCESS\n    {responseContent}\n\n");
                responseInfo = $"{methodName}: SUCCESS\n    {responseContent}\n\n";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{methodName}: FAILED -> {(int)httpResponse.StatusCode}: {httpResponse.ReasonPhrase}.\n    {responseContent}\n\n");
                Console.ForegroundColor = ConsoleColor.White;
                Debug.LogWarning($"{methodName}: FAILED -> {(int)httpResponse.StatusCode}: {httpResponse.ReasonPhrase}.\n    {responseContent}\n\n");
                responseInfo = $"{methodName}: FAILED -> {(int)httpResponse.StatusCode}: {httpResponse.ReasonPhrase}.\n    {responseContent}\n\n";
            }
            */
        }

        /// <summary>
        /// Container ID is part of the requestBody see the example
        /// </summary>
        /// <param name="databaseId"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        async Task CreateContainer(string databaseId, string requestBody)
        {
            var method = HttpMethod.Post;
            var resourceLink = $"dbs/{databaseId}";
            var requestDateString = DateTime.UtcNow.ToString("r");
            var auth = GenerateMasterKeyAuthorizationSignature(method, "colls", resourceLink, requestDateString, cosmosKey);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("authorization", auth);
            httpClient.DefaultRequestHeaders.Add("x-ms-date", requestDateString);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
            httpClient.DefaultRequestHeaders.Add("x-ms-offer-throughput", "400");

            var requestUri = new Uri($"{baseUrl}/{resourceLink}/colls");
            var requestContent = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
            Debug.LogWarning($"request Body-> {requestBody}");
            var httpRequest = new HttpRequestMessage { Method = method, Content = requestContent, RequestUri = requestUri };

            var httpResponse = await httpClient.SendAsync(httpRequest);
            await ReportOutput($"Create Container with thoughput mode x-ms-offer-throughput:", httpResponse);
        }
        /// <summary>
        /// This is actually creating an entry into the system
        /// we are not currently checking to see if this exists - will be adding that to another function
        /// </summary>
        /// <param name="item"></param>
        /// <param name="databaseId"></param>
        /// <param name="containerId"></param>
        /// <returns></returns>
        async Task CreateSoftwareDocument(string databaseId, string containerId, string PartionKey, string jsonData)
        {
            var method = HttpMethod.Post;

            //var resourceType = ResourceType.docs;
            var resourceLink = $"dbs/{databaseId}/colls/{containerId}";
            var requestDateString = DateTime.UtcNow.ToString("r");
            var auth = GenerateMasterKeyAuthorizationSignature(method, "docs", resourceLink, requestDateString, cosmosKey);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("authorization", auth);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
            httpClient.DefaultRequestHeaders.Add("x-ms-date", requestDateString);
            httpClient.DefaultRequestHeaders.Add("x-ms-documentdb-is-upsert", "True");
            httpClient.DefaultRequestHeaders.Add("x-ms-documentdb-partitionkey", $"[\"{PartionKey}\"]");

            var requestUri = new Uri($"{baseUrl}/{resourceLink}/docs");

            var requestContent = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage { Method = method, Content = requestContent, RequestUri = requestUri };

            var httpResponse = await httpClient.SendAsync(httpRequest);
            await ReportOutput("Create Document", httpResponse);
        }
        /// <summary>
        /// This replaces a document on the table
        /// </summary>
        /// <param name="id">document id</param>
        /// <param name="databaseId"></param>
        /// <param name="containerId"></param>
        /// <param name="partionKey"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        async Task ReplaceDocument(string id, string databaseId, string containerId, string partionKey, string jsonData)
        {
            var method = HttpMethod.Put;

            var resourceLink = $"dbs/{databaseId}/colls/{containerId}/docs/{id}";
            var requestDateString = DateTime.UtcNow.ToString("r");
            var auth = GenerateMasterKeyAuthorizationSignature(method, "docs", resourceLink, requestDateString, cosmosKey);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("authorization", auth);
            httpClient.DefaultRequestHeaders.Add("x-ms-date", requestDateString);
            httpClient.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");
            httpClient.DefaultRequestHeaders.Add("x-ms-documentdb-partitionkey", $"[\"{partionKey}\"]");
            var requestUri = new Uri($"{baseUrl}/{resourceLink}");
            var requestContent = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            var httpRequest = new HttpRequestMessage { Method = method, Content = requestContent, RequestUri = requestUri };
            var httpResponse = await httpClient.SendAsync(httpRequest);
            await ReportOutput($"Replace Document with id '{id}'", httpResponse);
        }
        #region Azure Key Signature requirements
        
        string GenerateMasterKeyAuthorizationSignature(HttpMethod verb, string resourceType, string resourceLink, string date, string key)
        {
            var keyType = "master";
            var tokenVersion = "1.0";
            var payload = $"{verb.ToString().ToLowerInvariant()}\n{resourceType.ToLowerInvariant()}\n{resourceLink}\n{date.ToLowerInvariant()}\n\n";

            var hmacSha256 = new System.Security.Cryptography.HMACSHA256 { Key = Convert.FromBase64String(key) };
            var hashPayload = hmacSha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
            var signature = Convert.ToBase64String(hashPayload);
            var authSet = WebUtility.UrlEncode($"type={keyType}&ver={tokenVersion}&sig={signature}");

            return authSet;
        }
        #endregion
        /// <summary>
        /// Response Codes
        /// If we didn't make it, will return fail code and information to the log as well as our local variable - for debugging
        /// Will write new return methods to match our get/post after I get them all flushed out
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        async Task ReportOutput(string methodName, HttpResponseMessage httpResponse)
        {
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"{methodName}: SUCCESS\n    {responseContent}\n\n");
                Debug.LogWarning($"{methodName}: SUCCESS\n    {responseContent}\n\n");
                responseInfo = $"{methodName}: SUCCESS\n    {responseContent}\n\n";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{methodName}: FAILED -> {(int)httpResponse.StatusCode}: {httpResponse.ReasonPhrase}.\n    {responseContent}\n\n");
                Console.ForegroundColor = ConsoleColor.White;
                Debug.LogWarning($"{methodName}: FAILED -> {(int)httpResponse.StatusCode}: {httpResponse.ReasonPhrase}.\n    {responseContent}\n\n");
                responseInfo = $"{methodName}: FAILED -> {(int)httpResponse.StatusCode}: {httpResponse.ReasonPhrase}.\n    {responseContent}\n\n";
            }
        }
    }

}
