using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DiscordToMoon.Readers
{
    public sealed class ImageReader
    {
        private readonly Bitmap _bmp;
        
        public ImageReader(string path)
        {
            Console.WriteLine($"Loading image from {path}");
            _bmp = new Bitmap(path);            
        }

        public void Read(string saveTo)
        {
            Console.WriteLine($"Reading image into {saveTo}");

            using (var sw = File.AppendText(saveTo))
            {
                for (var y = 0; y < _bmp.Height; y++)
                {
                    for (var x = 0; x < _bmp.Width; x++)
                    {
                        var result = "";
                        result += (char) _bmp.GetPixel(x, y).R;
                        result += (char) _bmp.GetPixel(x, y).G;
                        result += (char) _bmp.GetPixel(x, y).B;
                        result = result.Replace(((char) 0).ToString(), "");
                        if (string.IsNullOrEmpty(result)) continue;
                        sw.Write(result);
                    }
                }
            }

            Console.WriteLine("Done");
        }
    }
}