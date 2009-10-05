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

using System.IO;
using System.Linq;
using NXmpp.Sasl;
using Xunit;

namespace NXmpp.Tests.Sasl {
	public class DigestMD5ChallengeTests {
		// realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";

		#region Nested type: DigestChallengeAlgorithmTests
		public class DigestChallengeAlgorithmTests {
			[Fact]
			public void When_algorithm_ommitted_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Challenge(challange));
			}

			[Fact]
			public void When_algorithm_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Equal("md5-sess", digestChallenge.Algorithm);
			}

			[Fact]
			public void When_two_algorithm_supplied_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeCharsetTests

		public class DigestChallengeCharsetTests {
			[Fact]
			public void When_charset_ommitted_should_default() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-int\",algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Equal("iso-8859-1", digestChallenge.CharacterSet.WebName);
			}

			[Fact]
			public void When_charset_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-int\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Equal("utf-8", digestChallenge.CharacterSet.WebName);
			}

			[Fact]
			public void When_two_charset_supplied_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-int\",charset=utf-8,charset=utf-8,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeCipherOptsTests
		public class DigestChallengeCipherOptsTests {
			[Fact]
			public void When_cipheropts_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess,cipher=\"3des\"";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.NotNull(digestChallenge.CipherOptions);
				Assert.Equal(1, digestChallenge.CipherOptions.Count());
				Assert.Contains("3des", digestChallenge.CipherOptions.ToList());
			}

			[Fact]
			public void When_cipheroptsommitted_should_contian_none() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Null(digestChallenge.CipherOptions);
			}

			[Fact]
			public void When_unrecognized_cipher_supplied_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess,cipher=\"3desXX\"";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeMaxBufTests

		public class DigestChallengeMaxBufTests {
			[Fact]
			public void When_maxbuf_ommitted_should_default_to_65536() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-int\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Equal(65536, digestChallenge.MaximumBufferSize);
			}

			[Fact]
			public void When_maxbuf_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-int\",maxbuf=10000,charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Equal(10000, digestChallenge.MaximumBufferSize);
			}

			[Fact]
			public void When_two_maxbuf_supplied_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-int\",maxbuf=10000,maxbuf=10001,charset=utf-8,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeNonceTests

		public class DigestChallengeNonceTests {
			[Fact]
			public void When_nonce_omitted_ReadFromString_should_throw() {
				const string challange = "realm=\"somerealm\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Challenge(challange));
			}

			[Fact]
			public void When_one_nonce_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Equal("somenonce", digestChallenge.Nonce);
			}

			[Fact]
			public void When_two_nonces_supplied_ReadFromString_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeQopOptionsTests

		public class DigestChallengeQopOptionsTests {
			[Fact]
			public void When_no_qopoption_supplied_should_default_to_Auth() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Contains("auth", digestChallenge.QopOptions.ToList());
			}

			[Fact]
			public void When_qop_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-int\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Contains("auth-int", digestChallenge.QopOptions.ToList());
			}

			[Fact]
			public void When_two_qop_supplied_should_contain_both() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-int\",qop=\"auth-conf\",charset=utf-8,algorithm=md5-sess,cipher=\"3des\"";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Contains("auth-int", digestChallenge.QopOptions.ToList());
				Assert.Contains("auth-conf", digestChallenge.QopOptions.ToList());
			}

			[Fact]
			public void When_unrecognised_qop_supplied_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-intX\",charset=utf-8,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeRealmTests

		public class DigestChallengeRealmTests {
			[Fact]
			public void When_a_realm_is_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Contains("somerealm", digestChallenge.Realms.ToList());
			}

			[Fact]
			public void When_a_realm_is_supplied_twice_should_contain_value_once() {
				const string challange = "realm=\"somerealm\",realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Equal(1, digestChallenge.Realms.Count());
			}

			[Fact]
			public void When_realm_is_omitted_ReadFromString_should_not_throw() {
				const string challange = "nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				Assert.DoesNotThrow(() => new DigestMD5Challenge(challange));
			}

			[Fact]
			public void When_two_realm_are_supplied_should_contain_both() {
				const string challange = "realm=\"somerealm1\",realm=\"somerealm2\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Challenge(challange);
				Assert.Equal(2, digestChallenge.Realms.Count());
				Assert.Contains("somerealm1", digestChallenge.Realms.ToList());
				Assert.Contains("somerealm2", digestChallenge.Realms.ToList());
			}
		}

		#endregion

		#region Nested type: DigestChallengeStaleTests

		public class DigestChallengeStaleTests {
			[Fact]
			public void When_stale_supplied_and_initial_authentication_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-int\",charset=utf-8,algorithm=md5-sess,stale=\"true\"";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Challenge(challange));
			}

			[Fact]
			public void When_stale_supplied_and_sunsequent_authentication_should_not_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-int\",charset=utf-8,algorithm=md5-sess,stale=\"true\"";
				Assert.DoesNotThrow(() => new DigestMD5Challenge(challange));
			}
		}

		#endregion
	}
}