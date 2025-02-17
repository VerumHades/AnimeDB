using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.UserInterface.prompts
{
    internal class ErrorPrompt : InformationPrompt
    {
        public ErrorPrompt(string text) : base(text)
        {
            Title = "Error";
            HeaderColor = new Color(88);
            ForegroundColor = Color.WHITE;
            BackgroundColor = new Color(52);
        }
    }
}
