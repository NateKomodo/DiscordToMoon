# Discord to moon

Tool i made to convert discord json download into an image (using ascii bytes as RGB) for mr beasts lunar capsule thing.

# Usage

Git clone this repo then use dotnet CLI to compile for your platform.

You should download messages in your discord server using this https://github.com/Tyrrrz/DiscordChatExporter and specify the format as json

Put all of the JSON files into one directory (duplicate channels will be merged automatically) and run `DiscordToMoon.exe write folder-of-json-input output.png` to create the image

You can then read this back into a file with `DiscordToMoon.exe read input.png output.txt`
