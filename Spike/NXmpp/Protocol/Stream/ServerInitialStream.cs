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
using System.Xml;

namespace NXmpp.Protocol.Stream {
	internal class ServerInitialStream {
		private ServerInitialStream() {
			Version = new Version(0, 0); //if no version is supplied the default is 0.0.
		}

		internal JId From { get; private set; }

		internal string Id { get; private set; }

		internal Version Version { get; private set; }

		internal static ServerInitialStream CreateFromXml(XmlReader reader) {
			var serverInitialStream = new ServerInitialStream();
			if (reader.NodeType != XmlNodeType.Element && reader.LocalName != "stream") {
				throw new XmlNotWellFormedException("expecting stream element");
			}
			if (reader.MoveToAttribute("version")) {
				serverInitialStream.Version = new Version(reader.Value);
			}
			if (reader.MoveToAttribute("from")) {
				serverInitialStream.From = JId.Parse(reader.Value);
			}
			if (reader.MoveToAttribute("id")) {
				serverInitialStream.Id = reader.Value;
			}
			return serverInitialStream;
		}
	}
}