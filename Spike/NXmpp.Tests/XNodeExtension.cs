using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace NXmpp.Tests {
	/// <summary>
	/// The various comparison options.
	/// </summary>
	[Flags]
	public enum ComparisonOptions {
		/// <summary>
		/// The prefix used for the namespace will be taken into account in the comparison.
		/// </summary>
		NamespacePrefix = 1,

		/// <summary>
		/// The ordering of attributes will be taken into account in the comparison.
		/// </summary>
		AttributeOrdering = 2,

		/// <summary>
		/// Comments and notations will be taken into account in the comparison.
		/// </summary>
		CommentsAndNotations = 4,

		/// <summary>
		/// The style of an empty tag, whether it is a single tag with closing and
		/// opening inside it or two tags, one opening and one closing will be taken
		/// into account.
		/// </summary>
		EmptyTagStyle = 16,

		/// <summary>
		/// The element ordering will be taken into account when comparing.
		/// </summary>
		ElementOrdering = 32,

		/// <summary>
		/// A comparison using the XML standards for nodes.
		/// </summary>
		StandardsBased = ComparisonOptions.ElementOrdering | ComparisonOptions.EmptyTagStyle,

		/// <summary>
		/// The Microsoft method of comparing elements.
		/// </summary>
		MicrosoftImplementation = ComparisonOptions.NamespacePrefix | ComparisonOptions.AttributeOrdering | ComparisonOptions.CommentsAndNotations | ComparisonOptions.EmptyTagStyle | ComparisonOptions.ElementOrdering,

		/// <summary>
		/// A comparison that compares on semantic differences only.
		/// </summary>
		SemanticCompare = 0
	}

	/// <summary>
	/// Extension methods on XNodes.
	/// </summary>
	public static class XNodeExtension {

		#region Extension Methods
		/// <summary>
		/// Compares two comments using the indicated comparison options.
		/// </summary>
		/// <param name="c1">The first comment to compare.</param>
		/// <param name="c2">The second comment to compare.</param>
		/// <param name="options">The options to use in the comparison.</param>
		/// <returns>true if the comments are equal, false otherwise.</returns>
		public static bool DeepEquals(this XComment c1, XComment c2, ComparisonOptions options) {
			if ((c1 ?? c2) == null)
				return true;
			if ((c1 == null) || (c2 == null))
				return false; // They are not both null, so if either is, then the other isn't

			return c1.Value == c2.Value;
		}

		/// <summary>
		/// Compares two processing instructions using the indicated comparison options.
		/// </summary>
		/// <param name="p1">The first processing instruction to compare.</param>
		/// <param name="p2">The second processing instruction to compare.</param>
		/// <param name="options">The options to use in the comparison.</param>
		/// <returns>true if the processing instructions are equal, false otherwise.</returns>
		public static bool DeepEquals(this XProcessingInstruction p1, XProcessingInstruction p2, ComparisonOptions options) {
			if ((p1 ?? p2) == null)
				return true;
			if ((p1 == null) || (p2 == null))
				return false; // They are not both null, so if either is, then the other isn't

			return ((p1.Target == p2.Target) && (p1.Data == p1.Data));
		}

		/// <summary>
		/// Compares two texts using the indicated comparison options.
		/// </summary>
		/// <param name="t1">The first text to compare.</param>
		/// <param name="t2">The second text to compare.</param>
		/// <param name="options">The options to use in the comparison.</param>
		/// <returns>true if the texts are equal, false otherwise.</returns>
		public static bool DeepEquals(this XText t1, XText t2, ComparisonOptions options) {
			if ((t1 ?? t2) == null)
				return true;
			if ((t1 == null) || (t2 == null))
				return false; // They are not both null, so if either is, then the other isn't

			return ((t1.NodeType == t2.NodeType) && (t1.Value == t1.Value));
		}

		/// <summary>
		/// Compares two elements using the indicated comparison options.
		/// </summary>
		/// <param name="e1">The first element to compare.</param>
		/// <param name="e2">The second element to compare.</param>
		/// <param name="options">The options to use in the comparison.</param>
		/// <returns>true if the elements are equal, false otherwise.</returns>
		public static bool DeepEquals(this XElement e1, XElement e2, ComparisonOptions options) {
			if ((e1 ?? e2) == null)
				return true;
			if ((e1 == null) || (e2 == null))
				return false; // They are not both null, so if either is, then the other isn't

			// Compare the name
			if (!CompareNames(e1.Name, e2.Name, options))
				return false;

			// Performance tweak: compare attributes first, because they will often be faster to find a difference
			return AttributesEqual(e1, e2, options) && ContentsEqual(e1, e2, options);
		}

		/// <summary>
		/// Compares two documents using the indicated comparison options.
		/// </summary>
		/// <param name="d1">The first document to compare.</param>
		/// <param name="d2">The second document to compare.</param>
		/// <param name="options">The options to use in the comparison.</param>
		/// <returns>true if the documents are equal, false otherwise.</returns>
		public static bool DeepEquals(this XDocument d1, XDocument d2, ComparisonOptions options) {
			if ((d1 ?? d2) == null)
				return true;
			if ((d1 == null) || (d2 == null))
				return false; // They are not both null, so if either is, then the other isn't

			return DeepEquals(d1.Root, d2.Root, options);
		}

		/// <summary>
		/// Compares two attributes using the indicated comparison options.
		/// </summary>
		/// <param name="a1">The first attribute to compare.</param>
		/// <param name="a2">The second attribute to compare.</param>
		/// <param name="options">The options to use in the comparison.</param>
		/// <returns>true if the attributes are equal, false otherwise.</returns>
		public static bool DeepEquals(this XAttribute a1, XAttribute a2, ComparisonOptions options) {
			if ((a1 ?? a2) == null)
				return true;
			if ((a1 == null) || (a2 == null))
				return false; // They are not both null, so if either is, then the other isn't

			// Compare the name
			if (!CompareNames(a1.Name, a2.Name, options))
				return false;

			return a1.Value == a2.Value;
		}

		/// <summary>
		/// Compares two names.
		/// </summary>
		/// <param name="n1">The first name to compare.</param>
		/// <param name="n2">The second name to compare.</param>
		/// <param name="options">The options to use in the comparison.</param>
		/// <returns>true if the elements are equal, false otherwise.</returns>
		private static bool CompareNames(XName n1, XName n2, ComparisonOptions options) {
			// Compare the name
			if ((n1.LocalName != n2.LocalName) || (n1.Namespace != n2.Namespace))
				return false;
			else if ((options & ComparisonOptions.NamespacePrefix) == ComparisonOptions.NamespacePrefix)
				if (n1.NamespaceName != n2.NamespaceName)
					return false;

			return true;
		}

		/// <summary>
		/// Checks if two elements have equal attributes.
		/// </summary>
		/// <param name="e1">The first element to compare.</param>
		/// <param name="e2">The second element to compare.</param>
		/// <param name="options">The options to use in the comparison.</param>
		/// <returns>true if the element attributes are equal, false otherwise.</returns>
		private static bool AttributesEqual(XElement e1, XElement e2, ComparisonOptions options) {
			// They must both have or not have attributes, and if they don't we're good and can exit early
			if (e1.HasAttributes != e2.HasAttributes)
				return false;
			if (!e1.HasAttributes)
				return true;

			var attributes1 = e1.Attributes();
			var attributes2 = e2.Attributes();

			// We will ignore attribute ordering
			if ((options & ComparisonOptions.AttributeOrdering) != ComparisonOptions.AttributeOrdering) {
				attributes1 = attributes1.OrderBy(x => x.Name.Namespace.NamespaceName + x.Name.LocalName);
				attributes2 = attributes2.OrderBy(x => x.Name.Namespace.NamespaceName + x.Name.LocalName);
			}

			var enum1 = attributes1.GetEnumerator();
			var enum2 = attributes2.GetEnumerator();
			var next1 = enum1.MoveNext();
			var next2 = enum2.MoveNext();

			while (next1 && next2) {
				if (enum1.Current.IsNamespaceDeclaration != enum2.Current.IsNamespaceDeclaration)
					return false;
				if (!enum1.Current.IsNamespaceDeclaration || ((options & ComparisonOptions.NamespacePrefix) == ComparisonOptions.NamespacePrefix))
					if (!DeepEquals(enum1.Current, enum2.Current, options))
						return false;

				next1 = enum1.MoveNext();
				next2 = enum2.MoveNext();
			}

			// They have the same number of elements if these are equal
			return next1 == next2;
		}

		/// <summary>
		/// Checks if two elements have equal content.
		/// </summary>
		/// <param name="e1">The first element to compare.</param>
		/// <param name="e2">The second element to compare.</param>
		/// <param name="options">The options to use in the comparison.</param>
		/// <returns>true if the element contents are equal, false otherwise.</returns>
		private static bool ContentsEqual(XElement e1, XElement e2, ComparisonOptions options) {
			if (e1.Value != e2.Value)
				return false;
			if ((e1.IsEmpty != e2.IsEmpty) && ((options & ComparisonOptions.EmptyTagStyle) == ComparisonOptions.EmptyTagStyle))
				return false;
			if (e1.IsEmpty && e2.IsEmpty)
				return true;

			var nodes1 = e1.Nodes();
			var nodes2 = e2.Nodes();

			// Exclude comments and notations
			if ((options & ComparisonOptions.CommentsAndNotations) != ComparisonOptions.CommentsAndNotations) {
				nodes1 = nodes1.Where(x => x.NodeType != XmlNodeType.Comment && x.NodeType != XmlNodeType.Notation);
				nodes2 = nodes2.Where(x => x.NodeType != XmlNodeType.Comment && x.NodeType != XmlNodeType.Notation);
			}

			// Order the nodes if required
			if ((options & ComparisonOptions.ElementOrdering) != ComparisonOptions.ElementOrdering) {
				nodes1 = nodes1.OrderBy(x => x.ToString());
				nodes2 = nodes2.OrderBy(x => x.ToString());
			}

			var enum1 = nodes1.GetEnumerator();
			var enum2 = nodes2.GetEnumerator();
			var next1 = enum1.MoveNext();
			var next2 = enum2.MoveNext();

			while (next1 && next2) {
				if (!DeepEquals(enum1.Current, enum2.Current, options))
					return false;

				next1 = enum1.MoveNext();
				next2 = enum2.MoveNext();
			}

			// They have the same number of elements if these are equal
			return next1 == next2;
		}

		/// <summary>
		/// Compares two nodes using the indicated comparison options.
		/// </summary>
		/// <param name="n1">The first node to compare.</param>
		/// <param name="n2">The second node to compare.</param>
		/// <param name="options">The options to use in the comparison.</param>
		/// <returns>true if the nodes are equal, false otherwise.</returns>
		public static bool DeepEquals(this XNode n1, XNode n2, ComparisonOptions options) {
			if ((n1 ?? n2) == null)
				return true;
			if ((n1 == null) || (n2 == null))
				return false; // They are not both null, so if either is, then the other isn't

			if ((n1 is XElement) && (n2 is XElement))
				return DeepEquals((XElement)n1, (XElement)n2, options);
			else if ((n1 is XComment) && (n2 is XComment))
				return DeepEquals((XComment)n1, (XComment)n2, options);
			else if ((n1 is XText) && (n2 is XText))
				return DeepEquals((XText)n1, (XText)n2, options);
			else if ((n1 is XProcessingInstruction) && (n2 is XProcessingInstruction))
				return DeepEquals((XProcessingInstruction)n1, (XProcessingInstruction)n2, options);
			else if (!(n1.GetType().IsAssignableFrom(n2.GetType()) || n2.GetType().IsAssignableFrom(n1.GetType())))
				return false;
			else
				throw new NotImplementedException();
		}

		/// <summary>
		/// Reads data from a stream until the end is reached. The
		/// data is returned as a byte array. An IOException is
		/// thrown if any of the underlying IO calls fail.
		/// </summary>
		/// <param name="stream">The stream to read data from</param>
		// From Jon Skeet: http://www.yoda.arachsys.com/csharp/readbinary.html
		private static byte[] ReadFully(Stream stream) {
			byte[] buffer = new byte[32768];
			using (MemoryStream ms = new MemoryStream()) {
				while (true) {
					int read = stream.Read(buffer, 0, buffer.Length);
					if (read <= 0)
						return ms.ToArray();
					ms.Write(buffer, 0, read);
				}
			}
		}

		/// <summary>
		/// Reads data from a stream until the end is reached. The
		/// data is returned as a byte array. An IOException is
		/// thrown if any of the underlying IO calls fail.
		/// </summary>
		/// <param name="stream">The stream to read data from</param>
		/// <param name="initialLength">The initial buffer length</param>
		// From Jon Skeet: http://www.yoda.arachsys.com/csharp/readbinary.html
		private static byte[] ReadFully(Stream stream, int initialLength) {
			// If we've been passed an unhelpful initial length, just
			// use 32K.
			if (initialLength < 1) {
				initialLength = 32768;
			}

			byte[] buffer = new byte[initialLength];
			int read = 0;

			int chunk;
			while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0) {
				read += chunk;

				// If we've reached the end of our buffer, check to see if there's
				// any more information
				if (read == buffer.Length) {
					int nextByte = stream.ReadByte();

					// End of stream? If so, we're done
					if (nextByte == -1) {
						return buffer;
					}

					// Nope. Resize the buffer, put in the byte we've just
					// read, and continue
					byte[] newBuffer = new byte[buffer.Length * 2];
					Array.Copy(buffer, newBuffer, buffer.Length);
					newBuffer[read] = (byte)nextByte;
					buffer = newBuffer;
					read++;
				}
			}
			// Buffer is now too big. Shrink it.
			byte[] ret = new byte[read];
			Array.Copy(buffer, ret, read);
			return ret;
		}

		/// <summary>
		/// A very fast XNode comparison that implements the non-standard Microsoft implementation
		/// of XNodeEqualityComparer. This is effectively a string compare that ignores whitespace.
		/// </summary>
		/// <param name="stream1">The first XML stream.</param>
		/// <param name="stream2">The second XML stream.</param>
		/// <returns>True if the two documents are equal, false otherwise.</returns>
		public unsafe static bool FastMicrosoftXNodeCompare(Stream stream1, Stream stream2) {
			var buffer1 = ReadFully(stream1);
			var buffer2 = ReadFully(stream2);

			var count1 = buffer1.Length;
			var count2 = buffer2.Length;

			fixed (byte* bufPtr1 = buffer1)
			fixed (byte* bufPtr2 = buffer2) {
				byte* buf1End = bufPtr1 + buffer1.Length;
				byte* buf2End = bufPtr2 + buffer2.Length;
				byte* position1 = bufPtr1;
				byte* position2 = bufPtr2;

				while ((count1 > 0) && (count2 > 0)) {
					if (*position1 != *position2) {
						if ((*position1 != 32) && (*position2 != 32)) // Ignore spaces
							if ((*position1 != 39) && (*position2 != 39)) // Different quoting styles "/'
								return false;
							else if ((*position1 + *position2) != 73) // The two together must == '+"
								return false;
						while (*position1 == 32) {
							position1++;
							count1--;
						}
						while (*position2 == 32) {
							position2++;
							count2--;
						}
					}
					else {
						position1++;
						position2++;
						count1--;
						count2--;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// A very fast XNode comparison that implements the non-standard Microsoft implementation
		/// of XNodeEqualityComparer. This is effectively a string compare that ignores whitespace.
		/// </summary>
		/// <param name="file1">The first XML file.</param>
		/// <param name="file2">The second XML file.</param>
		/// <returns>True if the two documents are equal, false otherwise.</returns>
		public static bool FastMicrosoftXNodeCompare(string file1, string file2) {
			using (var stream1 = new FileStream(file1, FileMode.Open, FileAccess.Read, FileShare.Read, 10000, FileOptions.SequentialScan))
			using (var stream2 = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.Read, 10000, FileOptions.SequentialScan))
				return FastMicrosoftXNodeCompare(stream1, stream2);
		}
		#endregion

	}
}