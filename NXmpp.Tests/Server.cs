using System;
using System.Net.Sockets;
using System.Net;

namespace NXmpp.Tests
{
	/// <summary>
	/// Really simple server for testing purposes. Can only handle a single client.
	/// </summary>
	internal class Server : IDisposable
	{
		private readonly TcpListener _tcpListener;
		private TcpClient _tcpClient;

		/// <summary>
		/// Server on default port 4000.
		/// </summary>
		internal Server() : this(4000) {}


		/// <summary>
		/// Server on custom port
		/// </summary>
		/// <param name="port">Port number to listen to</param>
		internal Server(ushort port)
		{
			_tcpListener = new TcpListener(IPAddress.Loopback, port);
			_tcpListener.BeginAcceptTcpClient(AcceptTcpClient, null);
		}

		private void AcceptTcpClient(IAsyncResult asyncResult)
		{
			_tcpClient = _tcpListener.EndAcceptTcpClient(asyncResult);
			NetworkStream = _tcpClient.GetStream();
		}

		/// <summary>
		/// The network stream of 
		/// </summary>
		internal NetworkStream NetworkStream { get; private set;}

		#region IDisposable Members

		public void Dispose()
		{
			if (_tcpClient != null && _tcpClient.Connected)
			{
				_tcpClient.Close();
			}

		}

		#endregion
	}
}
