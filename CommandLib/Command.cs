using FinalProjectLib;

namespace CommandLib
{
    public class Command
    {
        public ChatCommand? ChatCommand { get; private set; }
        public MoveCommand? MoveCommand { get; private set; }
        public GameFieldCommand? GameFieldCommand { get; private set; }

        public StatusCode? StatusCode { get; set; }

        private Command() { }

        public Command(ChatCommand chatCommand)
        {
            ChatCommand = chatCommand;
            MoveCommand = null;
            GameFieldCommand = null;
        }

        public Command(MoveCommand moveCommand)
        {
            MoveCommand = moveCommand;
            GameFieldCommand = null;
            ChatCommand = null;
        }

        public Command(GameFieldCommand gameFieldCommand)
        {
            GameFieldCommand = gameFieldCommand;
            MoveCommand = null;
            ChatCommand = null;
        }

        public override string ToString()
        {
            if (ChatCommand is not null)
            {
                return "отправлено сообщение в чат";
            }
            if (MoveCommand is not null)
            {
                return "сделали ход";
            }
            else
            {
                return "отправили всем поле";
            }
        }
    }

    public class ChatCommand 
    {
        public string Text { get; private set; } = "";
        public string Author { get; private set; } = "";

        public ChatCommand(Message message)
        {
            Text = message.Text;
            Author = message.Author;
        }
    }

    public class MoveCommand 
    {
        public int Index {  get; private set; }
        public string Element { get; private set; } = "";

        public MoveCommand(int index, string element)
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
