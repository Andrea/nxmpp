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
using System.Text;
using System.Xml.Linq;

namespace NXmpp.Protocol.Sasl {
	internal class SaslChallenge : NonDirectedStanza {
		internal string ChallangeString { get; private set; }

		/*
			<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
			cmVhbG09InNvbWVyZWFsbSIsbm9uY2U9Ik9BNk1HOXRFUUdtMmhoIixxb3A9ImF1dGgiLGNoYXJzZXQ9dXRmLTgsYWxnb3JpdGhtPW1kNS1zZXNzCg==
			</challenge>
		 */
		internal static SaslChallenge CreateFromXml(XElement xDocument) {
			byte[] buffer = Convert.FromBase64String(xDocument.Value);
			var challenge = new SaslChallenge {ChallangeString = Encoding.UTF8.GetString(buffer, 0, buffer.Length)};
			return challenge;
		}
	}
}