using FinalProjectLib;

namespace CommandLib
{
    public class Command
    {
        public ChatCommand? ChatCommand { get; private set; } = null;
        public MoveCommand? MoveCommand { get; private set; } = null;
        public GameFieldCommand? GameFieldCommand { get; private set; }

        public StatusCode? StatusCode { get; set; }

        private Command() { }

        public Command(ChatCommand chatCommand)
        {
            ChatCommand = chatCommand;
        }

        public Command(MoveCommand moveCommand)
        {
            MoveCommand = moveCommand;
        }

        public Command(GameFieldCommand gameFieldCommand)
        {
            GameFieldCommand = gameFieldCommand;
        }
    }

    public class ChatCommand 
    {
        public string Text { get; private set; } = "";
        public string Author { get; private set; } = "";

        public ChatCommand(string text, string author)
        {
            Text = text;
            Author = author;
        }
    }

    public class MoveCommand 
    {
        public short Index {  get; private set; }
        public string Element { get; private set; } = "";

        public MoveCommand(short index, string element)
        {
            Index = index;
            Element = element;
        }
    }

    public class GameFieldCommand 
    {
        public GameField GameField { get; private set; }

        public GameFieldCommand(GameField gameField)
        {
            GameField = gameField;
        }
    }
}
