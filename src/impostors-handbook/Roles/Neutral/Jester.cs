using AmongUs.GameOptions;
using UnityEngine;
using ImpostorsHandbook.Enum;

namespace ImpostorsHandbook.Roles
{
    internal class Jester : BaseRole
    {
        public override string Name => "Jester";
        public override string Description => "";
        public override string Blurb => "Get voted out.";
        public override RoleTypes Type => RoleTypes.Crewmate;
        public override Color32 Color => new(146, 60, 255, 255);
        public override Team Team => Team.Neutral;
        public override Role Enum => Role.Jester;

        public static Jester Instance = new();

        public Jester(PlayerControl? player = null) : base(player)
        {
            isVotedOut = false;
        }

        public override void FixedUpdate() { }

        public bool isVotedOut;
    }
}
