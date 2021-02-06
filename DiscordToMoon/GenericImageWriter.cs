using System;
using System.Drawing.Imaging;
using System.IO;

namespace DiscordToMoon
{
    public sealed class GenericImageWriter : ImageWriter
    {
        private readonly string _value;
        
        public GenericImageWriter(string json)
        {
            _value = new JsonLoader(json).FromText();
            
            long characters = _value.Length;

            var pixels = (int)Math.Ceiling((characters / 3f));
            var canvas = (int)Math.Ceiling(Math.Sqrt(pixels));
            
            Console.WriteLine($"Total characters: {characters}, need {pixels} pixels (canvas size {canvas}x{canvas})");
            
            CreateCanvas(canvas);
        }
        
        public void Write(string imagePath, ImageFormat format)
        {
            Console.WriteLine("Making bitmap");
            
            foreach (var s in _value.ToCharArray())
                Write(s);
                
            FlushBuffer();
            Save(imagePath, format);
            
            Console.WriteLine($"Saved to {new FileInfo(imagePath).FullName}");
            Console.WriteLine("Done");
        }
    }
}