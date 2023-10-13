using AmongUs.GameOptions;
using UnityEngine;
using ImpostorsHandbook.Enum;

namespace ImpostorsHandbook.Roles
{
    internal class GuardianAngel : CrewmateRole
    {
        public override string Name => "Guardian Angel";
        public override string Description => "";
        public override string Blurb => "Placeholder.";
        public override RoleTypes Type => RoleTypes.Crewmate;
        public override Color32 Color => new(218, 248, 255, 255);
        public override Team Team => Team.Crewmate;
        public override Role Enum => Role.GuardianAngel;

        public static GuardianAngel Instance = new();

        public GuardianAngel(PlayerControl? player = null) : base(player) { }

        public override void FixedUpdate() { }
    }
}
