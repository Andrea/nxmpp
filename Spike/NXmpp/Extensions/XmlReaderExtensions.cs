﻿#region License

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
	internal static class XmlReaderExtensions {
		/// <summary>
		/// Reads to the next element in the stream
		/// </summary>
		/// <param name="reader">The xml reader</param>
		public static void ReadToNextElement(this XmlReader reader) {
			ReadToNextElement(reader, true);
		}

		/// <summary>
		/// Reads to the next element in the stream
		/// </summary>
		/// <param name="reader">The xml reader</param>
		/// <param name="skipCurrent">true to read to next element if current location is already an element. false returns current element</param>
		public static void ReadToNextElement(this XmlReader reader, bool skipCurrent) {
			if (reader.NodeType == XmlNodeType.Element && skipCurrent) {
				reader.Read();
			}
			while (reader.NodeType != XmlNodeType.Element) {
				reader.Read();
			}
		}
	}
}