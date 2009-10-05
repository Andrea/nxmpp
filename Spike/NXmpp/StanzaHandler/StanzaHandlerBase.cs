using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using NXmpp.Protocol;
using NXmpp.Protocol.Stanzas;

namespace NXmpp.StanzaHandler {
	internal abstract class StanzaHandlerBase : IStanzaHandler {

		private readonly Dictionary<string, IQRequest> _iqRequests = new Dictionary<string, IQRequest>();

		private readonly IXmppService _xmppService;

		internal StanzaHandlerBase(IXmppService xmppService) {
			_xmppService = xmppService;
		}

		private int id;

		#region IStanzaHandler Members

		public abstract string HandlerKey { get; }

		public abstract void Initialize();

		public abstract string Namespace { get; }

		protected JId JId { get { return _xmppService.JId; } }

		protected void SendIQ(IQRequest iqRequest) {
			var iq = new XElement("iq", new XAttribute("from", _xmppService.JId));
			if (iqRequest.To != null) {
				iq.Add(new XAttribute("to", iqRequest.To));
			}
			string stanzaId = NextId();
			iq.Add(new XAttribute("id", stanzaId));
			iq.Add(new XAttribute("type", iqRequest.RequestType.ToString().ToLower()));
			var innerElement = new XElement(XName.Get(iqRequest.InnerElementLocalName, Namespace));
			iq.Add(innerElement);
			if (iqRequest.Content != null && iqRequest.Content.Length > 0) {
				innerElement.Add(iqRequest.Content);
			}
			_iqRequests.Add(stanzaId, iqRequest);
			_xmppService.Send(this, iq);
		}

		protected void SendPresence(PresenceAvailabilityStatus? availabilityStatus, string detailedStatus) {
			var element = new XElement("presence");
			if (availabilityStatus.HasValue) {
				element.Add(new XAttribute("type", availabilityStatus.Value.ToString().ToLower()));
			}
			_xmppService.Send(this, element);
		}

		public void HandleIQStanza(XElement iqStanza) {
			if (iqStanza.Attribute("type").Value == "result" || iqStanza.Attribute("type").Value == "error") { //it's a response
				var responseId = iqStanza.Attribute("id").Value;
				if(!_iqRequests.ContainsKey(responseId)) {
					throw new InvalidOperationException("IQ response has no corresponding requests");
				}
				var iqRequest = _iqRequests[responseId];
				JId from = iqStanza.Attribute("from") != null ? JId.Parse(iqStanza.Attribute("from").Value) : null;
				JId to = iqStanza.Attribute("to") != null ? JId.Parse(iqStanza.Attribute("to").Value) : null;
				IQResponse iqResponse;
				if(iqStanza.Attribute("type").Value == "result") {
					var content = iqStanza.Elements().First().Elements().ToArray();
					iqResponse = new IQResponse(from, to, iqStanza.Descendants().First().Name.LocalName, content);
				}
				else {
					var error = Error.CreateFromXml(iqStanza.Element("error"));
					iqResponse = new IQResponse(from, to, error);
				}
				iqRequest.ResponseCallback(iqResponse);
			}
			else HandleIQStanzaPush(iqStanza);
		}

		protected abstract void HandleIQStanzaPush(XElement iqStanza);



		#endregion

		private string NextId() {
			return HandlerKey + "_" + id++;
		}

		protected delegate void IQResponseCallback(JId from, JId to, string id);

		protected abstract class IQ {
			internal JId To { get; private set; }
			
			internal string InnerElementLocalName { get; private set; }
			internal XElement[] Content { get; private set; }

			protected IQ(JId to, string innerElementLocalName, XElement[] content) {
				To = to;
				InnerElementLocalName = innerElementLocalName;
				Content = content;
			}
		}

		protected class IQRequest : IQ {
			internal IQRequest(string innerElementLocalName, IQRequestType requestType, Action<IQResponse> responseCallback) : this(null, innerElementLocalName, null, requestType, responseCallback) { }

			internal IQRequest(string innerElementLocalName, XElement[] content, IQRequestType requestType, Action<IQResponse> responseCallback) : this(null, innerElementLocalName, content, requestType, responseCallback) { }

			internal IQRequest(JId to, string innerElementLocalName, XElement[] content, IQRequestType requestType, Action<IQResponse> responseCallback) : base(to, innerElementLocalName, content) {
				RequestType = requestType;
				ResponseCallback = responseCallback;
			}

			internal IQRequestType RequestType { get; private set;  }
			internal Action<IQResponse> ResponseCallback { get; private set;}
		}

		protected class IQResponse : IQ {

			internal JId From { get; private set; }

			internal IQResponse(JId from, JId to, string innerElementLocalName, XElement[] content) : this(from, to, innerElementLocalName, content, IQResponseType.Result, null) { }

			internal IQResponse(JId from, JId to, Error error) : this(from, to, null, null, IQResponseType.Error, error) { }

			private IQResponse(JId from, JId to, string innerElementLocalName, XElement[] content, IQResponseType requestType, Error error) : base(to, innerElementLocalName, content) {
				From = from;
				ResponseType = requestType;
				Error = error;
			}

			internal IQResponseType ResponseType { get; private set;  }
			internal Error Error { get; private set; }
		}

		protected enum IQRequestType {
			Get,
			Set
		}

		protected enum IQResponseType {
			Result,
			Error
		}
	}
}