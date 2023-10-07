using AmongUs.GameOptions;
using UnityEngine;

namespace ImpostorsHandbook.Roles
{
    public class Crewmate : CrewmateRole
    {
        public Crewmate() : base()
        {
            Name = "Crewmate";
            Description = "";
            Blurb = "Do your tasks.";
            BaseRole = RoleTypes.Crewmate;
            RoleEnum = Enum.Role.Crewmate;
            TeamEnum = Enum.Team.Crewmate;
            Color = new Color32(140, 255, 255, 255);
        }
    }
}
