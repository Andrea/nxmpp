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
using System.Security.Cryptography;
using System.Text;

namespace NXmpp.Sasl {
	public class DigestMD5Authentication {
		private readonly DigestMD5Challenge digestChallenge;
		private readonly Random random = new Random();

		public DigestMD5Authentication(string challenge) {
			digestChallenge = new DigestMD5Challenge(challenge);
		}

		public string[] Realms {
			get { return digestChallenge.Realms; }
		}

		public string GetDigestResponse(string username, string password, string realm, string hostName) {
			return GetDigestResponse(username, password, realm, hostName, GenerateNonce(), "xmpp");
		}

		//used for testing algorithm (need to be able specifiy nonce)
		internal string GetDigestResponse(string username, string password, string realm, string hostName, string cnonce, string srvType) {
			var responseString = new StringBuilder();
			responseString.Append("username=\"" + username + "\"");
			responseString.Append(",realm=\"" + realm + "\"");
			responseString.Append(",nonce=\"" + digestChallenge.Nonce + "\"");
			responseString.Append(",cnonce=\"" + cnonce + "\"");
			responseString.Append(",nc=00000001");
			responseString.Append(",qop=" + DigestMD5Challenge.SupportedQopType);
			string digestUri = srvType + "/" + hostName;
			responseString.Append(",digest-uri=\"" + digestUri + "\"");
			responseString.Append(",charset=" + digestChallenge.CharacterSet.WebName);

			//rfc2831 section 2.1.2.1 Response Value
			var cryptHandler = new MD5CryptoServiceProvider();
			string a1Hash =
				MD5HashAsHexString(cryptHandler,
				                   MD5Hash(cryptHandler, username + ":" + realm + ":" + password)
				                   	.Concat(Encoding.ASCII.GetBytes(":" + digestChallenge.Nonce + ":" + cnonce))
				                   	.ToArray()
					);
			string a2Hash = MD5HashAsHexString(cryptHandler, "AUTHENTICATE:" + digestUri);
			string response = a1Hash + ":" + digestChallenge.Nonce + ":00000001:" + cnonce + ":auth:" + a2Hash;
			string responseHash = MD5HashAsHexString(cryptHandler, response);

			responseString.Append(",response=" + responseHash);
			return responseString.ToString();
		}

		private string GenerateNonce() {
			return random.Next(1234567, 9999999).ToString();
		}

		private static string MD5HashAsHexString(HashAlgorithm cryptHandler, string s) {
			return MD5HashAsHexString(cryptHandler, Encoding.ASCII.GetBytes(s));
		}

		// Hash an input and return the hash as a 32 character hexadecimal lowercase string.
		private static string MD5HashAsHexString(HashAlgorithm cryptHandler, byte[] data) {
			data = cryptHandler.ComputeHash(data);
			return BitConverter.ToString(data).Replace("-", "").ToLower();
		}

		private static byte[] MD5Hash(HashAlgorithm cryptHandler, string s) {
			return cryptHandler.ComputeHash(Encoding.ASCII.GetBytes(s));
		}
	}
}