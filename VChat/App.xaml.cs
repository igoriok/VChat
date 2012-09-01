using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using Caliburn.Micro;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using VChat.Controls;
using VChat.ViewModels;

namespace VChat
{
    public partial class App : Application
    {
        public PhoneApplicationFrame RootFrame
        {
            get { return (PhoneApplicationFrame)RootVisual; }
        }

        public static byte[] DeviceUniqueId
        {
            get { return DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[]; }
        }

        public static string DeviceName
        {
            get { return DeviceStatus.DeviceName; }
        }

        public static string DeviceManufacturer
        {
            get { return DeviceStatus.DeviceManufacturer; }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            UnhandledException += Application_UnhandledException;
            Startup += Application_Startup;
            Exit += Application_Exit;

            InitializeComponent();

            TiltEffect.TiltableItems.Add(typeof(StatusIcon));
            ConventionManager.AddElementConvention<MenuItem>(ItemsControl.ItemsSourceProperty, "DataContext", "Click");

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                BackgroundAudioPlayer.Instance.Stop();
            }
        }

        private void Application_Exit(object sender, EventArgs e)
        {
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
        }
    }
}