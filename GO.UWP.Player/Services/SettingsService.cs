using GO.UWP.Player.Contracts;
using GO.UWP.Player.Helpers;

namespace GO.UWP.Player.Services
{
    public class SettingsService : ISettingsService
    {
        public string CurrentDeviceId
        {
            get { return durrentDeviceId.Value; }
            set { durrentDeviceId.Value = value; }
        }
        private readonly LocalSetting<string> durrentDeviceId = new LocalSetting<string>(nameof(CurrentDeviceId));

        public string Individualization
        {
            get { return individualization.Value; }
            set { individualization.Value = value; }
        }
        private readonly LocalSetting<string> individualization = new LocalSetting<string>(nameof(Individualization));

        public string Username
        {
            get { return username.Value; }
            set { username.Value = value; }
        }
        private readonly LocalSetting<string> username = new LocalSetting<string>(nameof(Username));

        public string Password
        {
            get { return password.Value; }
            set { password.Value = value; }
        }
        private readonly LocalSetting<string> password = new LocalSetting<string>(nameof(Password));

        public int OperatorId
        {
            get { return operatorId.Value; }
            set { operatorId.Value = value; }
        }
        private readonly LocalSetting<int> operatorId = new LocalSetting<int>(nameof(OperatorId));
    }
}
