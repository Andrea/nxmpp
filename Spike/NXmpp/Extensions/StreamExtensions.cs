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

using System.Text;

namespace System.IO {
	public static class StreamExtensions {
		/// <summary>
		/// Writes a string to a stream using UTF8 encoding.
		/// </summary>
		/// <param name="stream">The stream to be written to</param>
		/// <param name="s">The string to write to the stream</param>
		/// <param name="flush"></param>
		/// <param name="includeByteOrderMark">true to omit a utf-8 byte order mark</param>
		public static void WriteStringUtf8(this Stream stream, string s, bool flush, bool includeByteOrderMark) {
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			var encoding = new UTF8Encoding(includeByteOrderMark);
			byte[] b = encoding.GetBytes(s);
			stream.Write(b, 0, b.Length);
			if (flush) {
				stream.Flush();
			}
		}

		/// <summary>
		/// Writes a string to a stream using UTF8 encoding. Byte order mark is omitted.
		/// </summary>
		/// <param name="stream">The stream to be written to</param>
		/// <param name="s">The string to write to the stream</param>>
		/// <param name="flush">Flush the stream after writing the string</param>
		public static void WriteStringUtf8(this Stream stream, string s, bool flush) {
			WriteStringUtf8(stream, s, flush, true);
		}

		/// <summary>
		/// Writes a string to a stream using UTF8 encoding. Stream is flushed and byte order mark is omitted.
		/// </summary>
		/// <param name="stream">The stream to be written to</param>
		/// <param name="s">The string to write to the stream</param>>
		public static void WriteStringUtf8(this Stream stream, string s) {
			WriteStringUtf8(stream, s, true, true);
		}

		public static string ReadStringUtf8(this Stream stream, int bufferSize) {
			var b = new byte[bufferSize];
			int bytesRead = stream.Read(b, 0, b.Length);
			return Encoding.UTF8.GetString(b, 0, bytesRead);

		}
	}
}