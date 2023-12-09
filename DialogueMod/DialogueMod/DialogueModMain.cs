﻿using System.Collections.Generic;
using System.Linq;
using Il2Cpp;
using MelonLoader;

namespace DialogueMod
{
    public class DialogueModMain : MelonMod
    {

        string Translation;

        public override void OnApplicationStart()
        {
            MelonHandler.LoadFromFile("UserLibs/UniverseLib.IL2CPP.Interop.ML.dll");

            UniverseLib.Universe.Init();

            Translation = System.IO.File.ReadAllText("Mods/Translation.YAML");

            GenerateDict(Translation);
        }

        Dictionary<string, string[]> HeaderDict = new Dictionary<string, string[]>();

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {


            foreach (TranslatedMessageProvider provider in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll<TranslatedMessageProvider>())
            {
                if (HeaderDict.ContainsKey(provider.name))
                {
                    int index = 0;

                    foreach (var dialogue in provider.Messages)
                    {
                        if (index <= HeaderDict[provider.name].Length - 1)
                        {
                            dialogue.English = HeaderDict[provider.name][index];
                        }

                        index++;
                    }
                }
            }



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
        }


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
                elements[i] = elements[i].Replace('-','\0');
                elements[i] = elements[i].Trim();
            }

            return elements.ToArray();
        }

    }
}
