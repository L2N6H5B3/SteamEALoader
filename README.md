# SteamEALoader
Load your EA Games from Steam

## How to use
SteamEALoader is designed to be used to launch EA Games (either via Origin or EA Desktop) from Steam or Steam Big Picture.  
Download the latest release, along with the appropriate [.NET 5.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/5.0).

Decide if you would like SteamEALoader to close the EA Loader after exiting the game.  If you would like to leave the launcher running, omit the 'launcher process name' argument below.

Add to Steam as a Non-Steam Game, open Properties, change Game Name and add the following info to the Launch Options / Arguments:

```"<full path to game executable>" "<game process name>" "<launcher process name>"```

Example:

```"C:\Program Files\EA Games\STAR WARS Battlefront II\starwarsbattlefrontii.exe" "starwarsbattlefrontii" "EADesktop"```
