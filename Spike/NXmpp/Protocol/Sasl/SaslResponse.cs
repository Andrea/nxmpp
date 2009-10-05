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
	internal class SaslResponse : NonDirectedStanza {
		private readonly string _response;

		internal SaslResponse(string response) {
			var encoding = new UTF8Encoding(false);
			byte[] buffer = encoding.GetBytes(response);
			_response = Convert.ToBase64String(buffer);
		}

		internal override XElement AsXElement() {
			return new XElement(XName.Get("response", NamespaceStrings.XmppSasl),
			                    new XText(_response));
		}
	}
}