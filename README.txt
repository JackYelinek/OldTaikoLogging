This is a program I made only for me (for now) so a lot of it includes hardcoded values. Also I took out a few files that have private information that I don't want public. 

The purpose of the program is to capture pictures from Taiko (PS4 rhythm game) and recognize whenever I would get a new high score, and send that score to my spreadsheet (https://docs.google.com/spreadsheets/d/15qtcBVmes43LlejxgynYoWWxqf99bVK-WmQUfGZfupo) as well as send a message in my twitch chat showing how much higher the score is (twitch could also be turned off if I'm not streaming). It can also see high scores from an emulator version of Taiko, but I don't play that too often, so I don't update or use that part of the program much. 

The program works by taking a screenshot of my capture software (OBS) and compares it to various state images to see what's going on in the game, if it's loading or I'm in the main menu or playing a song. It'll then recognize if I reached the results screen for a song, and read the data on the image and compare that to the data on my spreadsheet. If it's a record, it'll put it on, and if not, it won't. 
The way I get it to read the data from the image isn't very good though, it's just comparing bitmaps to see what each value is. I tried using OCR and failed, but maybe I need to look into it more to get it to work properly. 


I still want to add a few features to it, such as:
More/better Tests
Optimizations (not really a feature, but I need to do it anyway)
Set it up so anybody can use it (if anybody would want to use it, it's a very niche program)
Comments (also not a feature, but it would make my code look a bit less awful)
Variable file locations (kinda ties in with letting other people use it)
Potentially a "Recent High Scores" sheet in my spreadsheet, keeping the 10 most recent or so.