using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocketSwitcher
{
	class Program
	{
		static List<Socket> clients = new List<Socket>();
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			Socket socket_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket_server.Bind(new IPEndPoint(0, 20));
			socket_server.Listen(0);
			while (true)
			{
				Socket client = socket_server.Accept();
				Console.WriteLine($"IP:{client.RemoteEndPoint.ToString()} 链接了!");
				DoWork(client);
			}
		}

		static async void DoWork(Socket client)
		{
			var buff = new byte[8 + 1024];
			int buffsize = 0;
			clients.Add(client);
			while (client.Connected)
			{
				try
				{
					buffsize = await client.ReceiveAsync(buff, SocketFlags.None);
				}
				catch (Exception err)
				{
					Console.WriteLine(err.Message);
					break;
				}
				foreach (var c in clients)
				{
					if (!c.Connected) continue;
					if (c == client) continue;
					var buff2 = new byte[buffsize];
					Array.Copy(buff, buff2, buffsize);
					await c.SendAsync(new ReadOnlyMemory<byte>(buff2), SocketFlags.None);
				}
			}
			Console.WriteLine($"IP:{client.RemoteEndPoint.ToString()} 断开了!");
			clients.Remove(client);
		}

	}//End Class
}
