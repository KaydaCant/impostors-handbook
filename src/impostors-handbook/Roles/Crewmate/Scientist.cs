using AmongUs.GameOptions;
using UnityEngine;
using ImpostorsHandbook.Enum;

namespace ImpostorsHandbook.Roles
{
    internal class Scientist : CrewmateRole
    {
        public override string Name => "Scientist";
        public override string Description => "";
        public override string Blurb => "Placeholder.";
        public override RoleTypes Type => RoleTypes.Scientist;
        public override Color32 Color => new(140, 255, 172, 255);
        public override Team Team => Team.Crewmate;
        public override Role Enum => Role.Scientist;

        public static Scientist Instance = new();

        public Scientist(PlayerControl? player = null) : base(player) { }

        public override void FixedUpdate() { }
    }
}
