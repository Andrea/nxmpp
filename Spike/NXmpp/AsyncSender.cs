using System;
using System.Collection.Generic;
using System.Threading;
using NXmpp.Net;
using NXmpp.Protocol;

namespace NXmpp {
	internal class StanzaSender : IDisposable {
		private readonly IXmppConnection _connection;

		private readonly BlockingQueue<Stanza> _sendQueue = new BlockingQueue<Stanza>();
		private readonly Thread _sendThread;
		private bool _isDisposed;
		private bool _stopLoop;

		internal StanzaSender(IXmppConnection connection) {
			_connection = connection;
			_sendThread = new Thread(SendLoop) { Name = "SendStanzaLoop", IsBackground = true };
			_sendThread.Start();
		}

		private void SendLoop(object state) {
			Stanza stanza = null;
			while (!_stopLoop) {
				if (_sendQueue.Dequeue(100, ref stanza)) {
					_connection.Send(stanza);
				}
			}
		}

		#region IDisposable Members

		public void Dispose() {
			if (_isDisposed) return;
			_stopLoop = true;
			_sendThread.Join(1000);
			_isDisposed = true;
		}

		#endregion
	}
}
