using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;

namespace RandomMapServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to RandomMapServer!");
            if (!Directory.Exists("RandomMapServer")) {
                Console.WriteLine("Oh! It looks like this is your first time running RandomMapServer! Setting up...");
                Directory.CreateDirectory("RandomMapServer");
                Console.WriteLine("Creating maps and versions directories...");
                Directory.CreateDirectory("RandomMapServer/maps");
                Directory.CreateDirectory("RandomMapServer/versions");
                Console.WriteLine("Completed inital setup. Adding example world...");
                Util.createExampleWorld();
                Console.WriteLine("Example world added! Returning to normal startup.");
            }
            Util.clearAndPrepareServerFolder();
            DirectoryInfo mapToPlay;
            if (args.Length == 0)
            {
                DirectoryInfo[] maps = new DirectoryInfo("RandomMapServer/maps").GetDirectories();
                mapToPlay = maps[new Random().Next(0, maps.Count())];
            } else
            {
                if (Directory.Exists("RandomMapServer/maps/"+args[0]))
                {
                    mapToPlay = new DirectoryInfo("RamdomMapSever/maps/" + args[0]);
                } else
                {
                    Console.WriteLine("Error: The requested map was not found.");
                    return;
                }
            }
            Util.createServer(mapToPlay.Name);
            DirectoryInfo server = new DirectoryInfo("RandomMapServer/server");
            Directory.SetCurrentDirectory(server.FullName);
            var process = Process.Start("java", "-jar server.jar --nogui");
            Console.WriteLine("Enjoy your server! Stop the server like normal when done.");
            process.WaitForExit();
        }
    }

    class Util
    {
        public static void createExampleWorld()
        {
            if (!Directory.Exists("RandomMapServer/maps/ExampleMap"))
            {
                using (var client = new WebClient())
                {
                    Console.WriteLine("Downloading example world...");
                    client.DownloadFile("https://www.dropbox.com/s/fxpq5cvw3rdw1a5/ExampleWorld.zip?dl=1", "RandomMapServer/maps/ExampleMap.zip"); //FIXME Add dropbox link
                    Console.WriteLine("Extracting example world...");
                    ZipFile.ExtractToDirectory("RandomMapServer/maps/ExampleMap.zip", "RandomMapServer/maps/ExampleMap");
                    Console.WriteLine("Cleaning Up...");
                    File.Delete("RandomMapServer/maps/ExampleMap.zip");
                    Console.WriteLine("Done!");
                }
            }
        }

        public static void clearAndPrepareServerFolder()
        {
            if (Directory.Exists("RandomMapServer/server"))
            {
                Console.WriteLine("Deleting current server folder...");
                Directory.Delete("RandomMapServer/server", true);
            }
            Console.WriteLine("Creating server folder...");
            Directory.CreateDirectory("RandomMapServer/server");
            Console.Write("Do you accept Mojang's EULA? (Y/N): ");
            ConsoleKeyInfo result = Console.ReadKey();
            Console.WriteLine();
            if (result.Key != ConsoleKey.Y)
            {
                Console.WriteLine("Ok, exiting program.");
                Environment.Exit(0);
            }
            Console.WriteLine("Creating EULA acceptance text...");
            if (!Directory.Exists("RandomMapServer/server"))
            {
                Directory.CreateDirectory("RandomMapServer/server");
            }
            using (StreamWriter eulaText = File.CreateText("RandomMapServer/server/eula.txt"))
            {
                eulaText.WriteLine("eula=true");
            }
        }

        public static void createServer(string mapFolderName)
        {
            string mapPath = "RandomMapServer/maps/" + mapFolderName;
            Console.WriteLine("Loading " + mapFolderName + "...");
            if (!Directory.Exists(mapPath))
            {
                Console.WriteLine("Something went wrong! The map " + mapFolderName + " doesn't exist!");
            }
            string minecraftVersion;
            using (StreamReader reader = File.OpenText(mapPath + "/version.txt")) {
                minecraftVersion = reader.ReadLine();
            }
            copyVersion(minecraftVersion);
            copyMap(mapFolderName);
            Console.WriteLine("Server with map of " + mapFolderName + " and Minecraft version of " + minecraftVersion + " created!");
        }

        public static void copyMap(string mapFolderName)
        {
            CopyAll(new DirectoryInfo("RandomMapServer/maps/" + mapFolderName), new DirectoryInfo("RandomMapServer/server"));
        }

        public static void copyVersion(string version)
        {
            checkForVersion(version);
            File.Copy("RandomMapServer/versions/" + version + ".jar", "RandomMapServer/server/server.jar");
        }

        public static void checkForVersion(string version)
        {
            Console.WriteLine("Checking to see if server version " + version + " has been downloaded yet...");
            if (!File.Exists("RandomMapServer/versions/" + version + ".jar"))
            {
                Console.WriteLine("Downloading Minecraft server version " + version + "...");
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://s3.amazonaws.com/Minecraft.Download/versions/" + version + "/minecraft_server." + version + ".jar","RandomMapServer/versions/"+version+".jar");
                }
            } else
            {
                Console.WriteLine("Minecraft server version " + version + " is already downloaded!");
            }
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
