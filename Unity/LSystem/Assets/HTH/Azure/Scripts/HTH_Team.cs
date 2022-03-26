using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;
/// <summary>
/// Created: John Shull
/// Team: Hack the Hack
/// Date: 3/24/2022 EST
/// </summary>
namespace HackTheHack
{
    /// <summary>
    /// Core structs/classes for simple data classes tied to individuals
    /// Dependencies are Netwonsoft
    /// </summary>
    public class HTH_Team 
    {
        //JFuzz -> Key is going to be the team name in lowercase followed by '.' followed by a team number
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }
        //JFuzz -> Partition will be just the team name
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartionKey { get; set; }
        //JFuzz -> place to dump additional comments/notes maybe from some other data stream we aren't aware of yet
        public string Notes { get; set; }
        //JFuzz -> they are done with the hackathon
        public bool Finished { get; set; }
        //JFuzz -> Array of Team Members
        public HTH_Member[] Members { get; set; }
        
    }
    /// <summary>
    /// Want to lock down roles
    /// </summary>
    public enum HTH_Role
    {
        Hacker = 0,
        Mentor = 1,
        HTHacker = 2,
        MITStaff = 3 ,
    }
    /// <summary>
    /// Want to lock down names of software and services
    /// </summary>
    public enum HTH_Service
    {
        Discord = 0,
        GitHub = 1,
        Notion = 2,
        Gather = 3,
        Miro = 4,
    }
    /// <summary>
    /// Storing all individual information and a reference back to the team
    /// This is mainly going to be used as a lookup reference to pull information from
    /// DB Write Frequence: once - on setup
    /// DB Read Frequency: a lot - consider removing details not tied to handles
    /// </summary>
    public class HTH_Member
    {
        //JFuzz -> this references back to our PartionKey for cross reference tied to HTH_Team if needed later
        public string TeamName { get; set; }
        //JFuzz -> discord Handle followed by '.' followed by number e.g. TheJohnnyFuzz#4164 becomes = TheJohnnyFuzz.4164
        public string DiscordHandle { get; set; }
        //JFuzz -> Role of our participant
        public HTH_Role Role { get; set; }
        //JFuzz -> Email
        public string Email { get; set; }
        //JFuzz -> GitHub Handle
        public string GitHubHandle { get; set; }
        //JFuzz -> Notion Handle
        public string NotionHandle { get; set; }
        //JFuzz -> Gather Handle
        public string GatherHandle { get; set; }
        //JFuzz -> Sign up information
        public DateTime RegistrationDate { get; set; }
        //JFuzz -> First Time Reality Hacker?
        public bool FirstTimeMITHacker { get; set; }
        //JFuzz -> Main Textual Data
        public HTH_Software[] AllSoftwareData { get; set; }

    }
    /// <summary>
    /// High level aggregate data by services/software, use derived classes to represent different services or not
    /// DB Write Frequency: a lot, highest table/class we are going to hit
    /// DB Read Frequency: depends, when we generate visuals and/or need to run aggregate services
    /// </summary>
    public class HTH_Software
    {
        //JFuzz -> Partition Key = team name
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        //JFuzz -> Partition will be just the team name
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartionKey { get; set; }
        public string TeamName { get; set; }
        //JFuzz -> Service that we can filter against
        public HTH_Service ServiceName { get; set; }
        //JFuzz -> running total across this service for all items we pull back e.g. I sent a message on a thread of interest ++1
        public int TotalActions { get; set; }
        //JFuzz -> Array of information via textual information
        public HTH_Message[] PayLoad { get; set; }
    }
    /// <summary>
    /// This is going to be the most accessed bits of information across services for NLP
    /// This also can be done in a lot of 'better ways' just going to try and keep it simple
    /// future ideas could involve a graph based storage class and/or direct classes/structs to direct service SDKS
    /// Also could just dump the entire JSON/XML of said message service in a field tied to the class
    /// I'm going to duplicate some data so we can store this in a separate set of tables by services
    /// not sure if this will be broken out into independent tables by services (probably)
    /// </summary>

    public class HTH_Message
    {
        //JFuzz -> Primary Key = primary handle + "." + TeamName + "." + UTC timestamp DateTime
        public string MessageKey { get; set; }
        //JFuzz -> Will be a partition key
        public string TeamName{ get; set;}
        //JFuzz -> Another Partition
        public HTH_Service ServiceName { get; set; }
        //JFuzz -> Time information in UTC
        public DateTime MessageTStamp{ get; set;}
        
        
    }
    /// <summary>
    /// Commit Data from GitHub
    /// This is to align with the work by the other data collection team
    /// </summary>
    public class HTH_GitHub: HTH_Message
    {
        //JFuzz -> GitHub Name
        public string UserName;
        //JFuzz -> Author Name
        public string Name;
        //JFuzz -> commit sha
        public string Sha;
        //JFuzz -> The main payload we care about
        public string CommitMessage;
        //JFuzz -> If we needed this for other information
        public string CommitURL;
        //JFuzz -> Link to Avatar URL
        public string Avatar_Url;
        //JFuzz -> Main RepoName
        public string RepoName;
        //JFuzz -> The branch that this commit was on
        public string BranchName;
        //JFuzz -> Parent HTTP URL
        public string ParentUrl;
        //JFuzz -> Parent SHA for look up needs
        public string ParentSha;
       
        
        //start with the last item in the list, sort backwards, when I hit a duplicate kick out
        //convert and remove duplicates from the commit list
        //githubDataDict <string,List<commitData>>
    }

    public class HTH_Discord : HTH_Message
    {
        //JFuzz -> primary sender
        public string SenderHandle { get; set; }
        //JFuzz -> possible receiver - this might be 'none' if just posted to a running thread, but if it's a reply, this isn't a mention
        public string ReceiverHandle { get; set; }
        //JFuzz -> Thread information - e.g. in discord, where is this message?
        public string Thread { get; set; }
        //JFuzz -> Mentions array - e.g. in discord, multiple @'s in discord
        public string[] Mentions { get; set; }
        //JFuzz -> Tag array - e.g. if the service provides ways to 'tag' as defined by the service
        public string[] Tags { get; set; }
        //JFuzz -> Different services might allow us to store differnet factors, e.g. an item # for a GitHub commit
        public string ServiceReferenceID { get; set; }
        //JFuzz -> Has this message already been processed, e.g. if we are running NLP services and storing those results elsewhere we don't want to process this message again
        public bool Processed { get; set; }
        //JFuzz -> Area for added information by service, maybe a way to quickly pull this message again from the service SDK, could be a URL, or some unique hash that = original message data
        //very useful for later needs and/or maybe quicker ways to access the data from those services vs. pulling our db everytime - very situational
        public string ServiceURL { get; set; }
    }


}
