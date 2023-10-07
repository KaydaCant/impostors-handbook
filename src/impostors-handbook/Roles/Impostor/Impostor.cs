using AmongUs.GameOptions;
using UnityEngine;

namespace ImpostorsHandbook.Roles
{
    public class Impostor : ImpostorRole
    {
        public Impostor() : base()
        {
            Name = "Impostor";
            Description = "";
            Blurb = "Kill them idk.";
            BaseRole = RoleTypes.Impostor;
            RoleEnum = Enum.Role.Impostor;
            TeamEnum = Enum.Team.Impostor;
            Color = new Color32(255, 25, 25, 255);
        }
    }
}
