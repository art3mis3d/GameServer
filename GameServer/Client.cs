using System;
using System.Collections.Generic;
using System.Linq;
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

		public Client(int clientId)
		{
			id = clientId;
			tcp = new TCP(id);
		}

		/// <summary>
		/// TCP socket and communication API.
		/// </summary>
		public class TCP
		{
			public TcpClient socket;

			private readonly int id;
			private NetworkStream stream;
			private byte[] receiveBuffer;

			public TCP(int _id)
			{
				id = _id;
			}

			/// <summary>
			/// Client's TCP connect method for tcp communication.
			/// </summary>
			/// <param name="_socket">The <see cref="TcpClient"/> to pass in</param>
			public void Connect(TcpClient _socket)
			{
				socket = _socket;
				socket.ReceiveBufferSize = dataBufferSize;

				stream = socket.GetStream();

				receiveBuffer = new byte[dataBufferSize];

				stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

				// TODO: Send welcome packet.
			}

			private void ReceiveCallback(IAsyncResult result)
			{
				try
				{
					int byteLength = stream.EndRead(result);
					if (byteLength <= 0)
					{
						// TODO: Disconnect
						return;
					}

					byte[] data = new byte[byteLength];
					Array.Copy(receiveBuffer, data, byteLength);

					// TODO: Handle data.
					stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error receiving TCP Data: {ex}");
					// TODO: Disconnect.
				}
			}
		}
	}
}
