______     _                 ___  ____   _______ 
|  ___|   | |                |  \/  | | | |  _  \
| |_ _   _| |_ _   _ _ __ ___| .  . | | | | | | |
|  _| | | | __| | | | '__/ _ \ |\/| | | | | | | |
| | | |_| | |_| |_| | | |  __/ |  | | |_| | |/ / 
\_|  \__,_|\__|\__,_|_|  \___\_|  |_/\___/|___/  
                                                 
                                                 
=========1=========2=========3=========4=========5=========6=========7=========8

FutureMUD is a MUD engine designed to be used to create RPI MUDs. 

I highly encourage you to join us on our Discord, introduce yourself, and draw
upon the support that exists there. 

https://discord.gg/MZpPcZF

===| Liscensing |===============================================================

FutureMUD and all its tools are currently available under the following license:

Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License
https://creativecommons.org/licenses/by-nc-nd/3.0/

However, I do intend to revise this in the future to a license that permits 
commercial usage once I do a bit more research. You can feel free to proceed on
the assumption that I will change the license in the future with the following
main assumptions:

-- The MUD will always be free to use, no licensing fee will be charged

-- You are free to take donations, sell access to or otherwise monetise your MUD
   and everything around it

-- You are NOT free to on-sell the engine to others or otherwise sub-license the
   engine

-- You may not represent the engine as your own work and you must advertise your
   game as powered by the FutureMUD engine

-- I have not yet decided whether I want to open-source the engine yet. At this
   stage my reservation is mostly around maintaining control over the creative
   direction of the engine. I may change my mind on this in the future.

===| Requirements |=============================================================

There are the following minimum requirements:

-- 64 Bit Windows, Linux or MacOS
-- .Net Core 6
-- MySQL Server Version 5.7 or 8.0

You will also need to have an email address that you can send verification 
emails from eventually, but it is not strictly necessary when you are in the 
early development stage.

There are also the following minimum hardware requirements:

-- Minimum harddrive space ~150mb for the files, ~50mb for an average 
   database, ~5mb per 10 players per day for text logs

-- ~500mb of RAM for the game engine, but make sure to account for the operating
   system and anything else running (like the database, web server, etc)

-- Multi-core processor highly recommended, and running the game engine on its 
   own dedicated core

During the development stage when you're first building your MUD most of the 
major cloud hosting sites offer free-tier services that will work, e.g. 
Amazon's EC2 servers. You could also run it on your local PC and transfer the
database to a proper server later in your development.

===| Contents |================================================================

There are 4 key executables distributed with the engine:

1) MudSharpCore

This is the actual MUD Engine. This is what you will run on the server.

2) DatabaseSeeder

This sets up the database and seeds it with default data. You will run this
before you run the MUD Server for the first time.

3) DiscordBot

This optional module adds a discord bot integration for the MUD.

4) TerrainPlanner

This optional module is used to draw terrain for use with the MUD's autobuilder
module, designed for making large wilderness areas very quickly. This executable
only works on Windows and is designed to be used by your staff on their own PCs.

===| Installation |=============================================================

This section will run you through how to get the MUD up and running on your MUD
server. I assume in this guide that you are installing the MUD on a new server
that has not otherwise been setup or has been setup as a basic WAMP/LAMP server.

I should note however that it's absolutely possible to run all of this 
on your home PC and then transfer your database file to a hosted MUD server only
after you've done some building.

1) Ensure that you have installed the latest version of .Net 6. 
   
   https://dotnet.microsoft.com/download/dotnet/6.0

2) Ensure that you have installed MySQL Server 5.7 or 8.0

   For Windows
   https://dev.mysql.com/downloads/installer/

   For Linux
   https://dev.mysql.com/doc/refman/8.0/en/linux-installation.html

   I recommend that you create a user specifically for the MUD engine rather 
   than using the root account. Because the engine maintains the database 
   automatically during updates the user will require all but the GRANT
   permissions on the database you're creating.

   However, if you're just messing around you can feel free to use the root
   account to get things set up.

3) Download and Run the DatabaseSeeder application on the MUD Server (wherever 
   you're running the MUD from). If you're running on Linux, don't forget to 
   ensure you CHMOD the file to give it execute privs.
   
   The Database Seeder will run you through a series of questions that will set 
   the database up and fill it with the minimum information required to boot the 
   MUD.

   It will also create some example scripts for launching the MUD, though you
   may change, ignore, discard these as you please.

   In order to run the Database Seeder you will need to know the connection
   string to your databse. This is almost certainly in the following format
   if you installed MySQL with the default settings:

   server=localhost;port=3306;database=YOURMUDDB;uid=YOURUSERNAME;password=YOURPASSWORD;Default Command Timeout=300000

   Where your_dbo is what you want to call the database, username is your 
   MySQL username, and password is your MySQL password.

4) The Database Seeder sets up a file called Connection.config which contains
   the IP and Port number for the MUD. By default the MUD will run on port 4000
   but if you want it to run on a different port this is the place to do it.

   Whichever port you choose (or even if you leave the port open) you will need
   to configure your server's firewall to allow it through. This is tied very
   tightly to your operating system and other setup so I can't cover this in 
   detail in this guide. 

   In many cases you may do this on your hosting control panel entirely (which
   is the case on Amazon Web Services for example).

5) The MUD Server itself is a console application which means that it does
   not run with a GUI. Generally you will run it in the console or through
   a shortcut.

   The Database Seeder will have generated a file called Start-MUD.bat
   (for Windows) or Start-MUD.sh (for Linux or MacOS). This is an example of
   how to launch the MUD server and keep it auto-restarting if it crashes or
   if you reboot it.

   The Windows version of the script also copies files from a folder called 
   "Binaries" in the base folder. The idea is that you can apply new patches
   in this folder and the script will copy the files up to the base directory
   when it reboots. The linux script does not do this so you'll have to either
   modify it yourself or stop the MUD to apply patches.

   If you want to write your own version of this or you're running it locally
   what you need to know is that you must launch the MUD server with two 
   command line arguments.

   The first argument is always "MySql.Data.MySqlClient" for now but this may
   change to have multiple options one day.

   The second argument is your connection string, the same as you used for the
   Database Seeder.

   So for example, this is the command you would use to launch the MUD engine
   manually on Windows (in the command line):

   MudSharp.exe "MySql.Data.MySqlClient" "server=localhost;port=3306;database=YOURMUDDB;uid=YOURUSERNAME;password=YOURPASSWORD;Default Command Timeout=300000"

===| Common Installation Issues |==============================================

Issue: 

When you double click on the DatabaseSeeder.exe file it opens and then instantly 
closes.

Problem: 

You haven't got .Net 6 installed. Please install .Net 6 from the Microsoft 
website

Issue: 

You double click on the MudSharpCore.exe and it opens and instantly closes,
after having already run the Database Seeder.

Problem: 

You aren't supplying any command line arguments. Either use a script or make 
a shortcut with the command line arguments.

===| Discord Bot Setup |========================================================

TODO

===| Getting Started |==========================================================

TODO