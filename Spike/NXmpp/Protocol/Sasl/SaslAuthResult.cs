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

using System.Linq;
using System.Xml.Linq;

namespace NXmpp.Protocol.Sasl {
	internal class SaslAuthResult : NonDirectedStanza {
		private SaslAuthResult() {}
		internal bool Success { get; private set; }

		internal string FailReason { get; private set; }

		internal static SaslAuthResult CreateFromXml(XElement element) {
			var saslAuthResult = new SaslAuthResult();
			if (element.Name == XName.Get("success", NamespaceStrings.XmppSasl)) {
				saslAuthResult.Success = true;
			}
			else {
				saslAuthResult.Success = false;
				saslAuthResult.FailReason = element.Elements().First().Name.LocalName;
			}
			return saslAuthResult;
		}
	}
}