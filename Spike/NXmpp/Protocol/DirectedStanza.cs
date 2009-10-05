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

namespace NXmpp.Protocol {
	internal abstract class DirectedStanza : Stanza {
		protected DirectedStanza() {}

		protected DirectedStanza(JId from, JId to, string id) {
			From = from;
			To = to;
		}

		internal JId To { get; set; }

		internal JId From { get; set; }

		internal string Id { get; set; }
	}
}