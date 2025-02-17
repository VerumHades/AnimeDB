using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.UserInterface
{
    /// <summary>
    /// A class to save color for the screen
    /// </summary>
    public class Color
    {
        public static Color BLACK = new Color(0);
        public static Color RED = new Color(1);
        public static Color GREEN = new Color(2);
        public static Color YELLOW = new Color(3);
        public static Color BLUE = new Color(4);
        public static Color MAGENTA = new Color(5);
        public static Color CYAN = new Color(6);
        public static Color WHITE = new Color(7);

        public static Color BRIGHT_BLACK = new Color(8);
        public static Color BRIGHT_RED = new Color(9);
        public static Color BRIGHT_GREEN = new Color(10);
        public static Color BRIGHT_YELLOW = new Color(11);
        public static Color BRIGHT_BLUE = new Color(12);
        public static Color BRIGHT_MAGENTA = new Color(13);
        public static Color BRIGHT_CYAN = new Color(14);
        public static Color BRIGHT_WHITE = new Color(15);

        public int index;

        public Color(int index)
        {
            this.index = index;
        }

        public void addAnsiCode(ref int index, char[] buffer, bool background = false)
        {
            buffer[index++] = '\x1b';
            buffer[index++] = '[';
            buffer[index++] = background ? '4' : '3';
            buffer[index++] = '8';
            buffer[index++] = ';';
            buffer[index++] = '5';
            buffer[index++] = ';';

            int accumulated = 0;
            for(int i = 0;i < 3; i++)
            {
                double divisor = Math.Pow(10,  2 - i);
              
                int digit = (int) Math.Floor((double)(this.index - accumulated) / divisor);

                if (digit == 0 && i != 2) continue;
                accumulated += (int)divisor * digit;

                buffer[index++] = (char)('0' + (digit));
            }

            buffer[index++] = 'm';
        }
    }
}
