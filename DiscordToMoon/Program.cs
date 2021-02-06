using System;
using System.Drawing.Imaging;
using DiscordToMoon.Readers;
using DiscordToMoon.Writers;

namespace DiscordToMoon
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3) throw new ArgumentException("You must pass in 3 arguments. Usage: DiscordToMoon.exe <read/write/writeraw> <in> <out>");
            switch (args[0])
            {
                case "write":
                    new DiscordImageWriter(args[1]).Write(args[2], ImageFormat.Png);
                    break;
                case "read":
                    new ImageReader(args[1]).Read(args[2]);
                    break;
                case "writeraw":
                    new GenericImageWriter(args[1]).Write(args[2], ImageFormat.Png);
                    break;
            }
        }
    }
}