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
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Common.Logging;
using NXmpp.NCommon;
using NXmpp.Protocol;
using NXmpp.Protocol.Stream;
using NXmpp.Schemas;

namespace NXmpp.Net {
	internal class XmppConnection :IXmppConnection {
		private readonly Encoding _encoding;
		private readonly ILog _log;
		private readonly object _receiveLock = new object();
		private readonly object _sendLock = new object();
		private readonly Socket _socket;
		private readonly XmlReaderSettings _xmlReaderSettings;
		private readonly Version _protocolVersion;
		private bool _isDisposed;
		private NetworkStream _networkStream;
		private Stream _stream;
		private XmlReader _xmlReader;

		internal XmppConnection(XmppHost xmppHost, Version protocolVersion, ILog log) {
			Host = xmppHost;
			_protocolVersion = protocolVersion;
			_log = log;
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			_encoding = new UTF8Encoding(false);
			_xmlReaderSettings = new XmlReaderSettings {IgnoreComments = true, IgnoreProcessingInstructions = true, IgnoreWhitespace = true, ProhibitDtd = true, NameTable = new NameTable(), Schemas = {XmlResolver = new CustomXmlResolver()}};
		}

		#region IXmppConnection Members
		public void Secure(TlsCertificateCallback tlsCertificateCallback) {
			Guard.Against<ObjectDisposedException>(_isDisposed, "Object Disposed");
			var sslStream = new SslStream(_stream, true, (sender, certificate, chain, sslPolicyErrors) => tlsCertificateCallback(certificate, chain, sslPolicyErrors));
			sslStream.AuthenticateAsClient(Host.HostName);
			_stream = sslStream;
		}

		public void Compress() {
			throw new NotImplementedException("not implemented");
		}

		public void Dispose() {
			if(_isDisposed) return;
			if(_stream != null) {
				SendCore(new ClientCloseStream().AsXElement());
				_stream.Close();
				_socket.Close();
			}
			_isDisposed = true;
		}

		public void InitializeStream() {
			_networkStream.ReadTimeout = 30000;
			SendCore((new ClientInitialStream(JId.Parse(Host.HostName), _protocolVersion)).ToString());
			try {
				_xmlReader = XmlReader.Create(_stream, _xmlReaderSettings);
				_xmlReader.Read();
				if (_xmlReader.NodeType == XmlNodeType.XmlDeclaration) {
					_xmlReader.Read();
				}
				ServerInitialStream serverInitialStream = ServerInitialStream.CreateFromXml(_xmlReader);
				if (serverInitialStream.Version.Major < _protocolVersion.Major - 1) {
					throw new UnsupportedVersionException();
				}
				_networkStream.ReadTimeout = -1;
			}
			catch (IOException ex) {
				//typically thrown if read timeout occurs
				_log.Trace(ex);
				throw;
			}
		}

		public void Send(XElement stanza) {
			Guard.Against<ObjectDisposedException>(_isDisposed, "Object Disposed");
			SendCore(stanza);
		}

		public XElement Receive() {
			lock (_receiveLock) {
				Guard.Against<ObjectDisposedException>(_isDisposed, "Object Disposed");
				return ReceiveCore();
			}
		}

		public void Send(Stanza stanza) {
			Send(stanza.AsXElement());
		}
		#endregion

		public void Connect() {
			Guard.Against<ObjectDisposedException>(_isDisposed, "Object Disposed");
			Guard.Against<InvalidOperationException>(_socket.Connected, "Already connected");
			_socket.Connect(Host.IPAddress, Host.Port);
			_networkStream = new NetworkStream(_socket, false);
			_stream = _networkStream;
		}

		private XElement ReceiveCore() {
			_xmlReader.ReadToNextElement();
			XmlReader subTreeReader = _xmlReader.ReadSubtree();
			subTreeReader.Read();
			string stanzaString = subTreeReader.ReadOuterXml();
			_log.Trace("C REC:" + stanzaString);
			return XElement.Parse(stanzaString);
		}

		private void SendCore(XNode element) {
			SendCore(element.ToString(SaveOptions.DisableFormatting));
		}

		private void SendCore(string data) {
			byte[] bytes = _encoding.GetBytes(data);
			_stream.Write(bytes, 0, bytes.Length);
			_stream.Flush();
			_log.Trace("C SND:" + data);
		}

		#region IXmppConnection Members

		public XmppHost Host { get; private set; }

		#endregion
	}
}