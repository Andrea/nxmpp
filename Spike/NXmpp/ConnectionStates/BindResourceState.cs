using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NXmpp.Protocol;
using NXmpp.Protocol.Stream;

namespace NXmpp.ConnectionStates {
	internal class BindResourceState : ConnectionStateBase {
		private readonly ServerFeatures _serverFeatures;

		internal BindResourceState(IXmppContext context, ServerFeatures serverFeatures)
			: base(context) {
			_serverFeatures = serverFeatures;
		}

		internal override void Handle() {
			if (!_serverFeatures.Bind) {
				throw new InvalidOperationException("Bind expected");
			}
			Context.OnConnectionStateChanged(ConnectionStateType.BindingResource);
			var bindIqRequest = CreateBindIQRequest(Context.Settings.UserJId.Resource);
			Context.Connection.Send(bindIqRequest);
			XElement element = Context.Connection.Receive();
			//TODO: handle errors
			Context.JId = JId.Parse(element.Descendants(XName.Get("jid", NamespaceStrings.XmppBind)).First().Value);
			NextState = new StartAsyncState(Context);
		}

		private static XElement CreateBindIQRequest(string resource) {
			var element = new XElement("iq",
				new XAttribute("id", "bind_1"),
				new XAttribute("type", "set"),
				new XElement(XName.Get("bind", NamespaceStrings.XmppBind)));
			if (!string.IsNullOrEmpty(resource)) {
				element.Add(new XElement("resource", new XText(resource)));
			}
			return element;
		}
	}
}
