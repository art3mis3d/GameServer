﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	/// <summary>Sent from server to client.</summary>
	public enum ServerPackets
	{
		Welcome = 1,
		UdpTest
	}

	/// <summary>Sent from client to server.</summary>
	public enum ClientPackets
	{
		WelcomeReceived = 1,
		UdpTestReceived
	}

	public class Packet : IDisposable
	{
		private List<byte> _buffer;
		private byte[] _readableBuffer;
		private int _readPos;

		/// <summary>Creates a new empty packet (without an ID).</summary>
		public Packet()
		{
			_buffer = new List<byte>(); // Intitialize buffer
			_readPos = 0; // Set readPos to 0
		}

		/// <summary>Creates a new packet with a given ID. Used for sending.</summary>
		/// <param name="id">The packet ID.</param>
		public Packet(int id)
		{
			_buffer = new List<byte>(); // Intitialize buffer
			_readPos = 0; // Set readPos to 0

			Write(id); // Write packet id to the buffer
		}

		/// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
		/// <param name="data">The bytes to add to the packet.</param>
		public Packet(byte[] data)
		{
			_buffer = new List<byte>(); // Intitialize buffer
			_readPos = 0; // Set readPos to 0

			SetBytes(data);
		}

		#region Functions

		/// <summary>Sets the packet's content and prepares it to be read.</summary>
		/// <param name="data">The bytes to add to the packet.</param>
		public void SetBytes(byte[] data)
		{
			Write(data);
			_readableBuffer = _buffer.ToArray();
		}

		/// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
		public void WriteLength()
		{
			_buffer.InsertRange(0, BitConverter.GetBytes(_buffer.Count)); // Insert the byte length of the packet at the very beginning
		}

		/// <summary>Inserts the given int at the start of the buffer.</summary>
		/// <param name="value">The <see cref="int"/> to insert.</param>
		public void InsertInt(int value)
		{
			_buffer.InsertRange(0, BitConverter.GetBytes(value)); // Insert the int at the start of the buffer
		}

		/// <summary>Gets the packet's content in array form.</summary>
		public byte[] ToArray()
		{
			_readableBuffer = _buffer.ToArray();
			return _readableBuffer;
		}

		/// <summary>Gets the length of the packet's content.</summary>
		public int Length()
		{
			return _buffer.Count; // Return the length of buffer
		}

		/// <summary>Gets the length of the unread data contained in the packet.</summary>
		public int UnreadLength()
		{
			return Length() - _readPos; // Return the remaining length (unread)
		}

		/// <summary>Resets the packet instance to allow it to be reused.</summary>
		/// <param name="shouldReset">Whether or not to reset the packet.</param>
		public void Reset(bool shouldReset = true)
		{
			if (shouldReset)
			{
				_buffer.Clear(); // Clear buffer
				_readableBuffer = null;
				_readPos = 0; // Reset readPos
			}
			else
			{
				_readPos -= 4; // "Unread" the last read int
			}
		}

		#endregion

		#region Write Data

		/// <summary>Adds a <see cref="byte"/> to the packet.</summary>
		/// <param name="value">The <see cref="byte"/> to add.</param>
		public void Write(byte value)
		{
			_buffer.Add(value);
		}
		/// <summary>Adds an array of <see cref="byte"/>s to the packet.</summary>
		/// <param name="value">The <see cref="byte"/> array to add.</param>
		public void Write(byte[] value)
		{
			_buffer.AddRange(value);
		}
		/// <summary>Adds a <see cref="short"/> to the packet.</summary>
		/// <param name="value">The <see cref="short"/> to add.</param>
		public void Write(short value)
		{
			_buffer.AddRange(BitConverter.GetBytes(value));
		}
		/// <summary>Adds an <see cref="int"/> to the packet.</summary>
		/// <param name="value">The <see cref="int"/> to add.</param>
		public void Write(int value)
		{
			_buffer.AddRange(BitConverter.GetBytes(value));
		}
		/// <summary>Adds a <see cref="long"/> to the packet.</summary>
		/// <param name="value">The <see cref="long"/> to add.</param>
		public void Write(long value)
		{
			_buffer.AddRange(BitConverter.GetBytes(value));
		}
		/// <summary>Adds a <see cref="float"/> to the packet.</summary>
		/// <param name="value">The <see cref="float"/> to add.</param>
		public void Write(float value)
		{
			_buffer.AddRange(BitConverter.GetBytes(value));
		}
		/// <summary>Adds a <see cref="bool"/> to the packet.</summary>
		/// <param name="value">The <see cref="bool"/> to add.</param>
		public void Write(bool value)
		{
			_buffer.AddRange(BitConverter.GetBytes(value));
		}
		/// <summary>Adds a <see cref="string"/> to the packet.</summary>
		/// <param name="value">The <see cref="string"/> to add.</param>
		public void Write(string value)
		{
			Write(value.Length); // Add the length of the string to the packet
			_buffer.AddRange(Encoding.ASCII.GetBytes(value)); // Add the string itself
		}

		#endregion

		#region Read Data

		/// <summary>Reads a <see cref="byte"/> from the packet.</summary>
		/// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
		public byte ReadByte(bool moveReadPos = true)
		{
			if (_buffer.Count > _readPos)
			{
				// If there are unread bytes
				byte value = _readableBuffer[_readPos]; // Get the byte at readPos' position
				if (moveReadPos)
				{
					// If _moveReadPos is true
					_readPos += 1; // Increase readPos by 1
				}
				return value; // Return the byte
			}
			else
			{
				throw new Exception("Could not read value of type 'byte'!");
			}
		}

		/// <summary>Reads an array of <see cref="byte"/>s from the packet.</summary>
		/// <param name="length">The length of the <see cref="byte"/> array.</param>
		/// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
		public byte[] ReadBytes(int length, bool moveReadPos = true)
		{
			if (_buffer.Count > _readPos)
			{
				// If there are unread bytes
				byte[] value = _buffer.GetRange(_readPos, length).ToArray(); // Get the bytes at readPos' position with a range of length
				if (moveReadPos)
				{
					// If _moveReadPos is true
					_readPos += length; // Increase readPos by length
				}
				return value; // Return the bytes
			}
			else
			{
				throw new Exception("Could not read value of type 'byte[]'!");
			}
		}

		/// <summary>Reads a <see cref="short"/> from the packet.</summary>
		/// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
		public short ReadShort(bool moveReadPos = true)
		{
			if (_buffer.Count > _readPos)
			{
				// If there are unread bytes
				short value = BitConverter.ToInt16(_readableBuffer, _readPos); // Convert the bytes to a short
				if (moveReadPos)
				{
					// If _moveReadPos is true and there are unread bytes
					_readPos += 2; // Increase readPos by 2
				}
				return value; // Return the short
			}
			else
			{
				throw new Exception("Could not read value of type 'short'!");
			}
		}

		/// <summary>Reads an <see cref="int"/> from the packet.</summary>
		/// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
		public int ReadInt(bool moveReadPos = true)
		{
			if (_buffer.Count > _readPos)
			{
				// If there are unread bytes
				int value = BitConverter.ToInt32(_readableBuffer, _readPos); // Convert the bytes to an int
				if (moveReadPos)
				{
					// If _moveReadPos is true
					_readPos += 4; // Increase readPos by 4
				}
				return value; // Return the int
			}
			else
			{
				throw new Exception("Could not read value of type 'int'!");
			}
		}

		/// <summary>Reads a <see cref="long"/> from the packet.</summary>
		/// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
		public long ReadLong(bool moveReadPos = true)
		{
			if (_buffer.Count > _readPos)
			{
				// If there are unread bytes
				long value = BitConverter.ToInt64(_readableBuffer, _readPos); // Convert the bytes to a long
				if (moveReadPos)
				{
					// If _moveReadPos is true
					_readPos += 8; // Increase readPos by 8
				}
				return value; // Return the long
			}
			else
			{
				throw new Exception("Could not read value of type 'long'!");
			}
		}

		/// <summary>Reads a <see cref="float"/> from the packet.</summary>
		/// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
		public float ReadFloat(bool moveReadPos = true)
		{
			if (_buffer.Count > _readPos)
			{
				// If there are unread bytes
				float value = BitConverter.ToSingle(_readableBuffer, _readPos); // Convert the bytes to a float
				if (moveReadPos)
				{
					// If _moveReadPos is true
					_readPos += 4; // Increase readPos by 4
				}
				return value; // Return the float
			}
			else
			{
				throw new Exception("Could not read value of type 'float'!");
			}
		}

		/// <summary>Reads a <see cref="bool"/> from the packet.</summary>
		/// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
		public bool ReadBool(bool moveReadPos = true)
		{
			if (_buffer.Count > _readPos)
			{
				// If there are unread bytes
				bool value = BitConverter.ToBoolean(_readableBuffer, _readPos); // Convert the bytes to a bool
				if (moveReadPos)
				{
					// If _moveReadPos is true
					_readPos += 1; // Increase readPos by 1
				}
				return value; // Return the bool
			}
			else
			{
				throw new Exception("Could not read value of type 'bool'!");
			}
		}

		/// <summary>Reads a <see cref="string"/> from the packet.</summary>
		/// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
		public string ReadString(bool moveReadPos = true)
		{
			try
			{
				int length = ReadInt(); // Get the length of the string
				string value = Encoding.ASCII.GetString(_readableBuffer, _readPos, length); // Convert the bytes to a string
				if (moveReadPos && value.Length > 0)
				{
					// If _moveReadPos is true string is not empty
					_readPos += length; // Increase readPos by the length of the string
				}
				return value; // Return the string
			}
			catch
			{
				throw new Exception("Could not read value of type 'string'!");
			}
		}

		#endregion

		private bool _disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_buffer = null;
					_readableBuffer = null;
					_readPos = 0;
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
