using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
namespace HackTheHack
{
    /// <summary>
    /// Help load the data into Unity 
    /// </summary>
    public class HTH_Discord_Load : MonoBehaviour
    {
        public HTH_Discord_Data DiscordData;
        private HTH_Discord_Raw rawDiscordData;
        public List<string> DiscordHandles = new List<string>();
        public List<string> DiscordUnique = new List<string>();
        public List<string> SpecialDiscord = new List<string>();
        public Dictionary<string, Dictionary<string, int>> Mentions = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, int> MessageTally = new Dictionary<string, int>();
        public Dictionary<string, List<string>> DiscordMessagesByUser = new Dictionary<string, List<string>>();
        private void Start()
        {
            ProcessDiscordData();
            //TestMultipleMessages();
        }

        public void ProcessDiscordData()
        {
            
            rawDiscordData = JsonConvert.DeserializeObject<HTH_Discord_Raw>(DiscordData.RawJSON);

            Debug.LogWarning($"Number in array {rawDiscordData.undefined.Length}");
            DiscordHandles = DiscordData.DiscordHandles;
            DiscordUnique = DiscordData.DiscrodIP;
            SpecialDiscord = DiscordData.SpecialHandles;
            for (int i = 0; i < rawDiscordData.undefined.Length; i++)
            {
                var dText = rawDiscordData.undefined[i];
                if (!DiscordMessagesByUser.ContainsKey(dText.AuthorTag))
                {
                    List<string> tempList = new List<string>();
                    tempList.Add(dText.Text);
                    DiscordMessagesByUser.Add(dText.AuthorTag, tempList);
                }
                else
                {
                    //get list
                    var curTextList = DiscordMessagesByUser[dText.AuthorTag];
                    curTextList.Add(dText.Text);
                    DiscordMessagesByUser[dText.AuthorTag] = curTextList;
                }
                if (MessageTally.ContainsKey(dText.AuthorTag))
                {
                    MessageTally[dText.AuthorTag]++;
                }
                else
                {
                    MessageTally.Add(dText.AuthorTag, 1);
                }
                //determine if there is a mention
                bool foundMatch = false;
                var listOfMentions = MentionTagFound(dText.Text, ref foundMatch);
                if (listOfMentions.Count > 0)
                {
                    //process matches
                    
                    if (Mentions.ContainsKey(dText.AuthorTag))
                    {
                        var innerDictionary = Mentions[dText.AuthorTag];
                        for (int x = 0; x < listOfMentions.Count; x++)
                        {
                            var mentionName = listOfMentions[x];
                            if (innerDictionary.ContainsKey(mentionName))
                            {
                                innerDictionary[mentionName]++;
                            }
                            else
                            {
                                innerDictionary.Add(mentionName, 1);
                            }
                        }
                        Mentions[dText.AuthorTag] = innerDictionary;
                    }
                    else
                    {
                        Dictionary<string, int> newInnerDictionary = new Dictionary<string, int>();
                        for (int y = 0; y < listOfMentions.Count; y++)
                        {
                            var mentionName = listOfMentions[y];
                            if (newInnerDictionary.ContainsKey(mentionName))
                            {
                                newInnerDictionary[mentionName]++;
                            }
                            else
                            {
                                newInnerDictionary.Add(mentionName, 1);
                            }
                        }
                        Mentions.Add(dText.AuthorTag, newInnerDictionary);
                    }

                }
               
                    
                
            }
            Debug.LogWarning($"Number Discord Users {DiscordMessagesByUser.Keys.Count}");
            for (int a=0;a< DiscordHandles.Count; a++)
            {
                var userKey = DiscordHandles[a];
                QueryMentionsByUser(userKey);
            }
        }
        public void QueryMentionsByUser(string AuthorHandle)
        {
            if (Mentions.ContainsKey(AuthorHandle))
            {
                var keys = Mentions[AuthorHandle].Keys.ToList();
                for(int i = 0; i < keys.Count; i++)
                {
                    var otherHandle = keys[i];
                    var handleCounts = Mentions[AuthorHandle][otherHandle];
                    if (otherHandle== SpecialDiscord[0])
                    {
                        Debug.LogWarning($"Author: {AuthorHandle}, mentioned {otherHandle}, {handleCounts} times");
                    }
                    else
                    {
                        var intPlacement = DiscordUnique.IndexOf(otherHandle);
                        Debug.LogWarning($"Author: {AuthorHandle}, mentioned {DiscordHandles[intPlacement]}, {handleCounts} times");
                    }
                    
                }
            }
        }
        public void TestMultipleMessages()
        {
            bool mentionBool = false;
            var returnList = MentionTagFound("<@!414820778719313941> <@!470269079413063690>  I added malcolm's bot into our server.\nHere's a recent chat i had with him, <@!414820778719313941>  if you were interested in using that somehow", ref mentionBool);
            for(int i = 0; i < returnList.Count; i++)
            {
                Debug.LogWarning($"Return List Test: {returnList[i]}");
            }
        }
        private List<string> MentionTagFound(string message, ref bool foundMention)
        {
            //process
            List<string>mentions = new List<string>();
            string[] words = message.Split(' ');
            for(int i = 0; i < words.Length; i++)
            {
                for(int k=0;k< SpecialDiscord.Count; k++)
                {
                    if (words[i] == SpecialDiscord[k])
                    {
                        mentions.Add(SpecialDiscord[k]);
                        foundMention = true;
                    }
                }
                for(int j=0;j< DiscordUnique.Count; j++)
                {
                    if (words[i] == DiscordUnique[j])
                    {
                        mentions.Add(DiscordUnique[j]);
                        foundMention = true;
                    }
                }
            }
            return mentions;
        }
    }

}
