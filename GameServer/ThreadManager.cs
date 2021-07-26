using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	class ThreadManager
	{
		private static readonly List<Action> ExecuteOnMainThread = new List<Action>();
		private static readonly List<Action> ExecuteCopiedOnMainThread = new List<Action>();
		private static bool _actionToExecuteOnMainThread;

		/// <summary>Sets an action to be executed on the main thread.</summary>
		/// <param name="action">The action to be executed on the main thread.</param>
		public static void ActionExecuteOnMainThread(Action action)
		{
			if (action == null)
			{
				Console.WriteLine("No action to execute on main thread!");
				return;
			}

			lock (ExecuteOnMainThread)
			{
				ExecuteOnMainThread.Add(action);
				_actionToExecuteOnMainThread = true;
			}
		}

		/// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
		public static void UpdateMain()
		{
			if (_actionToExecuteOnMainThread)
			{
				ExecuteCopiedOnMainThread.Clear();
				lock (ExecuteOnMainThread)
				{
					ExecuteCopiedOnMainThread.AddRange(ExecuteOnMainThread);
					ExecuteOnMainThread.Clear();
					_actionToExecuteOnMainThread = false;
				}

				for (int i = 0; i < ExecuteCopiedOnMainThread.Count; i++)
				{
					ExecuteCopiedOnMainThread[i]();
				}
			}
		}
	}
}
