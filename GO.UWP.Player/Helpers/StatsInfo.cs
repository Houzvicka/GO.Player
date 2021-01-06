using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Security.Cryptography;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Xaml;

namespace GO.UWP.Player.Helpers
{
	/// <summary>
	/// Helper class containig basic information about system and the application.
	/// </summary>
	public class StatsInfo
	{
		/// <summary>
		/// Current app version in format "Major.Minor.Build.Revision"
		/// </summary>
		public static string AppVersion { get; private set; }

		/// <summary>
		/// Device name in format "{manufacturer} {model}"
		/// </summary>
		public static string DeviceModel { get; private set; }

		/// <summary>
		/// Type of the device, either "phone" or "tablet"
		/// </summary>
		public static string DeviceType { get; private set; }

		/// <summary>
		/// OS version in format WindowsPhone_8.1 or Windows_8.1 or Windows_Major.Minor.Build.Revision
		/// </summary>
		public static string OsVersion { get; private set; }

		/// <summary>
		/// The size of the current Window
		/// </summary>
		public static Size ScreenSize
		{
			get
			{
				Window current = Window.Current;
				return current != null ? new Size(current.Bounds.Width, current.Bounds.Height) : Size.Empty;
			}
		}

		/// <summary>
		/// Unique Device Id gatherd from the <see cref="HardwareIdentification"/> class.
		/// </summary>
		public static string DeviceId => lazyDeviceId.Value;
		private static readonly Lazy<string> lazyDeviceId = new Lazy<string>(GetDeviceIdInternal);

		/// <summary>
		/// Unique installation id, concatenated <see cref="DeviceId"/> and current timestamp.
		/// </summary>
		public static string InstallationId => lazyInstallationId.Value;
		private static readonly Lazy<string> lazyInstallationId = new Lazy<string>(GetInstallationId);

		/// <summary>
		/// Date and time of the first app installation in sortable format.
		/// </summary>
		public static string FirstInstall => lazyFirstInstall.Value;
		private static readonly Lazy<string> lazyFirstInstall = new Lazy<string>(GetFirstInstall);

		static StatsInfo()
		{
			PackageVersion ivd = Package.Current.Id.Version;
			AppVersion = $"{ivd.Major}.{ivd.Minor}.{ivd.Build}.{ivd.Revision}";

			DetectDeviceType();
		}

		private static string GetDeviceIdInternal()
		{
			// HardwareIdentification does not work on HoloLens/Xbox One/Surface Hub or IoT before Windows 10.0.14393
			if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.System.Profile.HardwareIdentification"))
			{
				HardwareToken token = HardwareIdentification.GetPackageSpecificToken(null);
				byte[] bytes = token.Id.ToArray();
				return BitConverter.ToString(bytes);
			}
			else
			{
				// using random string as a fallback
				IBuffer deviceIdBuffer = CryptographicBuffer.GenerateRandom(32);
				return CryptographicBuffer.EncodeToHexString(deviceIdBuffer);
			}
		}

		private static string GetInstallationId()
		{
			return installationId.Value ?? (installationId.Value = $"{DeviceId}.{DateTimeOffset.UtcNow.Ticks}");
		}
		private static readonly LocalSetting<string> installationId = new LocalSetting<string>("SznStatistics.InstallationId");

		private static string GetFirstInstall()
		{
			return firstInstall.Value ?? (firstInstall.Value = DateTime.UtcNow.ToString("yyyyMMddTHH:mm:ss.fff"));
		}
		private static readonly LocalSetting<string> firstInstall = new LocalSetting<string>("SznStatistics.FirstInstall");

		private const string AiTypeName = "Windows.System.Profile.AnalyticsInfo, Windows.System, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime";
		private const string AviTypeName = "Windows.System.Profile.AnalyticsVersionInfo, Windows.System, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime";

		private static void DetectDeviceType()
		{
			AnalyticsVersionInfo avi = AnalyticsInfo.VersionInfo;
			// set the device type based on the Windows 10 DeviceFamily property
			string deviceFamily = avi.DeviceFamily;
			switch (deviceFamily)
			{
				case "Windows.Mobile":
					DeviceType = "phone";
					break;
				case "Windows.Desktop":
					DeviceType = "tablet";
					break;
				case "Windows.Xbox":
					DeviceType = "xbox";
					break;
				default:
					// IoT, Surface Hub, HoloLens?
					DeviceType = deviceFamily;
					break;
			}

			// set the full OS version using the DeviceFamilyVersion property
			string sv = avi.DeviceFamilyVersion;
			SetOsVersion(sv);
        }

		private static void SetOsVersion(string sv)
		{
			ulong v = ulong.Parse(sv);
			ulong v1 = (v & 0xFFFF000000000000L) >> 48;
			ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
			ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
			ulong v4 = (v & 0x000000000000FFFFL);
			OsVersion = $"Windows_{v1}.{v2}.{v3}.{v4}";
		}
	}
}