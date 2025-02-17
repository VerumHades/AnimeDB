using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AnimeDB.UserInterface.prompts
{
    /// <summary>
    /// A prompt that ask for users confirmation and executes a lambda based on the input
    /// </summary>
    public class ConfirmationPrompt : InformationPrompt
    {
        private bool confirmation = false;
        private Action on_confirmed;
        public ConfirmationPrompt(string text, Action on_confirmed) : base(text)
        {
            Title = "Confirmation";
            this.on_confirmed = on_confirmed;
            display_hint = false;
        }
        override public void Event(Root root, ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Tab)
            {
                confirmation = !confirmation;
            }
            else if (key.Key == ConsoleKey.LeftArrow)
            {
                confirmation = true;
            }
            else if (key.Key == ConsoleKey.RightArrow)
            {
                confirmation = false;
            }
            else if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
            {
                root.ClosePrompt();
                if (confirmation) on_confirmed();
            }
        }

        override public void Draw(Root root, bool selected)
        {
            base.Draw(root, selected);

            if (confirmation) DrawText(0, Height - 2, "Confirm", BackgroundColor, ForegroundColor);
            else DrawText(0, Height - 2, "Confirm", ForegroundColor, HeaderColor);

            if (!confirmation) DrawText(Width - 9, Height - 2, "Cancel", BackgroundColor, ForegroundColor);
            else DrawText(Width - 9, Height - 2, "Cancel", ForegroundColor, HeaderColor);
        }
    }
}
