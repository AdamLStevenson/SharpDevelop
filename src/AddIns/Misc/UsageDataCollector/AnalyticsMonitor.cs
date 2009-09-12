﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop;
using System.IO;

namespace ICSharpCode.UsageDataCollector
{
	/// <summary>
	/// Main singleton class of the analytics. This class is thread-safe.
	/// </summary>
	public sealed class AnalyticsMonitor : IAnalyticsMonitor
	{
		public static readonly AnalyticsMonitor Instance = new AnalyticsMonitor();
		
		public static bool EnabledIsUndecided {
			get {
				return string.IsNullOrEmpty(PropertyService.Get("ICSharpCode.UsageDataCollector.Enabled"));
			}
		}
		
		public static bool Enabled {
			get {
				return string.Equals(PropertyService.Get("ICSharpCode.UsageDataCollector.Enabled"), bool.TrueString, StringComparison.OrdinalIgnoreCase);
			}
			set {
				PropertyService.Set("ICSharpCode.UsageDataCollector.Enabled", value.ToString());
				if (value) {
					Instance.OpenSession();
				} else {
					Instance.CloseSession();
					Instance.TryDeleteDatabase();
				}
			}
		}
		
		readonly object lockObj = new object();
		string dbFileName;
		AnalyticsSessionWriter session;
		
		private AnalyticsMonitor()
		{
			var container = ServiceManager.Instance.GetRequiredService<ThreadSafeServiceContainer>();
			container.TryAddService(typeof(IAnalyticsMonitor), this);
			dbFileName = Path.Combine(PropertyService.ConfigDirectory, "usageData.dat");
			
			SharpDevelop.Gui.WorkbenchSingleton.WorkbenchUnloaded += delegate { CloseSession(); };
		}
		
		public void OpenSession()
		{
			lock (lockObj) {
				if (session == null) {
					try {
						session = new AnalyticsSessionWriter(dbFileName);
					} catch (DatabaseTooNewException) {
						LoggingService.Warn("Could not use AnalyticsMonitor: too new version of database");
					}
				}
			}
		}
		
		void TryDeleteDatabase()
		{
			lock (lockObj) {
				CloseSession();
				try {
					File.Delete(dbFileName);
				} catch (IOException) {
				} catch (AccessViolationException) {
				}
			}
		}
		
		public void CloseSession()
		{
			lock (lockObj) {
				if (session != null) {
					session.Dispose();
					session = null;
				}
			}
		}
		
		public void TrackException(Exception exception)
		{
			lock (lockObj) {
				if (session != null) {
					// TODO: recognize inner exceptions
					session.AddException(exception.GetType().FullName, exception.StackTrace);
				}
			}
		}
		
		public IAnalyticsMonitorTrackedFeature TrackFeature(string featureName, string activationMethod)
		{
			TrackedFeature feature = new TrackedFeature();
			lock (lockObj) {
				if (session != null) {
					feature.IsTracked = true;
					feature.FeatureID = session.AddFeatureUse(featureName, activationMethod);
				}
			}
			return feature;
		}
		
		void EndTracking(TrackedFeature feature)
		{
			lock (lockObj) {
				if (session != null && feature.IsTracked) {
					feature.IsTracked = false;
					session.WriteEndTimeForFeature(feature.FeatureID);
				}
			}
		}
		
		sealed class TrackedFeature : IAnalyticsMonitorTrackedFeature
		{
			internal bool IsTracked;
			internal long FeatureID;
			
			public void EndTracking()
			{
				Instance.EndTracking(this);
			}
		}
	}
	
	public class AutoStartCommand : AbstractCommand
	{
		public override void Run()
		{
			if (AnalyticsMonitor.Enabled)
				AnalyticsMonitor.Instance.OpenSession();
		}
	}
}