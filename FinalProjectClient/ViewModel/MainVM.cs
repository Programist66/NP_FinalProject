using CommandLib;
using FinalProjectClient.HelperClasses;
using FinalProjectLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TcpLib;

namespace FinalProjectClient.ViewModel
{
    public class MainVM : BindableBase
    {
        private string playerSymbvol;
        private string playerName;
        private int Id;
        private TcpClient server;

        private bool isMyMove = false;

        public string Text
        {
            get;
            set => SetProperty(ref field, value);
        } = "";

        private bool isWork = true;
        public GameField GameField
        {
            get;
            set => SetProperty(ref field, value);
        }

        public MainVM(ConnectionData connection)
        {
            playerSymbvol = connection.PlayerSymbvol;
            isMyMove = (playerSymbvol == "X");
            playerName = connection.Name;
            server = connection.Server;
            Id = connection.ID;
            GameField = new GameField();
            CloseCommand = new DelegateCommand(Close);
            SendChatCommand = new DelegateCommand(SendChat);
            MakeMoveCommand = new RelayCommand<int>(MakeMove);
            Task.Run(ServerListen);
        }


        public ObservableCollection<Message> ChatMessages { get; } = [];

        public Member? CurrentPlayer
        {
            get => GameField.CurrentPlayer!;
            set
            {
                GameField.CurrentPlayer = value;
                RaisePropertyChanged(nameof(CurrentPlayer));
            }
        }

        public ICommand MakeMoveCommand { get; set; }
        private async void MakeMove(int index)
        {
            if(GameField.cells[index].Value != "" && !isMyMove) 
            {
                return;
            }
            await server.SendAsJson(new Command(new MoveCommand(index, playerSymbvol)));
        }

        public ICommand CloseCommand { get; set; }
        private void Close()
        {
            isWork = false;
        }

        private async Task ServerListen()
        {
            while (isWork)
            {
                Command cmd = await server.ReceiveAsJson<Command>();
                if (cmd.ChatCommand is not null)
                {
                    if (cmd.StatusCode == StatusCode.OK)
                    {
                        ChatMessages.Add(new Message(cmd.ChatCommand));
                    }
                    else if(cmd.StatusCode == StatusCode.BanWord)
                    {
                        MessageBox.Show("В вашем сообщение есть бан ворды!!!!", "BanWord", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show($"Не предвиденная ошибка: {cmd.StatusCode!.Description}", "Error",MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (cmd.GameFieldCommand is not null)
                {
                    if (cmd.StatusCode == StatusCode.OK)
                    {
                        GameField = cmd.GameFieldCommand.GameField;
                        if (GameField.CurrentPlayer!.Id == Id)
                        {
                            isMyMove = true;
                        }
                        if (GameField.Status == GameStatus.CrossWin)
                        {
                            if(MessageBox.Show("Победил крестик!!") == MessageBoxResult.OK) 
                            {
                                App.Current.Shutdown();
                            }
                        }
                        else if (GameField.Status == GameStatus.ZeroWin)
                        {
                            if (MessageBox.Show("Победил нолик!!") == MessageBoxResult.OK)
                            {
                                App.Current.Shutdown();
                            }
                        }
                        else if (GameField.Status == GameStatus.Draw)
                        {
                            if (MessageBox.Show("Ничья!!") == MessageBoxResult.OK)
                            {
                                App.Current.Shutdown();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Не предвиденная ошибка: {cmd.StatusCode!.Description}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if(cmd.MoveCommand is not null)
                {
                    if (cmd.StatusCode == StatusCode.OK)
                    {
                        isMyMove = false;
                    }
                    else
                    {
                        MessageBox.Show($"Не предвиденная ошибка: {cmd.StatusCode!.Description}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public ICommand SendChatCommand { get; set; }
        private async void SendChat() 
        {
            if (Text == "")
            {
                return;
            }
            await server.SendAsJson(new Command(new ChatCommand(new Message(playerName, Text))));
            Text = "";
        }
    }
}
