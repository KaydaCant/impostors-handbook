using AmongUs.GameOptions;
using UnityEngine;
using ImpostorsHandbook.Enum;

namespace ImpostorsHandbook.Roles
{
    internal class Shapeshifter : ImpostorRole
    {
        public override string Name => "Shapeshifter";
        public override string Description => "";
        public override string Blurb => "Placeholder.";
        public override RoleTypes Type => RoleTypes.Shapeshifter;
        public override Color32 Color => new(209, 17, 74, 255);
        public override Team Team => Team.Impostor;
        public override Role Enum => Role.Shapeshifter;

        public static Shapeshifter Instance = new();

        public Shapeshifter(PlayerControl? player = null) : base(player) { }

        public override void FixedUpdate() { }
    }
}
