using System.Globalization;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;

namespace TcpLib
{
    public static class TcpClientExtensions
    {
        #region Send Types
        public static async Task SendInt32(this TcpClient tcpClient, int @int) 
        {
            byte[] bytes = BitConverter.GetBytes(@int);
            await tcpClient.GetStream().WriteAsync(bytes);
        }

        public static async Task SendInt64(this TcpClient tcpClient, long @long)
        {
            byte[] bytes = BitConverter.GetBytes(@long);
            await tcpClient.GetStream().WriteAsync(bytes);
        }

        public static async Task SendBytes(this TcpClient tcpClient, byte[] bytes) 
        {
            await tcpClient.SendInt32(bytes.Length);
            await tcpClient.GetStream().WriteAsync(bytes);
        }

        public static async Task SendString(this TcpClient tcpClient, string @string)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(@string);
            await tcpClient.SendBytes(bytes);
        }

        public static async Task SendAsJson<T>(this TcpClient tcpClient, T t)
        {
            string json = JsonSerializer.Serialize(t);
            await tcpClient.SendString(json);
        }

        public static async Task SendFile(this TcpClient tcpClient, Stream stream)
        {
            long length = stream.Length;
            await tcpClient.SendInt64(length);
            await stream.CopyToAsync(tcpClient.GetStream());
        }

        public static async Task<bool> SendFileFromPath(this TcpClient tcpClient, string path) 
        {
            using Stream stream = File.OpenRead(path);
            string fileName = Path.GetFileName(path);
            await tcpClient.SendString(fileName);
            await tcpClient.SendFile(stream);
            return true;
        }

        public static async Task SendBinaryReader(this TcpClient tcpClient, Action<BinaryWriter> write) 
        {
            MemoryStream memory = new();
            BinaryWriter writer = new BinaryWriter(memory);
            write(writer);

            byte[] bytes = memory.ToArray();
            await tcpClient.SendBytes(bytes);
        }
        #endregion
        #region Receive Types
        public static async Task<int> ReceiveInt32(this TcpClient tcpClient) 
        {
            byte[] buffer = new byte[sizeof(int)];
            await tcpClient.GetStream().ReadExactlyAsync(buffer);
            return BitConverter.ToInt32(buffer);
        }

        public static async Task<long> ReceiveInt64(this TcpClient tcpClient)
        {
            byte[] buffer = new byte[sizeof(long)];
            await tcpClient.GetStream().ReadExactlyAsync(buffer);
            return BitConverter.ToInt64(buffer);
        }

        public static async Task<byte[]> ReceiveBytes(this TcpClient tcpClient) 
        {
            int length = await tcpClient.ReceiveInt32();
            byte[] buffer = new byte[length];
            await tcpClient.GetStream().ReadExactlyAsync(buffer);
            return buffer;
        }        

        public static async Task<string> ReceiveString(this TcpClient tcpClient) 
        {
            byte[] bytes = await tcpClient.ReceiveBytes();
            return Encoding.UTF8.GetString(bytes);
        }        

        public static async Task<T> ReceiveAsJson<T>(this TcpClient tcpClient) 
        {
            string json = await tcpClient.ReceiveString();
            return JsonSerializer.Deserialize<T>(json)
                ?? throw new NullReferenceException();
        }

        public static async Task ReceiveFile(this TcpClient tcpClient, Stream stream)
        {
            long length = await tcpClient.ReceiveInt64();
            byte[] buffer = new byte[1024];
            for (long copied = 0; copied < length;)
            {
                int read = await tcpClient.GetStream().ReadAsync(buffer, 0,
                    (int)Math.Min(buffer.Length, length - copied));
                await stream.WriteAsync(buffer, 0, read);
                copied += read;
            }
        }

        public static async Task<string> ReceiveFileToPath(this TcpClient tcpClient, string path = "") 
        {
            string name = await tcpClient.ReceiveString();
            string fullPath = Path.Combine(path, name);
            using Stream stream = File.Create(fullPath);
            await tcpClient.ReceiveFile(stream);
            return fullPath;
        }

        public static async Task<BinaryReader> ReceiveBinaryReader(this TcpClient tcpClient) 
        {
            byte[] bytes = await tcpClient.ReceiveBytes();

            MemoryStream memory = new MemoryStream(bytes);
            return new BinaryReader(memory);
        }
        #endregion
    }
}
