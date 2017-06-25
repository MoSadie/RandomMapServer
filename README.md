## Random Map Server

### What is it?
Well, it takes a list of Minecraft maps and chooses one at random to setup a server with. It also downloads the correct server version if it doesn't already have it downloaded.

### Setup
After compiling/downloading the executable, you run the executable. Note, it will make a folder called RandomMapServer to store all of the maps and server versions in.
After the inital setup, it will download an example world to show how an map folder is organized.
The folder structure should look like this:
```
RandomMapServer
 - maps
 - - ExampleWorld (This is the name of the map)
 - - - world
 - - - - Just normal Minecraft world stuff in here.
 - - - version.txt
 - - - - Inside of here should just be "1.12" to indicate that we want Minecraft version 1.12.
 - - - server.properties
 - - - - Demonstrates how to have files copied to the final server by changing the server MOTD.
 - versions
 - - 1.12.jar (This is the server jar for Minecraft 1.12)
 - server
 - - Normal Minecraft Server stuff like server.jar, world, server.properties.
 - - This folder is deleted and remade every time this program is opened, so don't store anything you want to save here.
```

### Alternate Usage
If you want a certain map to launch (ex. You want to generate a server.properties file) just run the executable with the arguments of the map's folder's name. (For example, "RandomMapServer.exe ExampleWorld" )