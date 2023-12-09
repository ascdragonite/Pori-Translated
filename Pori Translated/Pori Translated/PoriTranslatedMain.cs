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
using Il2CppSystem.Runtime.Remoting.Messaging;
using UnityEngine;
using static Il2Cpp.AsyncRaycast;
using static Il2Cpp.CageMath;

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
                    LoggerInstance.Warning(translatedMessageProvider.name + ":");

                    List<string> TextList = new List<string>();
                    foreach (TranslatedMessageProvider.MessageItem messageItem in translatedMessageProvider.Messages)
                    {
                        TextList.Add("- " + messageItem.English);
                        LoggerInstance.Msg("- " + messageItem.English);
                    }
                    TextList.Add("");
                    DialogList.Add(translatedMessageProvider.name + ":", TextList.ToArray());
                }
            }

        }


        public override void OnApplicationQuit()
        {
            int origCount = 0;
            Dictionary<string, string[]> result;
            try
            {
                GenerateDict(File.ReadAllText("Mods/DialogList.yaml"));
                origCount = HeaderDict.Count;
                foreach (var kvp in DialogList)
                {
                    if (!HeaderDict.ContainsKey(kvp.Key))
                    {
                        HeaderDict.Add(kvp.Key, kvp.Value);
                        LoggerInstance.Msg(kvp.Key);
                        LoggerInstance.Msg(kvp.Value.ToString());
                    }
                }

                result = HeaderDict;
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
                    foreach (string text in kvp.Value)
                    {
                        writer.WriteLine(text);
                    }
                }
            }
            LoggerInstance.Msg($"Saved +{result.Count - origCount} entries");
            LoggerInstance.Msg($"Total entries: {result.Count}");
            LoggerInstance.Msg("DIALOG EXTRACTED SUCCESSFULLY");
        }


        //[ascdragonite]: from here on is shadowofcat's code with some modifications

        public void ListDict(Dictionary<string, string[]> dict)
        {
            LoggerInstance.BigError("[][][] START OF DICTIONARY [][][]");
            foreach (var kvp in dict)
            {
                LoggerInstance.Warning(kvp.Key);
                foreach (string text in kvp.Value)
                {
                    LoggerInstance.Msg(text);
                }
            }
            LoggerInstance.BigError("[][][] END OF DICTIONARY [][][]");
        }

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
                    headers.Add(line.Trim());
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
                if (line.Trim() == headerName)
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
                elements.Add("");
            }
            return elements.ToArray();
        }


    }



}