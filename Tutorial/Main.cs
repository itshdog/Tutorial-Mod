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
        public const string ModName = "Mr. Sarcasm";
        //"MAJORVERSION.MINORPATCH.BUGFIX". Ex: 1.2.3 is Major Release 1, Patch 2, Bug Fix 3.
        public const string ModVersion = "0.0.2";
        public static AssetBundle Assets;
        public static ConfigEntry<string> MrSarcasmName { get; set; }
        public static ConfigEntry<string> CommandoSlideMessage { get; set; }
        public static ConfigEntry<string> CommandoShootMessage { get; set; }
        public static ConfigEntry<int> WhiteItemDrop { get; set; }
        public static ConfigEntry<int> GreenItemDrop { get; set; }
        public static ConfigEntry<int> RedItemDrop { get; set; }

        // Generates "sArCaStIc" messages
        public String sarcastic(string input)
        {
            string output = string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                if (i%2 == 1)
                {
                    output += char.ToLower(input[i]);
                }
                else
                {
                    output += char.ToUpper(input[i]);
                }
            }
            return output;
        }

        public void Awake()
        {
            // Initial Load Message
            Chat.AddMessage("Mr.Sarcasm v0.0.1 Loaded!");

            // Config
            MrSarcasmName = Config.Bind<string>(
                "Mr. Sarcasm",
                "Name",
                "Mr. Sarcasm",
                "The name on chat messages when Mr. Sarcasm is called."
            );
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
            WhiteItemDrop = Config.Bind<int>(
                "Drop Message Chance",
                "White Items",
                10,
                "The chance (X/100) for getting a sarcastic message"
            );
            GreenItemDrop = Config.Bind<int>(
                "Drop Message Chance",
                "Green Items",
                25,
                "The chance (X/100) for getting a sarcastic message"
            );
            RedItemDrop = Config.Bind<int>(
                "Drop Message Chance",
                "Red Items",
                100,
                "The chance (X/100) for getting a sarcastic message"
            );

            // Main
            var rand = new System.Random();

            On.EntityStates.Commando.SlideState.OnEnter += (orig, self) =>
            {
                int chance = rand.Next(1, 101);

                if (chance <= 10)
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

                if (chance <= 2)
                {
                    ChatMessage.SendColored(CommandoShootMessage.Value, Color.grey);
                    orig(self);
                } else
                {
                    orig(self);
                }
            };

            On.RoR2.PickupDropletController.CreatePickupDroplet += PickupDropletController_CreatePickupDroplet;

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
            var rand = new System.Random();
            int chance = rand.Next(1, 101);

            // White Items
            if (chance <= WhiteItemDrop.Value)
            {
                // Personal Shield Generator
                if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.PersonalShield.itemIndex))
                {
                    ChatMessage.Send("Yeah, PSG is more your speed LOL", MrSarcasmName.Value);
                }
                // Goat Hoof
                if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.Hoof.itemIndex))
                {
                    ChatMessage.Send("You might need this, you're a bit slow", MrSarcasmName.Value);
                }
                // Replusion Armor Plate
                if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.ArmorPlate.itemIndex))
                {
                    ChatMessage.Send("I saw you lost a lot of health before, so take an Armor Plate", MrSarcasmName.Value);
                }
                // Mocha
                if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.BoostAttackSpeed.itemIndex))
                {
                    ChatMessage.Send("i NeEd MoChA tO bE gOoD", MrSarcasmName.Value);
                }
                // Topaz Brooch
                if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.BarrierOnKill.itemIndex))
                {
                    ChatMessage.Send(sarcastic(RoR2Content.Items.BarrierOnKill.nameToken), MrSarcasmName.Value);
                }
            }

            // Green Items
            if (chance <= GreenItemDrop.Value)
            {
                if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.ChainLightning.itemIndex))
                {
                    ChatMessage.Send(sarcastic(RoR2Content.Items.ChainLightning.nameToken), MrSarcasmName.Value);
                }
            }

            // Red Items
            if (chance <= RedItemDrop.Value)
            {
                if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Items.Clover.itemIndex))
                {
                    name = RoR2.Language.GetString(pickupIndex.GetPickupNameToken());
                    ChatMessage.Send(sarcastic(name), MrSarcasmName.Value);
                }
            }

            // Lunar Items

            // Void Items

            // Boss Items

            // Equipment

            // Extra Roll Chance!
            if (chance >= 97)
            {
                ChatMessage.Send("Ok, I'm feeling a little generous today...", MrSarcasmName.Value);
                orig(pickupIndex, position, velocity);
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
