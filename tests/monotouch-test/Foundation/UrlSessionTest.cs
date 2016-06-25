//
// Unit tests for NSUrlSession
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;

#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UrlSessionTest {
		[Test]
		public void CreateDataTaskAsync ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Inconclusive ("requires iOS7");
			
			NSUrlSession session = NSUrlSession.SharedSession;

			var url = new NSUrl ("http://www.xamarin.com");
			var tmpfile = Path.GetTempFileName ();
			File.WriteAllText (tmpfile, "TMPFILE");
			var file_url = NSUrl.FromFilename (tmpfile);
			var file_data = NSData.FromFile (tmpfile);
			var request = new NSUrlRequest (url);

			var completed = false;
			var timeout = 30;

			/* CreateDataTask */
			completed = false;
			Assert.IsTrue (TestRuntime.RunAsync (DateTime.Now.AddSeconds (timeout), async () => {
				await session.CreateDataTaskAsync (request);
				completed = true;
			}, () => completed), "CreateDataTask a");

			completed = false;
			Assert.IsTrue (TestRuntime.RunAsync (DateTime.Now.AddSeconds (timeout), async () => {
				await session.CreateDataTaskAsync (url);
				completed = true;
			}, () => completed), "CreateDataTask b");

			/* CreateDownloadTask */
			completed = false;
			Assert.IsTrue (TestRuntime.RunAsync (DateTime.Now.AddSeconds (timeout), async () => {
				await session.CreateDownloadTaskAsync (request);
				completed = true;
			}, () => completed), "CreateDownloadTask a");


			completed = false;
			Assert.IsTrue (TestRuntime.RunAsync (DateTime.Now.AddSeconds (timeout), async () => {
				await session.CreateDownloadTaskAsync (url);
				completed = true;
			}, () => completed), "CreateDownloadTask b");

			/* CreateUploadTask */
			completed = false;
			Assert.IsTrue (TestRuntime.RunAsync (DateTime.Now.AddSeconds (timeout), async () => {
				try {
					await session.CreateUploadTaskAsync (request, file_url);
				} catch /* (Exception ex) */ {
//					Console.WriteLine ("Ex: {0}", ex);
				} finally {
					completed = true;
				}
			}, () => completed), "CreateUploadTask a");

			completed = false;
			Assert.IsTrue (TestRuntime.RunAsync (DateTime.Now.AddSeconds (timeout), async () => {
				try {
					await session.CreateUploadTaskAsync (request, file_data);
				} catch /* (Exception ex) */ {
//					Console.WriteLine ("Ex: {0}", ex);
				} finally {
					completed = true;
				}
			}, () => completed), "CreateUploadTask b");
		}

		[Test]
		public void DownloadDataAsync ()
		{
			if (!TestRuntime.CheckiOSSystemVersion (7, 0))
				Assert.Inconclusive ("NSUrlSession is iOS7+");
			
			bool completed = false;
			int failed_iteration = -1;

			TestRuntime.RunAsync (DateTime.Now.AddSeconds (30), async () => {
				for (int i = 0; i < 5; i++) {
					// Use the default configuration so we can make use of the shared cookie storage.
					var session = NSUrlSession.FromConfiguration (NSUrlSessionConfiguration.DefaultSessionConfiguration);

					var downloadUri = new Uri ("https://google.com");
					var downloadResponse = await session.CreateDownloadTaskAsync (downloadUri);

					var tempLocation = downloadResponse.Location;
					if (!File.Exists (tempLocation.Path)) {
						Console.WriteLine ("#{1} {0} does not exists", tempLocation, i);
						failed_iteration = i;
						break;
					}
				}
				completed = true;
			}, () => completed);

			Assert.AreEqual (-1, failed_iteration, "Failed");
		}

		[Test]
		public void SharedSession ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Inconclusive ("requires iOS7");
			
			// in iOS9 those selectors do not respond - but they do work (forwarded to __NSURLSessionLocal type ?)
			// * delegateQueue, sessionDescription, setSessionDescription:, delegate
			var session = NSUrlSession.SharedSession;
			Assert.Null (session.Delegate, "delegate");
			Assert.NotNull (session.DelegateQueue, "delegateQueue");
			Assert.Null (session.SessionDescription, "sessionDescription");
			session.SessionDescription = "descriptive label";
			Assert.That ((string)session.SessionDescription, Is.EqualTo ("descriptive label"), "setSessionDescription:");
			session.SessionDescription = null; // the session instance is global, so revert value to to make sure the test can be re-run successfully.
		}
	}
}