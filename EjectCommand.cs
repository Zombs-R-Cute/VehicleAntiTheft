using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Zombs_R_CuteVehicleAntiTheft
{
    public class EjectCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] commands)
        {
            
            UnturnedPlayer callingPlayer = UnturnedPlayer.FromName(caller.Id);
            InteractableVehicle vehicle;

            if (!Physics.Raycast(callingPlayer.Player.look.aim.position, callingPlayer.Player.look.aim.forward,
                out RaycastHit hit, 5, RayMasks.VEHICLE))
                return;
            
            vehicle = hit.transform.GetComponentInParent<InteractableVehicle>();
            if (!AntiTheft.IsPlayerOwnerOrInGroup(vehicle, callingPlayer) && !(callingPlayer.IsAdmin && AntiTheft.ConfiguratonIstance.EnableAdminOverride))
                return;

            if (vehicle.speed > 0)
            {
                UnturnedChat.Say(callingPlayer, "The vehicle must be stopped!", Color.red);
                return;
            }

            var passengers = vehicle.passengers;
            foreach (var passenger in passengers)
            {
                if (passenger.player == null)
                    continue;
            
                UnturnedPlayer pl = UnturnedPlayer.FromSteamPlayer(passenger.player);
                    
                if (AntiTheft.IsPlayerOwnerOrInGroup(vehicle, pl))
                    continue;
            

                if (commands.Length == 0 || commands.Length > 0 && pl.DisplayName.ToLower().Contains(commands[0].ToLower()))
                {
                    UnturnedChat.Say(pl, "The owner threw you out of the vehicle.", Color.red);
                    pl.CurrentVehicle.forceRemovePlayer(out byte seat, pl.CSteamID, out Vector3 pos, out byte angle);
                    VehicleManager.sendExitVehicle(vehicle, seat, pos, angle, true);
                }

            }
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "eject";
        public string Help => "Eject player(s) from vehicle";
        public string Syntax => "eject [name]";
        public List<string> Aliases => new List<string>() {"ej"};
        public List<string> Permissions => new List<string>() {"antitheft.eject"};
    }
}