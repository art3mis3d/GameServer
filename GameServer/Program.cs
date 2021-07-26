using System;
using System.Threading;

namespace GameServer
{
	class Program
	{
		private static bool isRunning;
		static void Main(string[] args)
		{
			Console.Title = "Game Server";
			isRunning = true;

			Thread mainThread = new Thread(new ThreadStart(MainThread));
			mainThread.Start();

			Server.Start(50, 26950);
		}

		private static void MainThread()
		{
			Console.WriteLine($"Main thread has started. Running at {Constants.ticksPerSec} ticks per second");
			DateTime nextloop = DateTime.Now;

			while (isRunning)
			{
				while (nextloop < DateTime.Now)
				{
					GameLogic.Update();

					nextloop = nextloop.AddMilliseconds(Constants.msPerTick);

					if (nextloop > DateTime.Now)
					{
						Thread.Sleep(nextloop - DateTime.Now);
					}
				}
			}
		}
	}
}
