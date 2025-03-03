using CommandLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectLib
{
    public class Message
    {
        public string Author { get; set; }
        public string Text { get; set; }

        public Message(string author, string text)
        {
            Author = author;
            Text = text;
        }

        public Message(ChatCommand chatCommand) 
        {
            Author = chatCommand.Author;
            Text = chatCommand.Text;
        }

        public override string ToString() 
        {
            return $"{Author}: {Text}";
        }
    }
}
