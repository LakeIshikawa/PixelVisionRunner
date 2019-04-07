﻿//   
// Copyright (c) Jesse Freeman, Pixel Vision 8. All rights reserved.  
//  
// Licensed under the Microsoft Public License (MS-PL) except for a few
// portions of the code. See LICENSE file in the project root for full 
// license information. Third-party libraries used by Pixel Vision 8 are 
// under their own licenses. Please refer to those libraries for details 
// on the license they use.
// 
// Contributors
// --------------------------------------------------------
// This is the official list of Pixel Vision 8 contributors:
//  
// Jesse Freeman - @JesseFreeman
// Christina-Antoinette Neofotistou @CastPixel
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// Shawn Rakowski - @shwany
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PixelVision8.Engine;
using PixelVision8.Engine.Chips;
using PixelVision8.Runner.Data;
using PixelVision8.Runner.Services;
using SharpFileSystem;
using SharpFileSystem.FileSystems;
using PixelVision8.Runner.Exporters;
using PixelVision8.Runner.Utils;
using Directory = System.IO.Directory;

namespace PixelVision8.Runner
{
    
    public enum CRTBiosSettings
    {
        CRT,
        Brightness,
        Sharpness,
        CRTEffectPath
    }
    
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class DesktopRunner : RunnerGame
    {
// Store the path to the game's files
        private IControllerChip controllerChip;
        public BiosService bios;
        public WorkspaceService workspaceService;
        protected string rootPath;
        protected FileSystemPath userBiosPath => FileSystemPath.Parse("/Storage/user-bios.json");
        protected string tmpPath;
        /// <summary>
        ///     This constructor saves the path to the game's files and kicks off the base constructor
        /// </summary>
        /// <param name="gamePath"></param>
        public DesktopRunner(string rootPath)
        {
            this.rootPath = rootPath;
        }

        protected override void ConfigureRunner()
        {
            base.ConfigureRunner();
            
            // TODO This may be a string
            Volume(MathHelper.Clamp(Convert.ToInt32((long) bios.ReadBiosData(BiosSettings.Volume.ToString(), 40L)), 0, 100));

            Mute(Convert.ToBoolean(bios.ReadBiosData(BiosSettings.Mute.ToString(), "False") as string));

            Window.Title =
                (string) bios.ReadBiosData(BiosSettings.SystemName.ToString(), "Pixel Vision 8 Runner");
            
            
        }

        #region CRT Filter Settings

        public bool EnableCRT(bool? toggle)
        {

            if (toggle.HasValue)
            {
                displayTarget.useCRT = toggle.Value;
                bios.UpdateBiosData(CRTBiosSettings.CRT.ToString(), toggle.Value.ToString());
                InvalidateResolution();
            }

            return displayTarget.useCRT;

        }

        public float Brightness(float? brightness = null)
        {
            if (brightness.HasValue)
            {
                displayTarget.brightness = brightness.Value;
                bios.UpdateBiosData(CRTBiosSettings.Brightness.ToString(), (long) (brightness * 100));
            }

            return displayTarget.brightness;
        }
        
        public float Sharpness(float? sharpness = null)
        {
            if (sharpness.HasValue)
            {
                displayTarget.sharpness = sharpness.Value;
                bios.UpdateBiosData(CRTBiosSettings.Sharpness.ToString(), sharpness);
            }

            return displayTarget.sharpness;
        }

        #endregion
        
        
        public override void ConfigureDisplayTarget()
        {
            // Get the virtual monitor resolution
            var tmpRes = ((string) bios.ReadBiosData(BiosSettings.Resolution.ToString(), "512x480"))
                .Split('x').Select(int.Parse)
                .ToArray();
            
            displayTarget = new DisplayTarget(graphics, tmpRes[0], tmpRes[1]);
            
            Fullscreen(Convert.ToBoolean(
                bios.ReadBiosData(BiosSettings.FullScreen.ToString(), "False") as string));
            StretchScreen(
                Convert.ToBoolean(
                    bios.ReadBiosData(BiosSettings.StretchScreen.ToString(), "False") as string));
            CropScreen(Convert.ToBoolean(
                bios.ReadBiosData(BiosSettings.CropScreen.ToString(), "False") as string));
            // Create the default display target

            
            // TODO this may be a string
            try
            {
                Scale(Convert.ToInt32((long) bios.ReadBiosData(BiosSettings.Scale.ToString(), 1)));
            }
            catch
            {
                Scale(Convert.ToInt32((string) bios.ReadBiosData(BiosSettings.Scale.ToString(), "1")));
            }
            
            // Configure CRT shader
            var shaderPath = FileSystemPath.Parse(bios.ReadBiosData(CRTBiosSettings.CRTEffectPath.ToString(), "/App/Effects/crt-lottes-mg.ogl.mgfxo") as string);

            
            if (workspaceService.fileSystem.Exists(shaderPath))
            {
                displayTarget.shaderPath = workspaceService.fileSystem.OpenFile(shaderPath, FileAccess.Read);
                
                EnableCRT(Convert.ToBoolean(bios.ReadBiosData(CRTBiosSettings.CRT.ToString(), "True") as string));
                Brightness(Convert.ToSingle((long) bios.ReadBiosData(CRTBiosSettings.Brightness.ToString(), 100L))/100F);
                Sharpness(Convert.ToSingle((long) bios.ReadBiosData(CRTBiosSettings.Sharpness.ToString(), -6L)));
            }
            
        }

        protected override void ConfigureKeyboard()
        {
            // Pass input mapping
            foreach (var keyMap in defaultKeys)
            {
                var rawValue = bios.ReadBiosData(keyMap.Key.ToString(), keyMap.Value, true);
                if (rawValue is long)
                    rawValue = Convert.ToInt32(rawValue);

                var keyValue = rawValue;

                tmpEngine.SetMetaData(keyMap.Key.ToString(), keyValue.ToString());
            }

            tmpEngine.controllerChip.RegisterKeyInput();
        }
        
        /// <summary>
        ///     The base runner contains a list of the core chips. Here you'll want to add the game chip to the list so it can run.
        ///     This is called when a new game is created by the runner.
        /// </summary>
        public override List<string> defaultChips
        {
            get
            {
                // Get the list of default chips
                var chips = base.defaultChips;

                // Add the custom C# game chip
                chips.Add(typeof(LuaGameChip).FullName);

                // Return the list of chips
                return chips;
            }
        }

        protected virtual void CreateWorkspaceService()
        {
            workspaceService = new WorkspaceService(new KeyValuePair<FileSystemPath, IFileSystem>(
                FileSystemPath.Root.AppendDirectory("App"),
                new PhysicalFileSystem(rootPath)));
            
            serviceManager.AddService(typeof(WorkspaceService).FullName, workspaceService);
        }
        
        /// <summary>
        ///     This is called when the runner first starts up.
        /// </summary>
        protected override void Initialize()
        {
            // Create the workspace starting at the App's directory
            CreateWorkspaceService();
            
            var biosPath = FileSystemPath.Root.AppendDirectory("App").AppendFile("bios.json");//Path.Combine(rootPath, "bios.json")));

            // Test if a bios file exists
            if (workspaceService.fileSystem.Exists(biosPath))
            {

                // Read the bios text
                var biosText = workspaceService.ReadTextFromFile(biosPath);
//                
                bios = new BiosService();

                try
                {
                    bios.ParseBiosText(biosText);
                }
                catch
                {
//                    DisplayBootErrorScreen("Error parsing system bios.");
                }

                ConfigureWorkspace();
                
                // Configure the runner
                ConfigureRunner();

                // Load the game
                LoadDefaultGame();
            }
            else
            {
                // TODO no bios found
            }
        }
        
        protected virtual void ConfigureWorkspace()
        {
            var mounts = new Dictionary<FileSystemPath, IFileSystem>();

            // Create the base directory in the documents and local storage folder
                
            // Get the base directory from the bios or use Pixel Vision 8 as the default name
            var baseDir = bios.ReadBiosData("BaseDir", "PixelVision8") as string;
                
            tmpPath = Path.Combine(LocalStorage, baseDir, "Tmp");
                
            // Create an array of required directories
            var requiredDirectories = new Dictionary<string, string>()
            {
                {"Storage", Path.Combine(LocalStorage, baseDir)},
                {"Tmp", tmpPath}
            };
                
            // Loop through the list of directories, make sure they exist and create them
            foreach (var directory in requiredDirectories)
            {
                
                if (!Directory.Exists(directory.Value))
                {

                    Directory.CreateDirectory(directory.Value);
                        
                }
                    
                // Add directories as mount points
                mounts.Add(FileSystemPath.Root.AppendDirectory(directory.Key), new PhysicalFileSystem(directory.Value));
                    
            }
                
            // Mount the filesystem
            workspaceService.MountFileSystems(mounts);

            var userBios =
                workspaceService.ReadTextFromFile(userBiosPath);
            
            bios.ParseBiosText(userBios);
  
            workspaceService.SetupLogFile(FileSystemPath.Parse(bios.ReadBiosData("LogFilePath", "/Tmp/Log.txt") as string));

        }

        public override void ConfigureServices()
        {
            var luaService = new LuaService(this);

            // Register Lua Service
            tmpEngine.AddService(typeof(LuaService).FullName, luaService);
        }

        /// <summary>
        ///     This mthod manually loads the game file's binary data then configures the engine and processes the files.
        /// </summary>
        protected virtual void LoadDefaultGame()
        {
            
            // Create a new dictionary to store the file binary data
            var gameFiles = workspaceService.LoadGame((string) bios.ReadBiosData("AutoRun", "/App/DefaultGame/"));//gamePath)));

            // Configure a new PV8 engine to play the game
            ConfigureEngine();

            // Process the files
            ProcessFiles(tmpEngine, gameFiles);

        }
        
        public virtual void SaveGameData(string path, IEngine engine, SaveFlags saveFlags, bool useSteps = true)
        {
            
            // Simple save game exporter

            var saveExporter = new SavedDataExporter(path, engine);
            saveExporter.CalculateSteps();
            while (saveExporter.completed == false)
            {
                saveExporter.NextStep();
            }
            
            // Save file
            var saveFile = new Dictionary<string, byte[]>()
            {
                {saveExporter.fileName, saveExporter.bytes}
            };
            
            workspaceService.SaveExporterFiles(saveFile);
            
        }

//        public override void ResetGame()
//        {
//            LoadDefaultGame();
//        }
        
        
        protected override void OnExiting(object sender, EventArgs args)
        {
            ShutdownSystem();

            base.OnExiting(sender, args);
        }

        public virtual void ShutdownSystem()
        {
            // We only want to call this once so don't run if shutdown is true
//            if (shutdown)
//                return;

            // Shutdown the active game
            ShutdownActiveEngine();

            // Toggle the shutdown flag
//            shutdown = true;

//            UpdateDiskInBios();
            SaveBiosChanges();
            
            // Save any changes to the bios to the user's custom bios file
            workspaceService.ShutdownSystem();
        }
        
        public override void ShutdownActiveEngine()
        {
            // Look to see if there is an active engine
            if (activeEngine == null)
                return;

            
            base.ShutdownActiveEngine();
            
            if (activeEngine.gameChip.saveSlots > 0)
            {
                //Print("Active Engine To Save", activeEngine.name);

                SaveGameData(workspaceService.FindValidSavePath(activeEngine.name), activeEngine, SaveFlags.SaveData, false);
            }

                // Save the active disk
//                workspaceService.SaveActiveDisk();

            

        }
        
        public void SaveBiosChanges()
        {
 
            // Look for changes
            if (bios.userBiosChanges != null)
            {
                // Get the path to where the user's bios should be saved
//                var path = FileSystemPath.Parse("/User/").AppendFile("user-bios.json");

                if (!workspaceService.fileSystem.Exists(userBiosPath))
                {
                    var newBios = workspaceService.fileSystem.CreateFile(userBiosPath);
                    newBios.Close();
                }

                // Create a user data dictionary
                var userData = new Dictionary<string, object>();

                // Load the raw data for ther user's bio
                var json = workspaceService.ReadTextFromFile(userBiosPath);

                // If the json file isn't empty, deserialize it
                if (json != "") userData = Json.Deserialize(json) as Dictionary<string, object>;

                // Loop through each of the items in the uerBiosChanges dictionary
                foreach (var pair in bios.userBiosChanges)
                    // Set the changed values over any existing values from the json
                    if (userData.ContainsKey(pair.Key))
                        userData[pair.Key] = pair.Value;
                    else
                        userData.Add(pair.Key, pair.Value);

                // Save the new bios data back to the user's bios file.
                workspaceService.SaveTextToFile(userBiosPath, Json.Serialize(userData), true);
            }
        }
    }
}