#region License

// Copyright 2009 Damian Hickey
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may
// not use this file except in compliance with the License. You may obtain a
// copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License. 

#endregion

using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using NXmpp.ConnectionStates;
using NXmpp.Dns;
using NXmpp.Dns.Windows;
using NXmpp.NCommon;
using NXmpp.Net;
using NXmpp.Protocol;
using NXmpp.Roster;
using NXmpp.StanzaHandler;
using System.Threading;

namespace NXmpp {
	public delegate bool TlsCertificateCallback(X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors);

	public class XmppClient : IDisposable {
		private readonly IXmppConnection _connection;
		private readonly ConnectionStatesEnumerable _connectionStatesEnumerable;
		private bool _isDisposed;
		private readonly Dictionary<string, IStanzaHandler> _stanzaHandlers = new Dictionary<string, IStanzaHandler>();
		private readonly IXmppService _xmppService;
		private readonly IXmppContext _xmppContext;
		private Thread _readStanzaThread;
		private readonly RosterManager _rosterManager;
		private readonly Dictionary<string, IStanzaHandler> iqRequests = new Dictionary<string, IStanzaHandler>();

		public XmppClient(XmppClientSettings xmppClientSettings) {
			Guard.AgainstArgumentNullException(() => xmppClientSettings);
			Settings = xmppClientSettings;
			_xmppContext = new XmppContext(this);
			_connectionStatesEnumerable = new ConnectionStatesEnumerable(_xmppContext);
			_xmppService = new XmppService(this);
			_rosterManager = new RosterManager(_xmppService);
			Roster = _rosterManager;
			_stanzaHandlers.Add(_rosterManager.Namespace, _rosterManager);
			MainLoop = new MainLoop("XmppClient");
		}

		//internal StanzaSender StanzaSender { get; private set;}

		internal MainLoop MainLoop { get; private set; }

		internal XmppClientSettings Settings { get; private set; }

		internal IXmppConnection Connection { get; private set; }

		internal JId JId { get; private set; }

		public ConnectionStateType ConnectionState { get; private set; }

		#region IDisposable Members

		public void Dispose() {
			if (_isDisposed) return;
			MainLoop.Dispose();
			Settings.Log.Trace("Disposing");
			if (_connection != null) {
				//_connection.Send(PresenceStanza.CreateBroadcastUnavilable(JId).AsXElement()); Do via rostermanager
				_connection.Dispose();
			}
			if(_readStanzaThread != null && _readStanzaThread.IsAlive) {
				_readStanzaThread.Abort();
				_readStanzaThread.Join();
			}
			_isDisposed = true;
		}

		#endregion

		public event EventHandler<EventArgs<ConnectionStateType>> ConnectionStateChanged;

		private void OnConnectionStateChanged(ConnectionStateType connectionStateType) {
			ConnectionState = connectionStateType;
			EventHandler<EventArgs<ConnectionStateType>> connectionStateChanged = ConnectionStateChanged;
			if (connectionStateChanged != null) {
				connectionStateChanged(this, new EventArgs<ConnectionStateType>(connectionStateType));
			}
		}

		public void Connect() {
			CheckDisposed();
			MainLoop.Queue(() => {
				foreach (ConnectionStateBase connectionState in _connectionStatesEnumerable) {
					connectionState.Handle();
				}
			});
		}

		public event EventHandler<EventArgs<string>> MessageReceived;

		public IRoster Roster { get; private set; }

		private void OnMessageReceived(string message) {
			EventHandler<EventArgs<string>> messageReceivedTemp = MessageReceived;
			if (messageReceivedTemp != null) {
				messageReceivedTemp(this, new EventArgs<string>(message));
			}
		}

		public event EventHandler<SubscriptionRequestEventArgs> SubscriptionRequested;

		private SubscriptionRequestEventArgs OnSubscriptionRequested(JId from) {
			EventHandler<SubscriptionRequestEventArgs> subscriptionRequestedTemp = SubscriptionRequested;
			var subscriptionRequestEventArgs = new SubscriptionRequestEventArgs(from);
			if (subscriptionRequestedTemp != null) {
				SubscriptionRequested(this, subscriptionRequestEventArgs);
			}
			return subscriptionRequestEventArgs;
		}

		private void CheckDisposed() {
			if (_isDisposed) {
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		private void StartAsync() {
			_readStanzaThread = new Thread(ReadStanzaWorker) {Name = "ReadStanzaWorker", IsBackground = true};
			_readStanzaThread.Start();
		}

		private void ReadStanzaWorker() {
			try {
				while (true) {
					XElement element = Connection.Receive();
					MainLoop.Queue(() => {
					               	if (element.Name.LocalName == "iq" && (element.Attribute("type").Value == "result" || element.Attribute("type").Value == "error")) {
					               		IStanzaHandler stanzaHandler = iqRequests[element.Attribute("id").Value];
					               		stanzaHandler.HandleIQStanza(element);
					               	}
					               }
						);
				}
			}
			catch(ThreadAbortException){}
		}

		#region Nested type: XmppContext

		private class XmppContext : IXmppContext {
			private readonly XmppClient _client;

			internal XmppContext(XmppClient client) {
				_client = client;
				ConnectionFactory = new XmppConnectionFactory();
				DnsServerLookupFactory = new WmiDnsServerLookupFactory(); //TODO: consider this to be part of configuration
			}

			#region IXmppContext Members

			public XmppClientSettings Settings {
				get { return _client.Settings; }
			}

			public void OnConnectionStateChanged(ConnectionStateType e) {
				_client.OnConnectionStateChanged(e);
			}

			public IXmppConnection Connection {
				get { return _client.Connection; }
				set { _client.Connection = value; }
			}

			public IXmppConnectionFactory ConnectionFactory { get; private set; }

			public IDnsServerLookupFactory DnsServerLookupFactory { get; private set; }

			public JId JId {
				get { return _client.JId; }
				set { _client.JId = value; }
			}

			public void StartAsync() {
				_client.StartAsync();
			}

			public void InitializeStanzaHandlers() {
				_client._rosterManager.Initialize();
			}

			#endregion
		}


		#endregion

		private class XmppService : IXmppService {

			private readonly XmppClient _client;

			internal XmppService(XmppClient client) {
				_client = client;
			}

			#region IXmppService Members

			public JId JId {
				get { return _client.JId; }
			}

			public void Send(IStanzaHandler sender, XElement element) {
				_client.MainLoop.Queue(() => {
						if (element.Name.LocalName == "iq") {
							_client.iqRequests.Add(element.Attribute("id").Value, sender);
						}
                        _client.Connection.Send(element);
					});

			}

			#endregion
		}
	}
}