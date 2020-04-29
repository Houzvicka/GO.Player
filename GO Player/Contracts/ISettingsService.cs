using System;

namespace GO_Player.Services
{
    public interface ISettingsService
    {
        string CurrentDeviceId { get; set; }
        string Individualization { get; set; }

        string Username { get; set; }
        string Password { get; set; }
        Guid OperatorId { get; set; }
    }
}