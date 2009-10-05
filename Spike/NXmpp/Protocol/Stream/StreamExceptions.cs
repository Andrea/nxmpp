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

namespace NXmpp.Protocol.Stream {
	/// <summary>
	/// Base exception class for stream related errors. Stream errors are fatal.
	/// </summary>
	public abstract class StreamException : Exception {
		internal StreamException() {}

		internal StreamException(string message) : base(message) {}

		internal static void ThrowStreamException(string error) {
			switch (error) {
				case "bad-format":
					throw new BadFormatException();
				case "bad-namespace-prefix":
					throw new BadNamespaceException();
				case "connection-timeout":
					throw new ConnectionTimeoutException();
				case "host-gone":
					throw new HostGoneException();
				case "host-unkown":
					throw new HostUnknownException();
				case "improper-addressing":
					throw new ImproperAddressingException();
				case "internal-server-error":
					throw new InternalServerErrorException();
				case "invalid-from":
					throw new InvalidFromException();
				case "invalid-id":
					throw new InvalidIdException();
				case "invalid-namespace":
					throw new InvalidNamespaceException();
				case "invalid-xml":
					throw new InvalidXmlException();
				case "not-authorized":
					throw new NotAuthorizedException();
				case "policy-violation":
					throw new PolicyViolationException();
				case "remote-connection-failed":
					throw new RemoteConnectionFailedException();
				case "resource-constraint":
					throw new ResourceContraintException();
				case "restricted-xml":
					throw new RestrictedXmlException();
				case "see-other-host": //TODO: pass in host name
					throw new SeeOtherHostException("host not supplied");
				case "system-shutdown":
					throw new SystemShutdownException();
				case "unsupported-encoding":
					throw new UnsupportedEncodingException();
				case "unsupported-stanza-type":
					throw new UnsupportedStanzaTypeException();
				case "unsupported-version":
					throw new UnsupportedVersionException();
				case "xml-not-well-formed":
					throw new XmlNotWellFormedException();
				default:
					throw new BadFormatException();
			}
		}
	}

	/// <summary>
	/// The entity has sent XML that cannot be processed; this error MAY be used instead of the more specific exceptions 
	/// derived from this exception.
	/// </summary>
	public class BadFormatException : StreamException {
		public BadFormatException() {}

		public BadFormatException(string message) : base(message) {}
	}

	/// <summary>
	/// The entity has sent a namespace prefix that is unsupported, or has sent no namespace prefix on an element that 
	/// requires such a prefix 
	/// </summary>
	public class BadNamespaceException : BadFormatException {
		public BadNamespaceException() {}

		public BadNamespaceException(string message) : base(message) {}
	}

	/// <summary>
	/// The streams namespace name is something other than "http://etherx.jabber.org/streams" or the dialback namespace 
	/// name is something other than "jabber:server:dialback"
	/// </summary>
	public class InvalidNamespaceException : BadFormatException {
		public InvalidNamespaceException() {}

		public InvalidNamespaceException(string message) : base(message) {}
	}

	/// <summary>
	/// The entity has sent invalid XML over the stream to a server that performs validation
	/// </summary>
	public class InvalidXmlException : BadFormatException {
		public InvalidXmlException() {}

		public InvalidXmlException(string message) : base(message) {}
	}

	/// <summary>
	/// The entity has attempted to send restricted XML features such as a comment, processing instruction, DTD, entity 
	/// reference, or unescaped character
	/// </summary>
	public class RestrictedXmlException : BadFormatException {
		public RestrictedXmlException() {}

		public RestrictedXmlException(string message) : base(message) {}
	}

	/// <summary>
	/// The initiating entity has encoded the stream in an encoding that is not supported by the server
	/// </summary>
	public class UnsupportedEncodingException : BadFormatException {
		public UnsupportedEncodingException() {}

		public UnsupportedEncodingException(string message) : base(message) {}
	}

	/// <summary>
	/// The initiating entity has sent XML that is not well-formed as defined
	/// </summary>
	public class XmlNotWellFormedException : BadFormatException {
		public XmlNotWellFormedException() {}

		public XmlNotWellFormedException(string message) : base(message) {}
	}

	/// <summary>
	/// The server is closing the active stream for this entity because a new stream has been initiated that conflicts with 
	/// the existing stream.
	/// </summary>
	public class ConflictException : StreamException {
		public ConflictException() {}

		public ConflictException(string message) : base(message) {}
	}

	/// <summary>
	/// The entity has not generated any traffic over the stream for some period of time (configurable according to a local 
	/// service policy).
	/// </summary>
	public class ConnectionTimeoutException : StreamException {
		public ConnectionTimeoutException() {}

		public ConnectionTimeoutException(string message) : base(message) {}
	}

