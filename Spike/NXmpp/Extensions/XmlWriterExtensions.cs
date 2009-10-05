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

namespace System.Xml {
	// http://elegantcode.com/2009/06/19/refactoring-xmlwriter/

	public static class XmlWriterExtensions {
		public static void WriteElementBlock(this XmlWriter xmlWriter, string localName, Action action) {
			xmlWriter.WriteStartElement(localName);
			action.Invoke();
			xmlWriter.WriteEndElement();
			xmlWriter.Flush();
		}

		public static void WriteElementBlock(this XmlWriter xmlWriter, string localName, string ns, Action action) {
			xmlWriter.WriteStartElement(localName, ns);
			action.Invoke();
			xmlWriter.WriteEndElement();
			xmlWriter.Flush();
		}

		public static void WriteElementBlock(this XmlWriter xmlWriter, string localName, Action<XmlWriter> action) {
			xmlWriter.WriteStartElement(localName);
			action.Invoke(xmlWriter);
			xmlWriter.WriteEndElement();
			xmlWriter.Flush();
		}

		public static void WriteElementBlock(this XmlWriter xmlWriter, string localName, string ns, Action<XmlWriter> action) {
			xmlWriter.WriteStartElement(localName, ns);
			action.Invoke(xmlWriter);
			xmlWriter.WriteEndElement();
			xmlWriter.Flush();
		}

		public static void WriteAttribute(this XmlWriter xmlWriter, string localName, string value) {
			xmlWriter.WriteStartAttribute(localName);
			xmlWriter.WriteValue(value);
			xmlWriter.WriteEndAttribute();
		}
	}
}