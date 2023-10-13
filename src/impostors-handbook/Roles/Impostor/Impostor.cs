using AmongUs.GameOptions;
using UnityEngine;
using ImpostorsHandbook.Enum;

namespace ImpostorsHandbook.Roles
{
    internal class Impostor : ImpostorRole
    {
        public override string Name => "Impostor";
        public override string Description => "";
        public override string Blurb => "Kill them all.";
        public override RoleTypes Type => RoleTypes.Impostor;
        public override Color32 Color => new(255, 25, 25, 255);
        public override Team Team => Team.Impostor;
        public override Role Enum => Role.Impostor;

        public static Impostor Instance = new();

        public Impostor(PlayerControl? player = null) : base(player) { }

        public override void FixedUpdate() { }
    }
}
