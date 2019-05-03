//
// Unit tests for NSRegularExpression
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2019 Microsoft Corp. All rights reserved.
//

using System;
using System.IO;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RegularExpressionTest {
		[Test]
		public void GetMatches ()
		{
			var text = "some text https://microsoft.com text text";

			var range = new NSRange (0, text.Length);
			var detector = NSDataDetector.Create (NSTextCheckingType.Link, out NSError error);

#if XAMCORE_4_0
			var matches = detector.GetMatches (new NSString (text), 0, range);
#else
			var matches = detector.GetMatches2 (new NSString (text), 0, range);
#endif

			Assert.AreEqual (10, matches [0].Range.Location, "Range.Location");
			Assert.AreEqual (21, matches [0].Range.Length, "Range.Length");
			Assert.AreEqual ("https://microsoft.com", matches [0].Url.AbsoluteString, "Url");
		}
	}
}
