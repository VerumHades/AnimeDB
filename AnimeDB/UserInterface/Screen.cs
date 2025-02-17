using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.UserInterface
{
    /// <summary>
    /// A class to handle rendering to the terminal without flicker,
    /// 
    /// Based on code from El chato  but heavily modified (Was originally rendering one by line, I reduced it to a C style buffer build every redraw but not reallocated)
    /// </summary>
    public class Screen
    {
        
        private struct Pixel
        {
            public char character = ' ';
            public Color fg_color = Color.WHITE;
            public Color bg_color = Color.BLACK;

            public Pixel()
            {
            }
        }
        char[] full_buffer;
        private Pixel[,] buffer;
        private int width;
        private int height;

        public int Width { get { return width; } }
        public int Height { get { return height; } }


        public Screen(int width, int height)
        {
            this.width = width;
            this.height = height;
            buffer = new Pixel[height, width];
            full_buffer = new char[width * height * 23 + height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    buffer[y, x].character = ' ';  
                }
            }
        }


        public void DrawTextAt(int x, int y, string text, Color? fg_color = null, Color? bg_color = null)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;

            int textLength = text.Length;
            for (int i = 0; i < textLength; i++)
            {
                if (x + i < width)
                {
                    buffer[y, x + i].fg_color = fg_color ?? Color.WHITE;
                    buffer[y, x + i].bg_color = bg_color ?? Color.BLACK;
                    buffer[y, x + i].character = text[i];
                }
            }
        }

        public void Render()
        {

            Console.SetCursorPosition(0, 0);

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color fg = buffer[y, x].fg_color ?? Color.WHITE;
                    Color bg = buffer[y, x].bg_color ?? Color.BLACK;
                    fg.addAnsiCode(ref index, full_buffer);
                    bg.addAnsiCode(ref index, full_buffer, true);
                    full_buffer[index++] = buffer[y, x].character; 
                }
                full_buffer[index++] = '\n';
            }

            Console.Write(full_buffer,0,index);
        }


        public void Clear()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    buffer[y, x].fg_color = Color.WHITE;
                    buffer[y, x].bg_color = Color.BLACK;
                    buffer[y, x].character = ' ';  
                }
            }
        }
    }
}
