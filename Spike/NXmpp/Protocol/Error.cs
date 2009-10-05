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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NXmpp.Extensions;

namespace NXmpp.Protocol {
	internal enum ErrorType {
		Cancel,
		Continue,
		Modify,
		Auth,
		Wait
	}

	internal enum ErrorCondition {
		BadRequest,
		Conflict,
		FeatureNotImplemented,
		Forbidden,
		Gone,
		InternalServerError,
		ItemNotFound,
		JidMalformed,
		NotAcceptable,
		NotAllowed,
		NotAuthorized,
		PaymentRequired,
		RecipientUnavailable,
		Redirect,
		RegistrationRequired,
		RemoteServerNotFound,
		RemoteServerTimeout,
		ResourceConstraint,
		ServiceUnavailable,
		SubscriptionRequired,
		UndefinedCondition,
		UnexpectedRequest
	}

	/// <summary>
	/// Stanza error (recoverable).
	/// </summary>
	internal class Error {

		/*
		 	<stanza-kind to='sender' type='error'>}
				[RECOMMENDED to include sender XML here]
				{<error type='error-type'>
					<defined-condition xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/>
					<text xmlns='urn:ietf:params:xml:ns:xmpp-stanzas' xml:lang='langcode'>
						OPTIONAL descriptive text
					</text>
				[OPTIONAL application-specific condition element]
				</error>}
			</stanza-kind>
		 */

		private Error(){}

		internal ErrorType Type { get; private set; }

		internal ErrorCondition Condition { get; private set; }

		internal string Text { get; private set; }

		internal static Error CreateFromXml(XElement element) {
			var error = new Error();
			error.Type = EnumHelper.Parse<ErrorType>(element.Attribute("type").Value, true);  //parsing by convention here.
			string errorCondition = element.Descendants().First().Name.LocalName.Replace("-", "");
			error.Condition = EnumHelper.Parse<ErrorCondition>(errorCondition, true);

			XElement textElement = element.Descendants(XName.Get("text", NamespaceStrings.XmppStanzas)).FirstOrDefault();
			if(textElement != null) {
				error.Text = textElement.Value;
			}
			return error;
		}
	}
}