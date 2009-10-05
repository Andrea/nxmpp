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

using Common.Logging;
using Common.Logging.Simple;
using NXmpp.Dns;
using NXmpp.NCommon;

namespace NXmpp {
	public class XmppClientSettings {
		public JId UserJId { get; private set; }
		public string Password { get; private set; }
		public TlsCertificateCallback TlsCertificateCallback { get; private set; }
		public UseTLS UseTLS { get; private set; }
		public ILog Log { get; private set; }
		internal int Port { get { return 5222; }}

		private XmppClientSettings() {
			UseTLS = UseTLS.IfSupported;
			Log = new ConsoleOutLogger("XmppClient", LogLevel.All, true, true, true, "yyyy-MM-dd hh:mm:ss");
		}

		public XmppClientSettings(JId userJid, string password, TlsCertificateCallback tlsCertificateCallback) : this() {
			Guard.AgainstArgumentNullException(() => userJid);
			Guard.AgainstArgumentNullException(() => tlsCertificateCallback);
			UserJId = userJid;
			Password = password;
			TlsCertificateCallback = tlsCertificateCallback;
		}
	}
}