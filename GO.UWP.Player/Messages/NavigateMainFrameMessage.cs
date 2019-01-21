using System;
using GalaSoft.MvvmLight.Messaging;

namespace GO.UWP.Player.Messages
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
