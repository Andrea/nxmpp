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

using System.Xml.Linq;

namespace NXmpp.Protocol.Tls {
	/*
	 * <proceed xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>
	*/
	internal class StartTlsResponse : NonDirectedStanza {
		private StartTlsResponse() {
			Proceed = false;
		}

		internal bool Proceed { get; private set; }

		internal static StartTlsResponse CreateFromXml(XElement element) {
			var startTlsResponse = new StartTlsResponse();
			if (element.Name == XName.Get("proceed", NamespaceStrings.XmppTls)) {
				startTlsResponse.Proceed = true;
			}
			return startTlsResponse;
		}
	}
}