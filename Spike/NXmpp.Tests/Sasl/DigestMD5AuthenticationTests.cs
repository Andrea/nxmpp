#region Licence

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

using System.Linq;
using NXmpp.Sasl;
using Xunit;

namespace NXmpp.Tests.Sasl {
	public class DigestMD5AuthenticationTests {
		[Fact]
		public void TestUsingRFCExampleData() {
			//using values from RFC 2831
			const string challenge = "realm=\"elwood.innosoft.com\",nonce=\"OA6MG9tEQGm2hh\",qop=\"auth\",algorithm=md5-sess,charset=utf-8";
			var digestAuthentication = new DigestMD5Authentication(challenge);
			string digestResponse = digestAuthentication.GetDigestResponse("chris", "secret", "elwood.innosoft.com", "elwood.innosoft.com", "OA6MHXh6VqTrRk", "imap");

			//extract the response value from the digestResponse string
			string response = digestResponse.Split(',').Where(x => x.StartsWith("response")).First().Split('=')[1];

			Assert.Equal("d388dad90d4bbd760a152321f2143af7", response);
		}
	}
}