using AmongUs.GameOptions;
using HarmonyLib;
using ImpostorsHandbook.Roles;
using UnityEngine;

namespace ImpostorsHandbook.Roles
{
    public class Jester : Role
    {
        private bool wasVotedOut;

        public Jester() : base()
        {
            Name = "Jester";
            Description = "";
            Blurb = "Get voted out.";
            BaseRole = RoleTypes.Crewmate;
            RoleEnum = Enum.Role.Jester;
            TeamEnum = Enum.Team.Neutral;
            Color = new Color32(146, 60, 255, 255);

            wasVotedOut = false;
        }

        public override bool WinConditionMet()
        {
            return wasVotedOut;
        }
    }
}
