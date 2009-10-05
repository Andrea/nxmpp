namespace NXmpp.Dns.Windows {
	public class WmiDnsServerLookupFactory : IDnsServerLookupFactory {
		#region IDnsServerLookupFactory Members

		public IDnsServerLookup Create() {
			return new WmiDnsServerLookup();
		}

		#endregion
	}
}