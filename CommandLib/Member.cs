using System.Net;
using System.Net.Sockets;

namespace FinalProjectLib;

public class Member 
{
    public int Id {  get;}
    public string? Name { get; set; }
    public TcpClient TcpClient { get; }
    public IPEndPoint IPEndPoint => (IPEndPoint) TcpClient.Client.RemoteEndPoint!;

    public Member (TcpClient tcpClient, int id) 
    {
        TcpClient = tcpClient;
        Id = id;
    }

    public override string ToString () => $"{Name} {IPEndPoint}";
}
