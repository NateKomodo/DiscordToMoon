# Discord to moon

Tool i made to convert discord json download into an image (using ascii bytes as RGB) for mr beasts lunar capsule thing.

# Usage

Git clone this repo then use dotnet CLI to compile for your platform. Alternatively, as of v1.1, precompiled binaries for linux and windows are available (see releases tab)

You should download messages in your discord server using this https://github.com/Tyrrrz/DiscordChatExporter and specify the format as json

Put all of the JSON files (should be one per channel) into one directory (duplicate channels will be merged automatically) and run `DiscordToMoon write folder-of-json-input output.png` to create the image

You can then read this back into a file with `DiscordToMoon read input.png output.txt`

If you want to play around with the text to image functionality, put a string (any text) in a file and run `DiscordToMoon writeraw input.txt out.png`
