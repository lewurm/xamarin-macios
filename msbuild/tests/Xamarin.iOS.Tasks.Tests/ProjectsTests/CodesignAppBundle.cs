﻿using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;

using NUnit.Framework;

using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("Debug")]
	[TestFixture ("Release")]
	public class CodesignAppBundle : ProjectTest
	{
		readonly string config;

		public CodesignAppBundle (string configuration) : base ("iPhone")
		{
			config = configuration;
		}

		static bool IsCodesigned (string path)
		{
			var psi = new ProcessStartInfo ("/usr/bin/codesign");
			var args = new ProcessArgumentBuilder ();

			args.Add ("--verify");
			args.AddQuoted (path);

			psi.Arguments = args.ToString ();

			var process = Process.Start (psi);
			process.WaitForExit ();

			return process.ExitCode == 0;
		}

		void AssertProperlyCodesigned ()
		{
			foreach (var dylib in Directory.EnumerateFiles (AppBundlePath, "*.dylib", SearchOption.AllDirectories))
				Assert.IsTrue (IsCodesigned (dylib), "{0} is not properly codesigned.", dylib);

			foreach (var appex in Directory.EnumerateDirectories (AppBundlePath, "*.appex", SearchOption.AllDirectories))
				Assert.IsTrue (IsCodesigned (appex), "{0} is not properly codesigned.", appex);

			var watchDir = Path.Combine (AppBundlePath, "Watch");
			if (Directory.Exists (watchDir)) {
				foreach (var watchApp in Directory.EnumerateDirectories (watchDir, "*.app", SearchOption.TopDirectoryOnly))
					Assert.IsTrue (IsCodesigned (watchApp), "{0} is not properly codesigned.", watchApp);
			}
		}

		[Test]
		public void RebuildNoChanges ()
		{
			BuildProject ("MyTabbedApplication", Platform, config, clean: true);

			AssertProperlyCodesigned ();

			var dsymDir = Path.GetFullPath (Path.Combine (AppBundlePath, "..", "MyTabbedApplication.app.dSYM"));
			var appexDsymDir = Path.GetFullPath (Path.Combine (AppBundlePath, "..", "MyActionExtension.appex.dSYM"));

			var timestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.TopDirectoryOnly).ToDictionary (file => file, file => GetLastModified (file));
			var dsymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var appexDsymTimestamps = Directory.EnumerateFiles (appexDsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			Thread.Sleep (1000);

			// Rebuild w/ no changes
			BuildProject ("MyTabbedApplication", Platform, config, clean: false);

			AssertProperlyCodesigned ();

			var newTimestamps = Directory.EnumerateFiles (AppBundlePath, "*.*", SearchOption.TopDirectoryOnly).ToDictionary (file => file, file => GetLastModified (file));
			var newDsymTimestamps = Directory.EnumerateFiles (dsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));
			var newAppexDsymTimestamps = Directory.EnumerateFiles (appexDsymDir, "*.*", SearchOption.AllDirectories).ToDictionary (file => file, file => GetLastModified (file));

			foreach (var file in timestamps.Keys) {
				// The executable files will all be newer because they get touched during each Build, all other files should not change
				if (Path.GetFileName (file) == "MyTabbedApplication" || Path.GetExtension (file) == ".dylib")
					continue;

				Assert.AreEqual (timestamps[file], newTimestamps[file], "App Bundle timestamp changed: " + file);
			}

			foreach (var file in dsymTimestamps.Keys) {
				// The Info.plist should be newer because it gets touched
				if (Path.GetFileName (file) == "Info.plist") {
					Assert.IsTrue (dsymTimestamps[file] < newDsymTimestamps[file], "App Bundle dSYMs Info.plist not touched: " + file);
				} else {
					Assert.AreEqual (dsymTimestamps[file], newDsymTimestamps[file], "App Bundle dSYMs changed: " + file);
				}
			}

			// The appex dSYMs will all be newer because they currently get regenerated after each Build due to the fact that the entire
			// *.appex gets cloned into the app bundle each time.
			//
			// Note: we could fix this by not using `ditto` and instead implementing this ourselves to only overwrite files if they've changed
			// and then setting some [Output] params that specify whether or not we need to re-codesign and/or strip debug symbols.
			foreach (var file in appexDsymTimestamps.Keys)
				Assert.IsTrue (appexDsymTimestamps[file] < newAppexDsymTimestamps[file], "App Extension dSYMs should be newer: " + file);
		}

		[Test]
		public void CodesignAfterModifyingAppExtensionTest ()
		{
			var csproj = BuildProject ("MyTabbedApplication", Platform, config, clean: true);
			var testsDir = Path.GetDirectoryName (Path.GetDirectoryName (csproj));
			var appexProjectDir = Path.Combine (testsDir, "MyActionExtension");
			var viewController = Path.Combine (appexProjectDir, "ActionViewController.cs");
			var mainExecutable = Path.Combine (AppBundlePath, "MyTabbedApplication");
			var timestamp = File.GetLastWriteTimeUtc (mainExecutable);
			var text = File.ReadAllText (viewController);

			AssertProperlyCodesigned ();

			Thread.Sleep (1000);

			// replace "bool imageFound = false;" with "bool imageFound = true;" so that we force the appex to get rebuilt
			text = text.Replace ("bool imageFound = false;", "bool imageFound = true;");
			File.WriteAllText (viewController, text);

			try {
				BuildProject ("MyTabbedApplication", Platform, config, clean: false);
				var newTimestamp = File.GetLastWriteTimeUtc (mainExecutable);

				// make sure that the main app bundle was codesigned due to the changes in the appex
				Assert.IsTrue (newTimestamp > timestamp, "The main app bundle does not seem to have been re-codesigned");

				AssertProperlyCodesigned ();
			} finally {
				// restore the original ActionViewController.cs code...
				text = text.Replace ("bool imageFound = true;", "bool imageFound = false;");
				File.WriteAllText (viewController, text);
			}
		}

		[Test]
		public void RebuildWatchAppNoChanges ()
		{
			BuildProject ("MyWatch2Container", Platform, config, clean: true);

			AssertProperlyCodesigned ();

			Thread.Sleep (1000);

			// Rebuild w/ no changes
			BuildProject ("MyWatch2Container", Platform, config, clean: false);

			// make sure everything is still codesigned properly
			AssertProperlyCodesigned ();
		}

		[Test]
		public void CodesignAfterModifyingWatchApp2Test ()
		{
			var csproj = BuildProject ("MyWatch2Container", Platform, config, clean: true);
			var testsDir = Path.GetDirectoryName (Path.GetDirectoryName (csproj));
			var appexProjectDir = Path.Combine (testsDir, "MyWatchKit2Extension");
			var viewController = Path.Combine (appexProjectDir, "InterfaceController.cs");
			var mainExecutable = Path.Combine (AppBundlePath, "MyWatch2Container");
			var timestamp = File.GetLastWriteTimeUtc (mainExecutable);
			var text = File.ReadAllText (viewController);

			AssertProperlyCodesigned ();

			Thread.Sleep (1000);

			// replace "bool imageFound = false;" with "bool imageFound = true;" so that we force the appex to get rebuilt
			text = text.Replace ("{0} awake with context", "{0} The Awakening...");
			File.WriteAllText (viewController, text);

			try {
				BuildProject ("MyWatch2Container", Platform, config, clean: false);

				AssertProperlyCodesigned ();

				var newTimestamp = File.GetLastWriteTimeUtc (mainExecutable);

				// make sure that the main app bundle was codesigned due to the changes in the appex
				// Note: this step requires msbuild instead of xbuild to work properly
				//Assert.IsTrue (newTimestamp > timestamp, "The main app bundle does not seem to have been re-codesigned");
			} finally {
				// restore the original ActionViewController.cs code...
				text = text.Replace ("{0} The Awakening...", "{0} awake with context");
				File.WriteAllText (viewController, text);
			}
		}
	}
}
