//
// Compatibility Helpers
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc.
//
#if XAMCORE_2_0 || !MONOMAC // MultipeerConnectivity is 64-bit only on OS X
using System;

namespace XamCore.MultipeerConnectivity {

#if !XAMCORE_3_0
	public partial class MCPeerID {

		[Obsolete ("This constructor does not create a valid instance")]
		public MCPeerID ()
		{
		}
	}

	public partial class MCAdvertiserAssistant {

		[Obsolete ("This constructor does not create a valid instance")]
		public MCAdvertiserAssistant ()
		{
		}
	}
#endif
}
#endif
