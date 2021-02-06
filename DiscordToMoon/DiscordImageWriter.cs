using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace DiscordToMoon
{
    public sealed class DiscordImageWriter : ImageWriter
    {
        private readonly Dictionary<string, List<string>> _manifest;
        
        public DiscordImageWriter(string jsonFolder)
        {
            _manifest = new JsonLoader(Directory.GetFiles(jsonFolder)).FromDiscord();
            
            long characters = _manifest.Values.Select(l => l.Select(s => s.Length).Sum()).Sum() + _manifest.Keys.Select(s => s.Length + 2).Sum();

            var pixels = (int)Math.Ceiling((characters / 3f));
            var canvas = (int)Math.Ceiling(Math.Sqrt(pixels));
            
            Console.WriteLine($"Total characters: {characters}, need {pixels} pixels (canvas size {canvas}x{canvas})");
            
            CreateCanvas(canvas);
        }

        public void Write(string imagePath, ImageFormat format)
        {
            Console.WriteLine("Making bitmap");

            foreach (var (key, value) in _manifest)
            {
                Console.WriteLine($"Writing: {key}");

                foreach (var s in (key + ":\n").ToCharArray())
                    Write(s);
                
                foreach (var s in value.Select(message => message.ToCharArray()).SelectMany(msg => msg))
                    Write(s);
            }
            
            FlushBuffer();
            Save(imagePath, format);
            
            Console.WriteLine($"Saved to {new FileInfo(imagePath).FullName}");
            Console.WriteLine("Done");
        }
    }
}