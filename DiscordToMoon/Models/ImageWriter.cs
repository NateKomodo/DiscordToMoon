using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace DiscordToMoon.Models
{
    public abstract class ImageWriter
    {
        private Bitmap _bmp;

        private int _x;
        private int _y;

        private int _canvas;

        private (char?, char?, char?) _buffer = (null, null, null);

        protected void CreateCanvas(int size)
        {
            _canvas = size;
            _bmp = new Bitmap(size, size);
        }

        protected void Write(char cha)
        {
            if (cha >= 128)
            {
                Console.WriteLine($"Dropping invalid character: {cha}");
                return;
            }
            
            if (!_buffer.Item1.HasValue)
                _buffer.Item1 = cha;
            else if (!_buffer.Item2.HasValue)
                _buffer.Item2 = cha;
            else if (!_buffer.Item3.HasValue)
                _buffer.Item3 = cha;
            else
                FlushBuffer(cha);
        }
        
        protected void FlushBuffer(char? overflow = null)
        {
            if (_bmp == null) throw new NullReferenceException("Please call create canvas before flushing buffer");
            var c = Color.FromArgb(255, _buffer.Item1 ?? 0, _buffer.Item2 ?? 0, _buffer.Item3 ?? 0);
            _bmp.SetPixel(_x, _y, c);
            _buffer = (overflow, null, null);
            _x++;
            if (_x >= _canvas)
            {
                _x = 0;
                _y++;
            }
        }

        protected void Save(string path, ImageFormat format)
        {
            _bmp.Save(path, format);
        }
    }
}