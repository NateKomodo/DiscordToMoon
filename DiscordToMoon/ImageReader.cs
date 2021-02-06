using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DiscordToMoon
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

            using (StreamWriter sw = File.AppendText(saveTo))
            {
                for (var y = 0; y < _bmp.Height; y++)
                {
                    for (var x = 0; x < _bmp.Width; x++)
                    {
                        var result = "";
                        result += (char) _bmp.GetPixel(x, y).R;
                        result += (char) _bmp.GetPixel(x, y).G;
                        result += (char) _bmp.GetPixel(x, y).B;
                        if (result.ToCharArray().Contains((char) 0)) continue;
                        sw.Write(result);
                    }
                }
            }

            Console.WriteLine("Done");
        }
    }
}