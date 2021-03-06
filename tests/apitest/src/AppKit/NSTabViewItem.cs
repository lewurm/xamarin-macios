﻿using System;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
#else
using AppKit;
using ObjCRuntime;
using Foundation;
#endif

namespace Xamarin.Mac.Tests
{
	public class NSTabViewItemTests
	{
		NSTabViewItem item;

		[SetUp]
		public void SetUp ()
		{
			item = new NSTabViewItem ();
		}

		[Test]
		public void NSTabViewItemShouldChangeImage ()
		{
			Asserts.EnsureYosemite ();

			var image = item.Image;
			item.Image = new NSImage ();

			Assert.IsFalse (item.Image == image, "NSTabViewItemShouldChangeImage - Failed to set the Image property");
		}

		[Test]
		public void NSTabViewItemShouldChangeViewController ()
		{
			Asserts.EnsureYosemite ();

			var vc = item.ViewController;
			item.ViewController = new NSViewController ();

			Assert.IsFalse (item.ViewController == vc, "NSTabViewItemShouldChangeViewController - Failed to set the ViewController property");
		}
	}
}

