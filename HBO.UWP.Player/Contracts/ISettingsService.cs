using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBO.UWP.Player.Contracts
{
    public interface ISettingsService
    {
        string CurrentDeviceId { get; set; }
        string Individualization { get; set; }
        string TempUriTest { get; set; }
    }
}
