about as simple as it gets, this is a tiny C# MPD client which hooks your media keys and uses them to control an MPD server. hold shift to not do that. 

has a config file called 'config', stored in the same folder as the executable, which stores host and port information plus a couple other things. if the program is connected when the user exits, on next start it'll attempt to reconnect.