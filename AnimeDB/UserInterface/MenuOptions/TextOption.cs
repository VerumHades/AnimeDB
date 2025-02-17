using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AnimeDB.UserInterface.MenuOptions
{
    /// <summary>
    /// A menu option that allows inputing text
    /// </summary>
    public class TextOption : MenuOption
    {
        private string title;
        private string text;
        private Action<string, NestedMenu> on_change;

        public TextOption(string title, string description, string text, Action<string, NestedMenu> on_change)
        {
            this.title = title;
            this.text = text;
            Description = description;
            this.on_change = on_change;
        }

        public string Title { get { return title; } set { title = value; } }

        public string Text => title;
        public string Argument => text;

        public string Description { get; set; }

        public void execute(int x, int y, NestedMenu menu)
        {
            Console.SetCursorPosition(menu.X + x + 1, menu.Y + y);
            Console.Write(" ".PadLeft(text.Length));
            Console.SetCursorPosition(menu.X + x + 1, menu.Y + y);
            Console.CursorVisible = true;
            string new_text = Console.ReadLine();
            if (new_text != "")
            {
                text = new_text;
                on_change(text, menu);
            }
            Console.CursorVisible = false;
        }
    }
}

