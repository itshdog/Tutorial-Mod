﻿using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using System;
using System.Reflection;
using Tutorial.Base_Classes;
using UnityEngine;



//automatically renamed based on project name.
namespace Tutorial
{
    //--------------R2API dependency. This template is heavily based on the modules provided by this API, so it uses it as a dependency.--------------------
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]

    //add other mod dependencies here using the format listed above.

    //------------------Makes mod server-side. Change this only if you're sure it won't fuck something up-----------------------
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    //------------------Defines your mod's GUID, Name, and Version to BepIn. These are set in variables later.---------------------------
    [BepInPlugin(ModGUID, ModName, ModVersion)]

    //------------------This line adds R2API dependencies required. ONLY ADD THE ONES YOU NEED. If you need to know which ones you need, check the R2API documentation----------
    [R2APISubmoduleDependency(nameof(ResourcesAPI), nameof(LanguageAPI), nameof(UnlockableAPI), nameof(ItemAPI))]

    //main class where the most basic stuff happens. Basically only here to activate the mod.
    public class Main : BaseUnityPlugin
    {

        //define mod ID: uses the format of "com.USERNAME.MODNAME"
        public const string ModGUID = "com.itshdog.tutorial";

        //define the mod name inside quotes. Can be anything.
        public const string ModName = "itshdog Tutorial";

        //define mod version inside quotes. Follows format of "MAJORVERSION.MINORPATCH.BUGFIX". Ex: 1.2.3 is Major Release 1, Patch 2, Bug Fix 3.
        public const string ModVersion = "0.0.1";

        //Creates an asset bundle that can be easily accessed from other classes
        public static AssetBundle Assets;

        //List other necessary variables and bits here. For example, you may need a list of all your new things to add them to the game properly.

        //this method runs when your mod is loaded.
        public void Awake()
        {

            //loads an asset bundle if one exists. Objects will need to be called from this bundle using AssetBundle.LoadAsset(string path)
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tutorial.mod_assets"))
            {

                if (stream != null)
                {

                    Assets = AssetBundle.LoadFromStream(stream);

                }

            }

            //runs our configs
            Configs();

            //this method will instantiate everything we want to add to the game. see below
            Instantiate();

            //runs hooks that are seperate from all additions (i.e, if you need to call something when the game runs or at special times)
            Hooks();

        }

        public void Configs()
        {

            //insert configs here

        }

        public void Hooks()
        {

            //insert hooks here

        }

        //we make calls to Verify on each thing here to make our call in Awake clean
        public void Instantiate()
        {

            VerifyItems(new Examples.EXAMPLE_ITEM());
            VerifyAchievements(new Examples.EXAMPLE_ACHIEVEMENT());

        }

        //this method will instantiate our items based on a generated config option
        public void VerifyItems(ItemBase item)
        {

            //generates a config file to turn the item on or off and get its value
            var isEnabled = Config.Bind<bool>("Items", "enable" + item.ItemName, true, "Enable this item in game? Default: true").Value;

            //checks to see if the config is enabled
            if (isEnabled)
            {

                //if the item is activated, instantiates the item
                item.Init(base.Config);

            }

        }

        //this method will instantiate our achievements based on a generated config option
        public void VerifyAchievements(AchievementBase achievement)
        {

            var isEnabled = Config.Bind<bool>("Items", "enable" + achievement.AchievementNameToken, true, "Enable this achievement in game? Default: true").Value;

            if (isEnabled)
            {

                achievement.Init(base.Config);

            }

        }

        //place other necessary methods below

    }

}