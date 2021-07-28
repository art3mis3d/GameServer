using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	class Player
	{
		public int Id {  get; private set; }
		public string Username {  get; private set; }

		public Vector3 position;
		public Quaternion rotation;

		private float _moveSpeed = 5f / Constants.TICKS_PER_SEC;
		private Vector2 _inputDir;

		public Player(int id, string username, Vector3 spawnPosition)
		{
			Id = id;
			Username = username;
			position = spawnPosition;
			rotation = Quaternion.Identity;
		}

		public void Update()
		{
			Move(_inputDir);
		}

		private void Move(Vector2 inputDir)
		{
			Vector3 forward = Vector3.Transform(new Vector3(0, 0, 1), rotation);
			Vector3 right = Vector3.Normalize(Vector3.Cross(forward, new Vector3(0, 1, 0)));

			Vector3 moveDirection = right * inputDir.X + forward * inputDir.Y;
			position += moveDirection * _moveSpeed;

			ServerSend.PlayerPosition(this);
			ServerSend.PlayerRotation(this);
		}

		public void SetInput(Vector2 inputVector, Quaternion inputRot)
		{
			_inputDir = inputVector;
			rotation = inputRot;
		}
	}
}
