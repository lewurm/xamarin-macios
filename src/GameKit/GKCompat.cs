// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
using Foundation;
using ObjCRuntime;

namespace GameKit {

#if !XAMCORE_3_0
	public partial class GKMatchRequest {
		
		[iOS (8,0), Mac (10,10)]
		[Obsolete ("Use 'RecipientResponseHandler' property.")]
		public virtual void SetRecipientResponseHandler (Action<GKPlayer, GKInviteRecipientResponse> handler)
		{
			RecipientResponseHandler = handler;
		}
	}

	public partial class GKMatchmaker {
		
		[Obsolete ("Use 'InviteHandler' property.")]
		public virtual void SetInviteHandler (GKInviteHandler handler)
		{
			InviteHandler = handler;
		}
	}
#endif // !XAMCORE_3_0

#if WATCH && !XAMCORE_4_0
	[Unavailable (PlatformName.WatchOS)]
	public static class GKGameSessionErrorCodeExtensions {
		[Obsolete ("Always returns null.")]
		public static NSString GetDomain (this GKGameSessionErrorCode self) => null;
	}
#endif

#if !XAMCORE_4_0 && !WATCH
	public partial class GKGameSession {

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidAddPlayer (GKGameSession session, GKCloudPlayer player) {}

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidChangeConnectionState (GKGameSession session, GKCloudPlayer player, GKConnectionState newState) {}

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidReceiveData (GKGameSession session, Foundation.NSData data, GKCloudPlayer player) {}

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidReceiveMessage (GKGameSession session, string message, Foundation.NSData data, GKCloudPlayer player) {}

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidRemovePlayer (GKGameSession session, GKCloudPlayer player) {}

		[Obsolete ("Empty stub (GKGameSessionEventListenerPrivate category members are not public API).")]
		public static void DidSaveData (GKGameSession session, GKCloudPlayer player, Foundation.NSData data) {}
	}
#endif
}
