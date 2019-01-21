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
        
        string Username { get; set; }
        string Password { get; set; }
        int OperatorId { get; set; }
    }
}
