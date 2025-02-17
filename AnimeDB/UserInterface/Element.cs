
namespace AnimeDB.UserInterface
{
    /// <summary>
    /// A base class for building elements tho the ui largely consist of just prompts because of their design convinience
    /// </summary>
    public abstract class Element
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string? Title { get; set; }

        public Color BackgroundColor { get; set; }

        public Color ForegroundColor { get; set; }

        public Color HeaderColor { get; set; }

        public Element() {
            BackgroundColor = new Color(236);
            ForegroundColor = Color.WHITE;
            HeaderColor = new Color(240);
            Width = 50;
            Height = 15;
        }

        /// <summary>
        /// Handles a key press dedicated to an element
        /// </summary>
        /// <param name="root"></param>
        /// <param name="key_info"></param>
        virtual public void Event(Root root, ConsoleKeyInfo key_info)
        {

        }

        /// <summary>
        /// Renders the element to the roots screen
        /// </summary>
        /// <param name="root"></param>
        /// <param name="selected"></param>
        virtual public void Draw(Root root, bool selected)
        {
            DrawBorder(root);
            if (selected) DrawText(0, -1, $" {Title} ", BackgroundColor, ForegroundColor);
            else DrawText(0, -1, $" {Title} ", null, HeaderColor);
            
        }
        /// <summary>
        /// Renders  the elements background and header
        /// </summary>
        /// <param name="root"></param>
        protected void DrawBorder(Root root)
        {
            int bw = Width;
            int bh = Height;

            DrawText(-1, -1, new string(' ', bw), null, HeaderColor);

            for(int i = 0;i < bh - 1; i++)
            {
                DrawText(-1, i, new string(' ',bw) );
            }

            //DrawText(-1, -1, "┌");
            //DrawText(-1, Height + 1, "└");
            //DrawText(Width + 1, -1, "┐");
            //DrawText(Width + 1, Height + 1, "┘");
        }
        /// <summary>
        /// Renders any text to the screen with some color
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="text">The text</param>
        /// <param name="fg_color">Background Color</param>
        /// <param name="bg_color">Foreground Color</param>
        protected void DrawText(int x, int y, string text, Color? fg_color = null, Color? bg_color = null)
        {
            if (text == null) text = "";
            text = text.Replace("\n", "").Replace("\r", "");

            if(Root.scr != null) Root.scr.DrawTextAt(X + 1 + x,Y + 1 + y,text, fg_color ?? ForegroundColor, bg_color ?? BackgroundColor);
        }

        /// <summary>
        /// Draws text but allows for wrapping
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="text">The text</param>
        /// <param name="wrap_width">The width after which to wrap</param>
        /// <returns>The amount of lines drawn</returns>
        protected int DrawTextWrapped(int x, int y, string text, int wrap_width)
        {
            int original_y = y;

            var words = text.Split(" ");
            int word_index = 0;

            while(word_index < words.Length)
            {
                string line = "";
                while (line.Length < wrap_width && word_index < words.Length)
                {
                    if(line.Length + words[word_index].Length >= wrap_width)
                    {
                        if(line.Length == 0)
                        {
                            line += words[word_index++].Substring(0, line.Length);
                            continue;
                        }
                        else break;
                    }
                    line += words[word_index++] + " ";
                }

                DrawText(x, y++, line);
            }

            return y - original_y;
        }
    }
}
