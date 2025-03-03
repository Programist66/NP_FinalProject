using System.Net.Sockets;
using System.Net;
using FinalProjectLib;
using TcpLib;
using System;
using CommandLib;

namespace FinalProjectServer
{
    public class Server
    {
        static async Task Main(string[] args) => await new Server().RunAsync();

        private TcpListener listener = null!;
        private List<Member> members = [];

        private GameField gameField = new();
        
        private TextChecker checker = TextChecker.Instance();

        private async Task RunAsync()
        {
            listener = new TcpListener(IPAddress.Any, 6596);
            listener.Start();
            Console.WriteLine("Сервер запущен и слушает на порту 6596");
            int count = 0;

            while (count < 2)
            {
                count++;
                Console.WriteLine(count);
                TcpClient client = await listener.AcceptTcpClientAsync();
                Member member = new Member(client, count);
                Console.WriteLine($"Подключился клиент {member}");
                lock (members)
                {
                    members.Add(member);
                }                


                if (count == 1)
                {
                    lock (gameField)
                    {
                        gameField.CrossPlayer = member;
                        gameField.CurrentPlayer = member;
                    }
                }
                else
                {
                    lock (gameField)
                    {
                        gameField.ZeroPlayer = member;
                    }
                }
                //_ = ListenToClient(member);
                _ = Task.Run(() => {_ = ListenToClient(member); });          
            }
            Command command;
            lock (gameField) 
            {
                gameField.Status = GameStatus.Process;
                command = new Command(new GameFieldCommand(gameField));
            }
            
            command.StatusCode = StatusCode.OK;
            await SendCommandToAll(command);
        }
        private async Task ListenToClient(Member member)
        {
            string name = await member.TcpClient.ReceiveString();

            member.Name = name;

            if (member.Id == 1)
            {
                await member.TcpClient.SendString("X");
            }
            else
            {
                await member.TcpClient.SendString("O");
            }
            await member.TcpClient.SendInt32(member.Id);
            while (true)
            {
                lock (gameField)
                {
                    if (gameField.Status != GameStatus.Process)
                    {
                        continue;
                    }
                }
                Command command = await member.TcpClient.ReceiveAsJson<Command>();
                Console.WriteLine(command);
                if (command.ChatCommand is not null) 
                {
                    if (command.ChatCommand.Text == "" || command.ChatCommand.Author == "")
                    {
                        command.StatusCode = StatusCode.IncorrectArgument;
                        await member.TcpClient.SendAsJson(command);
                        continue;
                    }
                    if (checker.ContainsBannedWord(command.ChatCommand.Text))
                    {
                        command.StatusCode = StatusCode.BanWord;
                        await member.TcpClient.SendAsJson(command);
                        continue;
                    }
                    command.StatusCode = StatusCode.OK;
                    await SendCommandToAll(command);
                }
                else if (command.MoveCommand is not null) 
                {
                    IReadOnlyList<Cell> cellsCopy;
                    lock (gameField)
                    {
                        cellsCopy = gameField.cells.ToArray();
                    }
                    bool isCurrentPlayer = false;
                    lock (gameField)
                    {
                        if (gameField.CurrentPlayer!.Id == member.Id)
                        {
                            isCurrentPlayer = true;
                        }
                    }
                    if (command.MoveCommand.Index == 0 || command.MoveCommand.Element == "" || command.MoveCommand.Index >= cellsCopy.Count)
                    {
                        command.StatusCode = StatusCode.IncorrectArgument;
                        await member.TcpClient.SendAsJson(command);
                        continue;
                    }
                    if (!isCurrentPlayer)
                    {
                        command.StatusCode = StatusCode.NotYourMove;
                        await member.TcpClient.SendAsJson(command);
                        continue;
                    }
                    if (cellsCopy[command.MoveCommand.Index].Value == "")
                    {
                        await Task.Run(() => UpdateGameField(command.MoveCommand.Index, command.MoveCommand.Element));
                    }
                    else
                    {
                        command.StatusCode = StatusCode.IncorrectMove;
                    }
                    command.StatusCode = StatusCode.OK;
                    await member.TcpClient.SendAsJson(command);

                    lock (gameField)
                    {
                        gameField.SwitchCurrentPlayer();
                    }

                    command = new Command(new GameFieldCommand(gameField));
                    command.StatusCode = StatusCode.OK;
                    await SendCommandToAll(command);
                }
                else
                {
                    command.StatusCode = StatusCode.IncorrectCommand;
                    await member.TcpClient.SendAsJson(command);
                }
            }
        }

        private async Task SendCommandToAll(Command command) 
        {
            Console.WriteLine(command);
            IReadOnlyList<Member> membersCopy;
            lock (members)
            {
                membersCopy = members.ToArray();
            }
            List<Task> tasks = [];
            foreach (Member member in membersCopy)  
                tasks.Add(member.TcpClient.SendAsJson<Command>(command));
            await Task.WhenAll(tasks);
        }

        private void UpdateGameField(int index, string Symbol)
        {
            int size = 25;

            IReadOnlyList<Cell> cells;

            lock (gameField)
            {
                cells = gameField.cells.ToArray();
            }

            cells[index].Value = Symbol;

            bool isDraw = true;

            lock (gameField)
            {
                gameField.cells = cells.ToList();
            }

            foreach (Cell cell in cells)
            {
                if (cell.Value == "")
                {
                    isDraw = false;
                }
            }

            if (isDraw)
            {
                lock (gameField)
                {
                    gameField.Status = GameStatus.Draw;
                    return;
                }                
            }

            bool CheckLine(int startIndex, int deltaRow, int deltaCol)
            {
                int count = 0;
                string? current = null;

                for (int i = 0; i < 5; i++)
                {
                    int row = (startIndex / size) + i * deltaRow;
                    int col = (startIndex % size) + i * deltaCol;
                    int index = row * size + col;

                    if (row >= 0 && row < size && col >= 0 && col < size)
                    {
                        if (cells[index].Value == current && current != null)
                        {
                            count++;
                        }
                        else
                        {
                            current = cells[index].Value;
                            count = (current == "X" || current == "O") ? 1 : 0;
                        }

                        if (count == 5)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                return false;
            }

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].Value == "X" || cells[i].Value == "O")
                {                    
                    if (CheckLine(i, 0, 1) ||   // Горизонтально
                        CheckLine(i, 1, 0) ||   // Вертикально
                        CheckLine(i, 1, 1) ||   // Диагональ вниз вправо
                        CheckLine(i, 1, -1))    // Диагональ вниз влево
                    {
                        if (cells[i].Value == "X")
                        {
                            lock (gameField)
                            {
                                gameField.Status = GameStatus.CrossWin;
                                return;
                            }
                        }
                        else if (cells[i].Value == "O")
                        {
                            lock (gameField)
                            {
                                gameField.Status = GameStatus.ZeroWin;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
