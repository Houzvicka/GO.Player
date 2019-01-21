using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace HBO.UWP.Player.Messages
{
    public class NavigateMainFrameMessage : MessageBase
    {
        public Type PageType { get; }

        public NavigateMainFrameMessage(Type pageType)
        {
            PageType = pageType;
        }
    }
}
