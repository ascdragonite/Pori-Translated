using System;
using Il2Cpp;
using Il2CppGrdk;
using MelonLoader;
using UnityEngine.SceneManagement;
using UniverseLib;
using Il2CppInterop.Runtime;
using Harmony;
using Il2CppCore;
using System.Net;

namespace Pori_Translated
{
    public class PoriTranslatedMain : MelonMod
    {
        Dictionary<string, string[]> HeaderDict = new Dictionary<string, string[]>();
        Dictionary<string, string[]> DialogList = new Dictionary<string, string[]>();
        List<string> HeaderList = new List<string>();
        public override void OnInitializeMelon()
        {
            MelonHandler.LoadFromFile("UserLibs/UniverseLib.IL2CPP.Interop.ML.dll", null);

            LoggerInstance.Msg("hiii :3");

            
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            
            foreach (TranslatedMessageProvider translatedMessageProvider in RuntimeHelper.FindObjectsOfTypeAll<TranslatedMessageProvider>())
            {
                if (!HeaderList.Contains(translatedMessageProvider.name + ":"))
                {
                    HeaderList.Add(translatedMessageProvider.name + ":");
                    base.LoggerInstance.Msg(translatedMessageProvider.name + ":");

                    List<string> TextList = new List<string>();
                    foreach (TranslatedMessageProvider.MessageItem messageItem in translatedMessageProvider.Messages)
                    {
                        TextList.Add("- " + messageItem.English);
                        base.LoggerInstance.Msg("- " + messageItem.English);
                    }

                    DialogList.Add(translatedMessageProvider.name + ":", TextList.ToArray());
                }
            }

        }


        public override void OnApplicationQuit()
        {
            Dictionary<string, string[]> result;
            try
            {
                GenerateDict("Mods/DialogList.yaml");
                result = DialogList.Union(HeaderDict.Where(k => !DialogList.ContainsKey(k.Key))).ToDictionary(k => k.Key, v => v.Value);
            }
            catch 
            {
                LoggerInstance.Msg("DialogList not created yet!!!");
                result = DialogList;
            }

            using (StreamWriter writer = new StreamWriter("Mods/DialogList.yaml"))
            {
                foreach (KeyValuePair<string, string[]> kvp in result)
                {
                    writer.WriteLine(kvp.Key);
                    foreach(string text in kvp.Value)
                    {
                        writer.WriteLine(text);
                    }
                }
            }
            LoggerInstance.Msg("DIALOG EXTRACTED SUCCESSFULLY");
        }


        //[ascdragonite] from here on is shadowofcat's code


        public void GenerateDict(string Database)
        {
            HeaderDict.Clear();

            string[] HeaderNames = GetHeaders(Database);

            foreach (string header in headers)
            {
                string[] elements = GetElements(Database, header);

                if (!HeaderDict.ContainsKey(header))
                {
                    HeaderDict.Add(header, elements);
                }
            }
            LoggerInstance.Msg("Read from existing DialogList");
        }


        //this part gets all the headers
        // the headers are the names of the message providers

        List<string> headers = new List<string>();
        string[] GetHeaders(string yamlContent)
        {

            string[] lines = yamlContent.Split('\n');

            headers.Clear();

            foreach (var line in lines)
            {
                // Check if the line contains a header
                if (line.Trim().EndsWith(":"))
                {
                    // Debug statement to print the header
                    string header = line.Replace(":", "");
                    header = header.Trim();

                    headers.Add(header);
                }
            }
            return headers.ToArray();
        }


        //and this part gets all the elements out of a header


        static string[] GetElements(string yamlContent, string headerName)
        {
            List<string> elements = new List<string>();

            string[] lines = yamlContent.Split('\n');
            bool insideHeader = false;

            foreach (var line in lines)
            {
                // Check if the line contains the header name
                if (line.Trim() == headerName + ":")
                {
                    insideHeader = true;
                }
                else if (insideHeader && !string.IsNullOrWhiteSpace(line))
                {
                    // Check if the line is not empty or a comment
                    // Add the element to the list
                    elements.Add(line.Trim());
                }
                else if (insideHeader && string.IsNullOrWhiteSpace(line))
                {
                    // If an empty line is encountered, stop processing the header
                    break;
                }
            }

            for (int i = 0; i < elements.ToList().Count; i++)
            {
                elements[i] = elements[i].Replace('-', '\0');
                elements[i] = elements[i].Trim();
            }

            return elements.ToArray();
        }


    }



}