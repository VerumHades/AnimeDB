using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.UserInterface.prompts
{
    public class InformationPrompt : Element
    {
        private string text;
        protected bool display_hint = true;
        public InformationPrompt(string text)
        {
            this.text = text;
            Title = "Information";
            HeaderColor = new Color(26);
            ForegroundColor = Color.BLACK;
            BackgroundColor = new Color(32);
        }

        override public void Event(Root root, ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
            {
                root.ClosePrompt();
            }
        }

        override public void Draw(Root root, bool selected)
        {
            base.Draw(root, selected);

            DrawTextWrapped(0, 0, text, Width - 2);
            if (display_hint) DrawText(0, Height - 2, "Press 'enter' or 'space' to close");
        }
    }
}
