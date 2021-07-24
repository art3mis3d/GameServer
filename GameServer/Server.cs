using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
	/// <summary>
	/// Class which houses all the server logic
	/// </summary>
	class Server
	{
		public static int MaxPlayers { get; private set; }
		public static int Port { get; private set; }
		public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
		private static TcpListener tcpListener;

		/// <summary>
		/// Method to start the server
		/// </summary>
		/// <param name="maxPlayers">The maximum no of concurrent players</param>
		/// <param name="port">The port to start the server listening on</param>
		public static void Start(int maxPlayers, int port)
		{
			MaxPlayers = maxPlayers;
			Port = port;

			Console.WriteLine("Starting Server...");
			InitializeServerData();

			tcpListener = new TcpListener(IPAddress.Any, Port);
			tcpListener.Start();
			tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

			Console.WriteLine($"Server started on port {Port}.");
		}

		private static void TCPConnectCallback(IAsyncResult result)
		{
			TcpClient _Client = tcpListener.EndAcceptTcpClient(result);
			tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
			Console.WriteLine($"Incoming connection from {_Client.Client.RemoteEndPoint}...");

			for (int i = 1; i <= MaxPlayers; i++)
			{
				if (clients[i].tcp.socket == null)
				{
					clients[i].tcp.Connect(_Client);
					return;
				}
			}

			Console.WriteLine($"{_Client.Client.RemoteEndPoint} failed to connect: Server full!");
		}

		private static void InitializeServerData()
		{
			for (int i = 1; i <= MaxPlayers; i++)
			{
				clients.Add(i, new Client(i));	
			}
		}
	}
}
