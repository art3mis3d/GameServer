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
		public delegate void PacketHandler(int fromClient, Packet packet);
		public static Dictionary<int, PacketHandler> packetHandlers;

		private static TcpListener tcpListener;
		private static UdpClient udpListener;

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
			tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

			udpListener = new UdpClient(port);
			udpListener.BeginReceive(UDPReceiveCallback, null);


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

		private static void UDPReceiveCallback(IAsyncResult result)
		{
			try
			{
				IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
				udpListener.BeginReceive(UDPReceiveCallback, null);

				if (data.Length < 4)
				{
					return;
				}

				using (Packet packet = new Packet(data))
				{
					int clientId = packet.ReadInt();

					if (clientId == 0)
					{
						return;
					}

					if (clients[clientId].udp.endPoint == null)
					{
						clients[clientId].udp.Connect(clientEndPoint);
						return;
					}

					if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString())
					{
						clients[clientId].udp.HandleData(packet);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error receving UDP data: {ex}");
			}
		}

		public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
		{
			try
			{
				if (clientEndPoint != null)
				{
					udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error sending data to {clientEndPoint} via udp: {ex}");
			}
		}

		private static void InitializeServerData()
		{
			for (int i = 1; i <= MaxPlayers; i++)
			{
				clients.Add(i, new Client(i));
			}

			packetHandlers = new Dictionary<int, PacketHandler>()
			{
				{ (int)ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived },
				{ (int)ClientPackets.UdpTestReceived, ServerHandle.UDPTestReceived },
			};
			Console.WriteLine("Initialized Packets");
		}
	}
}
