using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.UserInterface.MenuOptions
{
    /// <summary>
    /// A tohle menu option
    /// </summary>
    public class ToggleOption : MenuOption
    {
        private string text;
        private bool state = false;
        private Action on_action;
        private Action off_action;

        public ToggleOption(bool state, string text, string description, Action on_action, Action off_action)
        {
            this.state = state;
            this.on_action = on_action;
            this.off_action = off_action;
            Description = description;
            this.text = text;
        }

        public string Text => text;
        public string Argument => $"[{(state ? "*" : " ")}]";

        public string Description { get; set; }

        public void execute(int x, int y, NestedMenu menu)
        {
            if (state) off_action();
            else on_action();

            state = !state;
        }
    }
}
