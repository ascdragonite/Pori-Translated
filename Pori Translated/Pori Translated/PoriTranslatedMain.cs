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
        List<string> DialogList = new List<string>();
        public override void OnInitializeMelon()
        {
            MelonHandler.LoadFromFile("UserLibs/UniverseLib.IL2CPP.Interop.ML.dll", null);

            LoggerInstance.Msg("hiii :3");

            
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            
            foreach (TranslatedMessageProvider translatedMessageProvider in RuntimeHelper.FindObjectsOfTypeAll<TranslatedMessageProvider>())
            {
                if (!DialogList.Contains(translatedMessageProvider.name + ":"))
                {
                    DialogList.Add(translatedMessageProvider.name + ":");
                    base.LoggerInstance.Msg(translatedMessageProvider.name + ":");
                    foreach (TranslatedMessageProvider.MessageItem messageItem in translatedMessageProvider.Messages)
                    {
                        if (!DialogList.Contains("- " + messageItem.English))
                        {
                            DialogList.Add("- " + messageItem.English);
                            base.LoggerInstance.Msg("- " + messageItem.English);
                        }
                    }
                }
            }

        }


        public override void OnApplicationQuit()
        {
            System.IO.File.WriteAllLines("Mods/DialogList.txt", DialogList);
        }

    }
    
}