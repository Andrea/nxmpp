using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NXmpp.Net {
	public class AdapterInfo {
		const int MAX_ADAPTER_DESCRIPTION_LENGTH = 128;
		const int ERROR_BUFFER_OVERFLOW = 111;
		const int MAX_ADAPTER_NAME_LENGTH = 256;
		const int MAX_ADAPTER_ADDRESS_LENGTH = 8;
		const int MIB_IF_TYPE_OTHER = 1;
		const int MIB_IF_TYPE_ETHERNET = 6;
		const int MIB_IF_TYPE_TOKENRING = 9;
		const int MIB_IF_TYPE_FDDI = 15;
		const int MIB_IF_TYPE_PPP = 23;
		const int MIB_IF_TYPE_LOOPBACK = 24;
		const int MIB_IF_TYPE_SLIP = 28;

		[DllImport("iphlpapi.dll", CharSet = CharSet.Ansi)]
		public static extern int GetAdaptersInfo(IntPtr pAdapterInfo, ref Int64 pBufOutLen);

		public static IP_ADAPTER_INFO[] GetAdapters() {
			long structSize = Marshal.SizeOf(typeof(IP_ADAPTER_INFO));
			IntPtr pArray = Marshal.AllocHGlobal(new IntPtr(structSize));

			int ret = GetAdaptersInfo(pArray, ref structSize);

			if (ret == ERROR_BUFFER_OVERFLOW) // ERROR_BUFFER_OVERFLOW == 111
            {
				// Buffer was too small, reallocate the correct size for the buffer.
				pArray = Marshal.ReAllocHGlobal(pArray, new IntPtr(structSize));

				ret = GetAdaptersInfo(pArray, ref structSize);
			} // if

			if (ret == 0) {
				// Call Succeeded
				IntPtr pEntry = pArray;
				var adapterInfos = new List<IP_ADAPTER_INFO>();
				do {
					// Retrieve the adapter info from the memory address
					var entry = (IP_ADAPTER_INFO)Marshal.PtrToStructure(pEntry, typeof(IP_ADAPTER_INFO));

					//// ***Do something with the data HERE!***
					//Console.WriteLine("\n");
					//Console.WriteLine("Index: {0}", entry.Index.ToString());

					//// Adapter Type
					//string tmpString;
					//switch (entry.Type) {
					//    case MIB_IF_TYPE_ETHERNET:
					//        tmpString = "Ethernet";
					//        break;
					//    case MIB_IF_TYPE_TOKENRING:
					//        tmpString = "Token Ring";
					//        break;
					//    case MIB_IF_TYPE_FDDI:
					//        tmpString = "FDDI";
					//        break;
					//    case MIB_IF_TYPE_PPP:
					//        tmpString = "PPP";
					//        break;
					//    case MIB_IF_TYPE_LOOPBACK:
					//        tmpString = "Loopback";
					//        break;
					//    case MIB_IF_TYPE_SLIP:
					//        tmpString = "Slip";
					//        break;
					//    default:
					//        tmpString = "Other/Unknown";
					//        break;
					//} // switch
					//Console.WriteLine("Adapter Type: {0}", tmpString);

					//Console.WriteLine("Name: {0}", entry.AdapterName);
					//Console.WriteLine("Desc: {0}\n", entry.AdapterDescription);

					//Console.WriteLine("DHCP Enabled: {0}", (entry.DhcpEnabled == 1) ? "Yes" : "No");

					//if (entry.DhcpEnabled == 1) {
					//    Console.WriteLine("DHCP Server : {0}", entry.DhcpServer.IpAddress.Address);

					//    // Lease Obtained (convert from "time_t" to C# DateTime)
					//    DateTime pdatDate = new DateTime(1970, 1, 1).AddSeconds(entry.LeaseObtained).ToLocalTime();
					//    Console.WriteLine("Lease Obtained: {0}", pdatDate.ToString());

					//    // Lease Expires (convert from "time_t" to C# DateTime)
					//    pdatDate = new DateTime(1970, 1, 1).AddSeconds(entry.LeaseExpires).ToLocalTime();
					//    Console.WriteLine("Lease Expires : {0}\n", pdatDate.ToString());
					//} // if DhcpEnabled

					//Console.WriteLine("IP Address     : {0}", entry.IpAddressList.IpAddress.Address);
					//Console.WriteLine("Subnet Mask    : {0}", entry.IpAddressList.IpMask.Address);
					//Console.WriteLine("Default Gateway: {0}", entry.GatewayList.IpAddress.Address);

					//// MAC Address (data is in a byte[])
					//tmpString = string.Empty;
					//for (int i = 0; i < entry.AddressLength - 1; i++) {
					//    tmpString += string.Format("{0:X2}-", entry.Address[i]);
					//}
					//Console.WriteLine("MAC Address    : {0}{1:X2}\n", tmpString, entry.Address[entry.AddressLength - 1]);

					//Console.WriteLine("Has WINS: {0}", entry.HaveWins ? "Yes" : "No");
					//if (entry.HaveWins) {
					//    Console.WriteLine("Primary WINS Server  : {0}", entry.PrimaryWinsServer.IpAddress.Address);
					//    Console.WriteLine("Secondary WINS Server: {0}", entry.SecondaryWinsServer.IpAddress.Address);
					//} // HaveWins

					// Get next adapter (if any)
					pEntry = entry.Next;

				} while (pEntry != IntPtr.Zero);

				Marshal.FreeHGlobal(pArray);
				return adapterInfos.ToArray();

			} // if
			Marshal.FreeHGlobal(pArray);
			throw new InvalidOperationException("GetAdaptersInfo failed: " + ret);
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct IP_ADAPTER_INFO {
			public IntPtr Next;
			public Int32 ComboIndex;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ADAPTER_NAME_LENGTH + 4)]
			public string AdapterName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ADAPTER_DESCRIPTION_LENGTH + 4)]
			public string AdapterDescription;
			public UInt32 AddressLength;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ADAPTER_ADDRESS_LENGTH)]
			public byte[] Address;
			public Int32 Index;
			public UInt32 Type;
			public UInt32 DhcpEnabled;
			public IntPtr CurrentIpAddress;
			public IP_ADDR_STRING IpAddressList;
			public IP_ADDR_STRING GatewayList;
			public IP_ADDR_STRING DhcpServer;
			public bool HaveWins;
			public IP_ADDR_STRING PrimaryWinsServer;
			public IP_ADDR_STRING SecondaryWinsServer;
			public Int32 LeaseObtained;
			public Int32 LeaseExpires;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct IP_ADDR_STRING {
			public IntPtr Next;
			public IP_ADDRESS_STRING IpAddress;
			public IP_ADDRESS_STRING IpMask;
			public Int32 Context;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct IP_ADDRESS_STRING {
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
			public string Address;
		}
	}

}
