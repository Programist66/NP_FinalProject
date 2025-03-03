using FinalProjectLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectClient.HelperClasses
{
    public class ConnectionData
    {
        public int ID {  get; }
        public TcpClient Server { get;}
        public string Name { get;}
        public string PlayerSymbvol { get;}

        public ConnectionData(TcpClient server, int id, string name, string playerSymbvol)
        {
            Server = server;
            Name = name;
            ID = id;
            PlayerSymbvol = playerSymbvol;
        }
    }
}
