using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	class GameLogic
	{
		public static void Update()
		{
			foreach (Client client in Server.clients.Values)
			{
				if (client.player != null)
				{
					client.player.Update();
				}
			}

			ThreadManager.UpdateMain();
		}
	}
}
