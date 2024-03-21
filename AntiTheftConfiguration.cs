using Rocket.API;

namespace Zombs_R_CuteVehicleAntiTheft
{
    public class AntiTheftConfiguration:IRocketPluginConfiguration
    {
        public bool EnableAdminOverride;
        public bool AllowVehicleToBeLockpicked;
        public bool AllowAnyPlayerToSwitchToDriverSeat;
        public bool AllowAllVehiclesToBeCarjacked;
        
        public void LoadDefaults()
        {
            EnableAdminOverride = true;
            AllowVehicleToBeLockpicked = false;
            AllowAnyPlayerToSwitchToDriverSeat = false;
            AllowAllVehiclesToBeCarjacked = true;
        }
    }
}