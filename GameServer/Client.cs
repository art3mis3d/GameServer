using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	/// <summary>
	/// Class to store all client information and communication protocol
	/// </summary>
	class Client
	{
		public static readonly int dataBufferSize = 4096;
		public int id;
		public TCP tcp;
		public UDP udp;

		public Client(int clientId)
		{
			id = clientId;
			tcp = new TCP(id);
			udp = new UDP(id);
		}

		/// <summary>
		/// TCP socket and communication API.
		/// </summary>
		public class TCP
		{
			public TcpClient socket;

			private readonly int _id;
			private NetworkStream _stream;
			private Packet _receivedData;
			private byte[] _receiveBuffer;

			public TCP(int _id)
			{
				this._id = _id;
			}

			/// <summary>
			/// Client's TCP connect method for tcp communication.
			/// </summary>
			/// <param name="_socket">The <see cref="TcpClient"/> to pass in</param>
			public void Connect(TcpClient _socket)
			{
				socket = _socket;
				socket.ReceiveBufferSize = dataBufferSize;

				_stream = socket.GetStream();

				_receivedData = new Packet();
				_receiveBuffer = new byte[dataBufferSize];

				_stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

				ServerSend.Welcome(_id, "Welcome to the server!");
			}

			public void SendData(Packet packet)
			{
				try
				{
					if (socket != null)
					{
						_stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error sending data to player {_id} via TCP: {ex}");
				}
			}

			private void ReceiveCallback(IAsyncResult result)
			{
				try
				{
					int byteLength = _stream.EndRead(result);
					if (byteLength <= 0)
					{
						// TODO: Disconnect
						return;
					}

					byte[] data = new byte[byteLength];
					Array.Copy(_receiveBuffer, data, byteLength);

					_receivedData.Reset(HandleData(data));
					_stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error receiving TCP Data: {ex}");
					// TODO: Disconnect.
				}
			}

			private bool HandleData(byte[] data)
			{
				int packetLength = 0;

				_receivedData.SetBytes(data);

				if (_receivedData.UnreadLength() >= 4)
				{
					packetLength = _receivedData.ReadInt();
					if (packetLength <= 0)
					{
						return true;
					}
				}

				while (packetLength > 0 && packetLength <= _receivedData.UnreadLength())
				{
					byte[] packetBytes = _receivedData.ReadBytes(packetLength);
					ThreadManager.ActionExecuteOnMainThread(() =>
					{
						using (Packet packet = new Packet(packetBytes))
						{
							int packetId = packet.ReadInt();
							Server.packetHandlers[packetId](_id, packet);
						}
					});

					packetLength = 0;

					if (_receivedData.UnreadLength() >= 4)
					{
						packetLength = _receivedData.ReadInt();
						if (packetLength <= 0)
						{
							return true;
						}
					}
				}

				if (packetLength <= 1)
				{
					return true;
				}

				return false;
			}
		}

		public class UDP
		{
			public IPEndPoint endPoint;

			private int _id;

			public UDP(int id)
			{
				_id = id;
			}

			public void Connect(IPEndPoint iPEndPoint)
			{
				endPoint = iPEndPoint;
				ServerSend.UDPTest(_id);
			}

			public void SendData(Packet packet)
			{
				Server.SendUDPData(endPoint, packet);
			}

			public void HandleData(Packet packetData)
			{
				int packetLength = packetData.ReadInt();
				byte[] packetBytes = packetData.ReadBytes(packetLength);

				ThreadManager.ActionExecuteOnMainThread(() =>
				{
					using (Packet packet = new Packet(packetBytes))
					{
						int packetId = packet.ReadInt();
						Server.packetHandlers[packetId](_id, packet);
					}
				});
			}
		}
	}
}
