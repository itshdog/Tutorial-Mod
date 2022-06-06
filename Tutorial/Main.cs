using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Reflection;
using System.IO;
using Tutorial.Base_Classes;
using UnityEngine;

public static class ConfigManager
{
    private static ConfigFile configFileA;
    private static ConfigFile configFileB;
    private const string configFolderName = "myConfigFolder";
    private static string ConfigFolderPath { get => System.IO.Path.Combine(Paths.ConfigPath, configFolderName); }

    private static void Init()
    {
        // Ensures that the config folder exists, if not, create it.
        if (!Directory.Exists(ConfigFolderPath))
        {
            Directory.CreateDirectory(ConfigFolderPath);
        }
        //Create the config files, notice how we use Path.Combine for creating the new files.            
        configFileA = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, "ConfigFileA.CFG"), true);
        configFileB = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, "ConfigFileB.CFG"), true);
    }
}

namespace Tutorial
{
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [R2APISubmoduleDependency(nameof(ResourcesAPI), nameof(LanguageAPI), nameof(UnlockableAPI), nameof(ItemAPI))]

    public class Main : BaseUnityPlugin
    {

        //"com.USERNAME.MODNAME"
        public const string ModGUID = "com.itshdog.tutorial";
        public const string ModName = "itshdog Tutorial";
        //"MAJORVERSION.MINORPATCH.BUGFIX". Ex: 1.2.3 is Major Release 1, Patch 2, Bug Fix 3.
        public const string ModVersion = "0.0.1";
        public static AssetBundle Assets;
        public static ConfigEntry<string> CommandoSlideMessage { get; set; }
        public static ConfigEntry<string> CommandoShootMessage { get; set; }

        public void Awake()
        {
            // Initial Load Message
            Chat.AddMessage("Mr.Sarcasm v0.0.1 Loaded!");

            // Config
            CommandoSlideMessage = Config.Bind<string>(
                "Commando",
                "SlideMessage",
                "*Gracefully slides ass across the ground*",
                "This is the message which is played when SlideState.OnEnter is called"
            );

            CommandoShootMessage = Config.Bind<string>(
                "Commando",
                "ShootMessage",
                "Pew! Pew! Pew!",
                "This is the message which is played when FirePistol2.OnEnter is called"
            );

            // Main
            var rand = new System.Random();

            On.EntityStates.Commando.SlideState.OnEnter += (orig, self) =>
            {
                int chance = rand.Next(1, 101);

                if (chance <= 25)
                {
                    ChatMessage.SendColored(CommandoSlideMessage.Value, Color.gray);
                    orig(self);
                } else
                {
                    orig(self);
                }
            };

            On.EntityStates.Commando.CommandoWeapon.FirePistol2.OnEnter += (orig, self) =>
            {
                int chance = rand.Next(1, 101);

                if (chance <= 5)
                {
                    ChatMessage.SendColored(CommandoShootMessage.Value, Color.grey);
                    orig(self);
                } else
                {
                    orig(self);
                }
            };

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

        private void PickupDropletController_CreatePickupDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, UnityEngine.Vector3 position, UnityEngine.Vector3 velocity)
        {
            if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.Hoof.itemIndex))
            {
                Chat.AddMessage("Goat lookin' ass LMAOO");
                ChatMessage.Send("GOTTA RUN FAST!");
            }
            orig(pickupIndex, position, velocity);
        }

        private void FirePistol2_FireBullet(On.EntityStates.Commando.CommandoWeapon.FirePistol2.orig_FireBullet orig, EntityStates.Commando.CommandoWeapon.FirePistol2 self, string targetMuzzle)
        {
            throw new NotImplementedException();
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
    }
}
