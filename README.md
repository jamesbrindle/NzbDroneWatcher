# NzbDroneWatcher

This is a Windows services that watches NzbDrone services, such as:

* Sonarr
* Radarr
* Lidarr

This solution is specific to my use case, so alter for your own scenario, but basically, I have two 'Radarr' instances - Using the same exe process, but pointing at different 
paths and using NSSM in order to have multiple Radarr 'instances'.

It works fine having two Radarr instances this way (using NSSM - without the need for Docker - Which I ended up removing due it Docker popping up with error everytime I shut down Windows) but 
every now and again, one of the Radarr services would fail and stop. There's just something that causing this that I can't work out...

So as a workaround, I elected to just create a quick Windows services that watches the NZB Done services and start them if they're down.

The usual Windows 'Recovery' options don't appear to be a solution to this issue, because, for some reason it's required that the original 'Radarr' services (not the one using NSSM) needs
to be started up first... So this Windows service would first stop Radarr-4K (using NSSM) and then start up Radarr (original) then start up Radarr-4K (NSSM) again.

While I were at it, I also made this watch my other NZB Drone services (Sonarr and Lidarr) just in case those went down.

##  NZB Drone Service List

You specify ther service list in the App.Config file in a key value node element. It looks like this:

```
<add key="ServicesToWatch" value="[Sonarr:Sonarr.Console];[Radarr:Radarr];[Radarr-4K:nssm];[Lidarr:Lidarr.Console]"/>
```

Where you seperate the service list with a semi-colon (`;`), and inside square brackets you include a service - process map, where the left side is a service name and the right side is a process name 
seperated by a regarl colon (`:`).

This is so, if the service fails to 'stop' (where we decide to stop it), it will kill the process.

## Installing the Windows Service from this Windows Service Project

See here:

https://docs.microsoft.com/en-us/dotnet/framework/windows-services/how-to-install-and-uninstall-services

### Example Service Install

*Using the .Net 4.x framework, on a 64-bit operating system, where the exe is places in a folder: C:\Utilities\NzbDroneWatcher:*

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319>InstallUtil.exe "c:\Utilities\NzbDroneWatcher\NzbDroneWatcher.exe"
```

You can change certain properties by default of the service in the 'Designer View' of class 'ProjectInstaller.cs'.
