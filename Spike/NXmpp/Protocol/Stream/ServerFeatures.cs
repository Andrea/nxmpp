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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NXmpp.Protocol.Stream {
	/*
	<stream:features>
		<starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'>
			<required/>
		</starttls>
		<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
			<mechanism>DIGEST-MD5</mechanism>
			<mechanism>PLAIN</mechanism>
		</mechanisms>
		<bind xmlns='urn:ietf:params:xml:ns:xmpp-bind'/>
		<session xmlns='urn:ietf:params:xml:ns:xmpp-session'/>
	</stream:features>
	*/

	internal class ServerFeatures : DirectedStanza {
		private ServerFeatures() {
			TLSSupport = TLSSupportType.NotSpecified;
		}

		internal TLSSupportType TLSSupport { get; private set; }

		internal IEnumerable<string> Mechanisms { get; private set; }

		internal bool Bind { get; private set; }

		internal bool Session { get; private set; }

		internal static ServerFeatures CreateFromXml(XElement element) {
			var serverFeatures = new ServerFeatures();
			XElement chile = element.Descendants(XName.Get("starttls", NamespaceStrings.XmppTls)).FirstOrDefault();
			if (chile != null) {
				serverFeatures.TLSSupport = chile.Element(XName.Get("required", NamespaceStrings.XmppTls)) != null ?
				                                                                                                             	TLSSupportType.Required : TLSSupportType.SupportedNotRequired;
			}
			serverFeatures.Mechanisms = (from mechanism in element.Descendants(XName.Get("mechanism", NamespaceStrings.XmppSasl)) select mechanism.Value).ToList();

			chile = element.Descendants(XName.Get("bind", NamespaceStrings.XmppBind)).FirstOrDefault();
			if (chile != null) {
				serverFeatures.Bind = true;
			}

			chile = element.Descendants(XName.Get("session", NamespaceStrings.XmppSession)).FirstOrDefault();
			if (chile != null) {
				serverFeatures.Session = true;
			}

			return serverFeatures;
		}

		#region Nested type: TLSSupportType

		internal enum TLSSupportType {
			Required,
			SupportedNotRequired,
			NotSpecified
		}

		#endregion
	}
}