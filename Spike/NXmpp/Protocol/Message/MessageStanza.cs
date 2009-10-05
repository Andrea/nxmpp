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

using System.Xml;

namespace NXmpp.Protocol.Message {
	internal class MessageStanza : DirectedStanza {

		private readonly string body;

		internal MessageStanza(JId from, JId to, string id, string body) : base(from, to, id) {
			this.body = body;
		}

		//public override string ToString() {
		//    return XmlToString(xmlWriter => {
		//        xmlWriter.WriteStartElement("message");

		//        xmlWriter.WriteAttribute("from", From.ToString());
		//        xmlWriter.WriteAttribute("to", To.ToString());
		//        xmlWriter.WriteAttribute("id", Id);
		//        xmlWriter.WriteAttribute("type", "chat");

		//        xmlWriter.WriteStartElement("body");
		//        xmlWriter.WriteValue(body);
		//        xmlWriter.WriteEndElement();
		//        xmlWriter.WriteEndElement();
		//                       });
		//}
	}
}