using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Email;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GO.UWP.Player.Extensions;
using GO.UWP.Player.Helpers;
using GO.UWP.Player.Pages;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace GO.UWP.Player
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox")
            {
                this.RequiresPointerMode = Windows.UI.Xaml.ApplicationRequiresPointerMode.WhenRequested;
            }

            AppCenter.Start("ebe8eb3b-643e-4dc1-b1be-c209fbdfb57d", typeof(Analytics), typeof(Crashes));
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            MainPage page = Window.Current.Content as MainPage;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (page == null)
            {
                page = new MainPage();
                Window.Current.Content = page;
            }

            if (e.PrelaunchActivated == false)
            {
                // Ensure the current window is active
                Window.Current.Activate();
            }

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox")
            {
                this.FocusVisualKind = FocusVisualKind.Reveal;
            }
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            Debug.WriteLine($"EXCEPTION: {args.Message}");
            ShowError(args);
            if (Debugger.IsAttached) Debugger.Break();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
        
        private async void ShowError(Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // warning Do not include this test Error dialog in the Release build!
            e.Handled = true;
            try
            {
                var msgbox = new ContentDialog
                {
                    Title = "Nastala výjimka v aplikaci!",
                    Content = "V aplikaci nastala chyba, odeslat hlášení o chybě mailem?",
                    PrimaryButtonText = "Odeslat",
                    CloseButtonText = "Zrušit"
                };
                var res = await msgbox.ShowAsync();
                if (res != ContentDialogResult.Secondary) return;

                EmailMessage mail = new EmailMessage();
                mail.To.Add(new EmailRecipient("app@jiri.it"));
                mail.Subject = "[Exception] GO Player";
                mail.Body = $"Seznam.cz {StatsInfo.AppVersion}, {StatsInfo.OsVersion}, {StatsInfo.DeviceModel}" +
                            $"\n\n{e.Message}\n\n{e.Exception.ToNiceString(6)}";
                await EmailManager.ShowComposeNewEmailAsync(mail);
            }
            catch (Exception ex2)
            {
                Debug.WriteLine($"Error sending error report: {ex2}");
            }
            Current.Exit();
        }
    }
}