	/// <summary>
	/// The value of the 'to' attribute provided by the initiating entity in the stream header corresponds to a hostname that 
	/// is no longer hosted by the server.
	/// </summary>
	public class HostGoneException : StreamException {
		public HostGoneException() {}

		public HostGoneException(string message) : base(message) {}
	}

	/// <summary>
	/// The value of the 'to' attribute provided by the initiating entity in the stream header does not correspond to a 
	/// hostname  that is hosted by the server.
	/// </summary>
	public class HostUnknownException : StreamException {
		public HostUnknownException() {}

		public HostUnknownException(string message) : base(message) {}
	}

	/// <summary>
	/// A stanza sent between two servers lacks a 'to' or 'from' attribute (or the attribute has no value).
	/// </summary>
	public class ImproperAddressingException : StreamException {
		public ImproperAddressingException() {}

		public ImproperAddressingException(string message) : base(message) {}
	}

	/// <summary>
	/// The server has experienced a misconfiguration or an otherwise-undefined internal error that prevents it from servicing 
	/// the stream.
	/// </summary>
	public class InternalServerErrorException : StreamException {
		public InternalServerErrorException() {}

		public InternalServerErrorException(string message) : base(message) {}
	}

	/// <summary>
	/// The JID or hostname provided in a 'from' address does not match an authorized JID or validated domain negotiated
	/// between servers via SASL or dialback, or between a client and a server via authentication and resource binding.
	/// </summary>
	public class InvalidFromException : StreamException {
		public InvalidFromException() {}

		public InvalidFromException(string message) : base(message) {}
	}

	/// <summary>
	/// The stream ID or dialback ID is invalid or does not match an ID previously provided.
	/// </summary>
	public class InvalidIdException : StreamException {
		public InvalidIdException() {}

		public InvalidIdException(string message) : base(message) {}
	}

	/// <summary>
	/// The entity has attempted to send data before the stream has been authenticated, or otherwise is not authorized to 
	/// perform an action related to stream negotiation; the receiving entity MUST NOT process the offending stanza 
	/// before sending the stream error.
	/// </summary>
	public class NotAuthorizedException : StreamException {
		public NotAuthorizedException() {}

		public NotAuthorizedException(string message) : base(message) {}
	}

	/// <summary>
	/// The entity has violated some local service policy; the server MAY choose to specify the policy in the &lt;text/&gt;
	///  element or an application-specific condition element.
	/// </summary>
	public class PolicyViolationException : StreamException {
		public PolicyViolationException() {}

		public PolicyViolationException(string message) : base(message) {}
	}

	/// <summary>
	/// The server is unable to properly connect to a remote entity that is required for authentication or authorization.
	/// </summary>
	public class RemoteConnectionFailedException : StreamException {
		public RemoteConnectionFailedException() {}

		public RemoteConnectionFailedException(string message) : base(message) {}
	}

	/// <summary>
	/// The server lacks the system resources necessary to service the stream.
	/// </summary>
	public class ResourceContraintException : StreamException {
		public ResourceContraintException() {}

		public ResourceContraintException(string message) : base(message) {}
	}

	/// <summary>
	/// The server will not provide service to the initiating entity but is redirecting traffic to another host
	/// </summary>
	public class SeeOtherHostException : StreamException {
		public SeeOtherHostException(string otherHost) {
			OtherHost = otherHost;
		}

		public SeeOtherHostException(string otherHost, string message) : base(message) {
			OtherHost = otherHost;
		}

		public string OtherHost { get; private set; }
	}

	/// <summary>
	/// The server is being shut down and all active streams are being closed.
	/// </summary>
	public class SystemShutdownException : StreamException {
		public SystemShutdownException() {}

		public SystemShutdownException(string message) : base(message) {}
	}

	/// <summary>
	/// The error condition is not one of those defined by the other conditions in this list; this error condition SHOULD be 
	/// used only in conjunction with an application-specific condition.
	/// </summary>
	public class UndefinedException : StreamException {
		public UndefinedException() {}

		public UndefinedException(string message) : base(message) {}
	}

	/// <summary>
	/// The initiating entity has sent a first-level child of the stream that is not supported by the server.
	/// </summary>
	public class UnsupportedStanzaTypeException : StreamException {
		public UnsupportedStanzaTypeException() {}

		public UnsupportedStanzaTypeException(string message) : base(message) {}
	}

	/// <summary>
	/// The value of the 'version' attribute provided by the initiating entity in the stream header specifies a version of XMPP 
	/// that is not supported by the server; the server MAY specify the version(s) it supports in the &gt;text/&lt; element.
	/// </summary>
	public class UnsupportedVersionException : StreamException {
		public UnsupportedVersionException() {}

		public UnsupportedVersionException(string message) : base(message) {}
	}
}