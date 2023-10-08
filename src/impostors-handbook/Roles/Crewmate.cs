using AmongUs.GameOptions;
using UnityEngine;
using ImpostorsHandbook.Enum;

namespace ImpostorsHandbook.Roles
{
    internal class Crewmate : CrewmateRole
    {
        public override string Name => "Crewmate";
        public override string Description => "";
        public override string Blurb => "Do your tasks.";
        public override RoleTypes Type => RoleTypes.Crewmate;
        public override Color32 Color => new(140, 255, 255, 255);
        public override Team Team => Team.Crewmate;
        public override Role Enum => Role.Crewmate;

        public static Crewmate Instance = new();

        public Crewmate(PlayerControl? player = null) : base(player) { }
    }
}
