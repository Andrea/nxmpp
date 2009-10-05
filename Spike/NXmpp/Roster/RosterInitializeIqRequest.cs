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
using System.Xml.Linq;
using NXmpp.Protocol;
using NXmpp.Protocol.Iq;

namespace NXmpp.Roster {
	internal class RosterInitializeIqRequest : IqRequest<RosterInitializeIqResponse> {
		internal RosterInitializeIqRequest(Action<RosterInitializeIqResponse> responseCompleteCallback, string id) : base(IqRequestType.Get, responseCompleteCallback, id) {}

		internal RosterInitializeIqResponse ParseResponse(XElement element) {
			var rosterIQResponse = new RosterInitializeIqResponse();
			rosterIQResponse.Parse(element);
			return rosterIQResponse;
		}

		internal override XElement AsXElement() {
			var element = new XElement("iq",
			                           new XAttribute("id", Id),
			                           new XAttribute("type", IqRequestType.ToString().ToLower()),
			                           new XElement(XName.Get("query", NamespaceStrings.XmppRoster)));
			return element;
		}
	}
}