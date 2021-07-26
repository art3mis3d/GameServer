using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	/// <summary>
	/// Class housing all methods to create and send <see cref="Packet"/>s from the server
	/// </summary>
	class ServerSend
	{
		/// <summary>
		/// Method to send Data to a <see cref="Client"/> via TCP
		/// </summary>
		/// <param name="toClient">The client id to send the packet to</param>
		/// <param name="packet">The <see cref="Packet"/> to send</param>
		private static void SendTCPData(int toClient, Packet packet)
		{
			packet.WriteLength();
			Server.clients[toClient].tcp.SendData(packet);
		}

		/// <summary>
		/// Method to send Data to all <see cref="Client"/>s via TCP
		/// </summary>
		/// <param name="packet">The <see cref="Packet"/> to send</param>
		private static void SendTCPDataToAll(Packet packet)
		{
			packet.WriteLength();
			for (int i = 1; i <= Server.MaxPlayers; i++)
			{
				Server.clients[i].tcp.SendData(packet);
			}
		}

		/// <summary>
		/// Method to send Data to all <see cref="Client"/>s except one via TCP
		/// </summary>
		/// <param name="exceptClient">The client id to exclude from the send</param>
		/// <param name="packet">The <see cref="Packet"/> to send</param>
		private static void SendTCPDataToAll(int exceptClient, Packet packet)
		{
			packet.WriteLength();
			for (int i = 1; i <= Server.MaxPlayers; i++)
			{
				if (i != exceptClient)
				{
					Server.clients[i].tcp.SendData(packet);
				}
			}
		}

		/// <summary>
		/// Packet welcoming client to the server
		/// </summary>
		/// <param name="toClient">The Client ID to write on the packet</param>
		/// <param name="msg">The message to write on the packet</param>
		public static void Welcome(int toClient, string msg)
		{
			using (Packet packet = new Packet((int)ServerPackets.Welcome))
			{
				packet.Write(msg);
				packet.Write(toClient);

				SendTCPData(toClient, packet);
			}
		}
	}
}
