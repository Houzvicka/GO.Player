using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using CommonServiceLocator;
using HBO.UWP.Player.Contracts;
using HBO.UWP.Player.Helpers;
using HBO.UWP.Player.Model;
using Newtonsoft.Json;

namespace HBO.UWP.Player
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
    }
}
