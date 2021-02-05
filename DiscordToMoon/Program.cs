using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DiscordToMoon
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3) throw new ArgumentException("You must pass in 3 arguments. Usage: DiscordToMoon.exe <read/write> <in> <out>");
            switch (args[0])
            {
                case "write":
                    ToFile(args[1], args[2], ImageFormat.Png);
                    break;
                case "read":
                    FromFileRaw(args[1], args[2]);
                    break;
            }
        }
        
        private static void ToFile(string jsonPath, string imagePath, ImageFormat format)
        {
            Console.WriteLine($"Loading json from {jsonPath}/*");
            
            var json = Directory.GetFiles(jsonPath).Select(file => JObject.Parse(File.ReadAllText(file))).ToList();

            Console.WriteLine($"Loaded {json.Count} files");
            
            var manifest = new Dictionary<string, List<string>>();
            
            foreach (var j in json)
            {
                var name = j.SelectToken("channel.name")?.ToObject<string>();
                var messages = j.SelectToken("messages", false)?.ToObject<JArray>();
                
                if (string.IsNullOrEmpty(name) || messages == null) throw new NullReferenceException();
                
                var messageList = new List<string>();
                
                foreach (var message in messages)
                {
                    var content = message.SelectToken("content", false)?.ToObject<string>();
                    var author = message.SelectToken("author.name", false)?.ToObject<string>();
                    content = content != null ? StripNonAscii(content) : null;
                    author = author != null ? StripNonAscii(author) : null;
                    messageList.Add(string.IsNullOrEmpty(content)
                        ? $"{author}: <image/file/emoji>\n"
                        : $"{author}: {content}\n");
                }

                name = "#" + name;
                
                if (manifest.ContainsKey(name))
                    manifest[name].AddRange(messageList);
                else
                    manifest.Add(name, messageList);
            }
            
            Console.WriteLine($"Loaded {manifest.Keys.Count} channels and {manifest.Values.Select(m => m.Count).Sum()} messages");
            foreach (var kvp in manifest) Console.WriteLine($" - {kvp.Key}: {kvp.Value.Count}");

            long characters = manifest.Values.Select(l => l.Select(s => s.Length).Sum()).Sum() + manifest.Keys.Select(s => s.Length + 2).Sum();

            var pixels = (int)Math.Ceiling((characters / 3f));
            var canvas = (int)Math.Ceiling(Math.Sqrt(pixels));
            
            Console.WriteLine($"Total characters: {characters}, need {pixels} pixels (canvas size {canvas}x{canvas})");
            
            var bmp = new Bitmap(canvas, canvas);

            var x = 0;
            var y = 0;
            
            var writeCalls = 0;
            var pixelsWritten = 0;

            (char?, char?, char?) buffer = (null, null, null);

            Console.WriteLine("Making bitmap");

            foreach (var (key, value) in manifest)
            {
                Console.WriteLine($"Writing: {key}");
                
                var channelHeader = key + ":\n";
                var header = channelHeader.ToCharArray();
                
                foreach (var s in header)
                    Write(s, ref buffer, ref bmp, ref x, ref y, ref writeCalls, ref pixelsWritten, canvas);

                foreach (var s in value.Select(message => message.ToCharArray()).SelectMany(msg => msg))
                    Write(s, ref buffer, ref bmp, ref x, ref y, ref writeCalls, ref pixelsWritten, canvas);
                
            }
            
            FlushBuffer(ref buffer, ref bmp, ref x, ref y, ref pixelsWritten, canvas);
            
            bmp.Save(imagePath, format);
            
            Console.WriteLine($"Saved to {new FileInfo(imagePath).FullName}");
            
            Console.WriteLine($"{writeCalls} calls to write, expected {characters}");
            Console.WriteLine($"{pixelsWritten} pixels written, expected {pixels}");
            
            Console.WriteLine("Done");
        }

        private static void FromFileRaw(string path, string toFile)
        {
            Console.WriteLine($"Loading image from {path}");

            var bmp = new Bitmap(path);
            
            
            Console.WriteLine("Reading image");
            
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    var result = "";
                    result += (char)bmp.GetPixel(x, y).R;
                    result += (char)bmp.GetPixel(x, y).G;
                    result += (char)bmp.GetPixel(x, y).B;
                    if (result.ToCharArray().Contains((char)0)) continue;
                    File.AppendAllText(toFile, result);
                }
            }

            Console.WriteLine("Done");
        }

        private static void Write(char cha, ref (char?, char?, char?) buffer, ref Bitmap bmp, ref int x, ref int y, ref int writeCalls, ref int pixelsWritten, int canvas)
        {
            writeCalls++;
            if (cha >= 128)
            {
                Console.WriteLine($"Dropping invalid character: {cha}");
                return;
            }
            
            if (!buffer.Item1.HasValue)
                buffer.Item1 = cha;
            else if (!buffer.Item2.HasValue)
                buffer.Item2 = cha;
            else if (!buffer.Item3.HasValue)
                buffer.Item3 = cha;
            else
                FlushBuffer(ref buffer, ref bmp, ref x, ref y, ref pixelsWritten, canvas, cha);
        }

        private static void FlushBuffer(ref (char?, char?, char?) buffer, ref Bitmap bmp, ref int x, ref int y, ref int pixelsWritten, int canvas, char? overflow = null)
        {
            var c = Color.FromArgb(255, buffer.Item1 ?? 0, buffer.Item2 ?? 0, buffer.Item3 ?? 0);
            bmp.SetPixel(x, y, c);
            buffer = (overflow, null, null);
            x++;
            if (x >= canvas)
            {
                x = 0;
                y++;
            }
            pixelsWritten++;
        }

        private static string StripNonAscii(string s)
        {
            return s.ToCharArray().Where(c => c < 128).Aggregate("", (current, c) => current + c);
        }
    }
}