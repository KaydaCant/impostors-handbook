using AmongUs.GameOptions;
using UnityEngine;
using ImpostorsHandbook.Enum;

namespace ImpostorsHandbook.Roles
{
    internal class Engineer : CrewmateRole
    {
        public override string Name => "Engineer";
        public override string Description => "";
        public override string Blurb => "Placeholder.";
        public override RoleTypes Type => RoleTypes.Engineer;
        public override Color32 Color => new(255, 189, 60, 255);
        public override Team Team => Team.Crewmate;
        public override Role Enum => Role.Engineer;

        public static Engineer Instance = new();

        public Engineer(PlayerControl? player = null) : base(player) { }

        public override void FixedUpdate() { }
    }
}
