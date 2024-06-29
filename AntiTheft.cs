using System;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Zombs_R_CuteVehicleAntiTheft
{
    public class AntiTheft : RocketPlugin<AntiTheftConfiguration>
    {
        public static AntiTheftConfiguration ConfiguratonIstance;

        protected override void Load()
        {
            ConfiguratonIstance = Configuration.Instance;
            Logger.Log("AdminOverride is " + (Configuration.Instance.EnableAdminOverride ? "Enabled" : "Disabled"));
            Logger.Log("Lockpicking is " +
                       (Configuration.Instance.AllowVehicleToBeLockpicked ? "Allowed" : "Disallowed"));
            Logger.Log("Swapping to driver seat is " +
                       (Configuration.Instance.AllowAnyPlayerToSwitchToDriverSeat ? "Allowed" : "Disallowed"));

            VehicleManager.onVehicleLockpicked +=
                (InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow) =>
                {
                    if (instigatingPlayer == null && vehicle.isInsideSafezone)
                    {
                        allow = true;
                        return;
                    }

                    var player = UnturnedPlayer.FromPlayer(instigatingPlayer);
                    
                    allow = Configuration.Instance.AllowVehicleToBeLockpicked ||
                            player.IsAdmin && Configuration.Instance.EnableAdminOverride;

                    if (!allow)
                        UnturnedChat.Say(player, "You are not allowed to unlock this vehicle!", Color.red);
                };

            VehicleManager.onSwapSeatRequested += (Player instigatingPlayer, InteractableVehicle vehicle,
                ref bool allow,
                byte index, ref byte seatIndex) =>
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(instigatingPlayer);
                allow = Configuration.Instance.AllowAnyPlayerToSwitchToDriverSeat || seatIndex != 0 ||
                        IsPlayerOwnerOrInGroup(vehicle, player) ||
                        UnturnedPlayer.FromPlayer(instigatingPlayer).IsAdmin &&
                        Configuration.Instance.EnableAdminOverride||!vehicle.isLocked;

                if (!allow)
                    UnturnedChat.Say(player, "You are not allowed to drive this vehicle!", Color.red);
            };


            VehicleManager.onVehicleCarjacked += (InteractableVehicle vehicle, Player instigatingPlayer,
                ref bool allow,
                ref Vector3 force, ref Vector3 torque) =>
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(instigatingPlayer);
                allow = IsPlayerOwnerOrInGroup(vehicle, player) ||
                        Configuration.Instance.AllowAllVehiclesToBeCarjacked ||
                        player.IsAdmin && Configuration.Instance.EnableAdminOverride ||
                        vehicle.lockedOwner == CSteamID.Nil;
            };
            
            UseableTire.onModifyTireRequested +=
                (UseableTire useable, InteractableVehicle vehicle, int index, ref bool allow) =>
                {
                    allow = Configuration.Instance.AllowTiresToBeDamaged;
                };
            
            VehicleManager.onDamageTireRequested += (CSteamID id, InteractableVehicle vehicle, int index,
                ref bool allow, EDamageOrigin origin) =>
            {
                allow = Configuration.Instance.AllowTiresToBeDamaged;
            };
            
            
        }

        public static bool IsPlayerOwnerOrInGroup(InteractableVehicle vehicle, UnturnedPlayer player)
        {
            return vehicle.lockedOwner == player.CSteamID ||
                   vehicle.lockedGroup == player.Player.quests.groupID && vehicle.lockedGroup != CSteamID.Nil;
        }
    }
}