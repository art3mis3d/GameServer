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
		/// Method to send Data to a <see cref="Client"/> via UDP
		/// </summary>
		/// <param name="toClient">The client id to send the packet to</param>
		/// <param name="packet">The <see cref="Packet"/> to send</param>
		private static void SendUDPData(int toClient, Packet packet)
		{
			packet.WriteLength();
			Server.clients[toClient].udp.SendData(packet);
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
		/// Method to send Data to all <see cref="Client"/>s via UDP
		/// </summary>
		/// <param name="packet">The <see cref="Packet"/> to send</param>
		private static void SendUDPDataToAll(Packet packet)
		{
			packet.WriteLength();
			for (int i = 1; i <= Server.MaxPlayers; i++)
			{
				Server.clients[i].udp.SendData(packet);
			}
		}

		/// <summary>
		/// Method to send Data to all <see cref="Client"/>s except one via UDP
		/// </summary>
		/// <param name="exceptClient">The client id to exclude from the send</param>
		/// <param name="packet">The <see cref="Packet"/> to send</param>
		private static void SendUDPDataToAll(int exceptClient, Packet packet)
		{
			packet.WriteLength();
			for (int i = 1; i <= Server.MaxPlayers; i++)
			{
				if (i != exceptClient)
				{
					Server.clients[i].udp.SendData(packet);
				}
			}
		}

		#region Packets

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

		public static void SpawnPlayer(int toClient, Player player)
		{
			using (Packet packet = new Packet((int)ServerPackets.SpawnPlayer))
			{
				packet.Write(player.Id);
				packet.Write(player.Username);
				packet.Write(player.position);
				packet.Write(player.rotation);

				SendTCPData(toClient, packet);
			}
		}

		public static void PlayerPosition(Player player)
		{
			using (Packet packet = new Packet((int)ServerPackets.PlayerPosition))
			{
				packet.Write(player.Id);
				packet.Write(player.position);

				SendUDPDataToAll(packet);
			}
		}

		public static void PlayerRotation(Player player)
		{
			using (Packet packet = new Packet((int)ServerPackets.PlayerRotation))
			{
				packet.Write(player.Id);
				packet.Write(player.rotation);

				SendUDPDataToAll(player.Id, packet);
			}
		}

		#endregion
	}
}
