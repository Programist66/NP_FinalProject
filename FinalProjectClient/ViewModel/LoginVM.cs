using FinalProjectClient.HelperClasses;
using FinalProjectLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TcpLib;

namespace FinalProjectClient.ViewModel
{
    public class LoginVM
    {
        public string Name { get; set; } = "";
        public string IP { get; set; } = "";
        public int Port { get; set; }

        private LoginForm loginForm;

        public LoginVM(LoginForm loginForm)
        {
            this.loginForm = loginForm;
            ConnectCommand = new DelegateCommand(Connect);
        }

        public ICommand ConnectCommand { get; set; }
        public async void Connect()
        {
            if (Name == "" || IP == "")
            {
                MessageBox.Show("Неверно введены данные!!");
                return;
            }
            TcpClient server = new TcpClient();
            await server.ConnectAsync(IP, Port);

            await server.SendString(Name);

            string symbvol = await server.ReceiveString();
            int id = await server.ReceiveInt32();

            MessageBox.Show(id.ToString());

            ConnectionData connectionData = new(server, id, Name, symbvol);

            MainWindow mainWindow = new MainWindow(connectionData);

            loginForm.Hide();
            mainWindow.Show();
        }
    }
}
