using AmongUs.GameOptions;
using UnityEngine;

namespace ImpostorsHandbook.Roles
{
    public abstract class BaseRole
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Blurb { get; }
        public abstract RoleTypes Type { get; }
        public abstract Color32 Color { get; }
        public abstract Enum.Team Team { get; }
        public abstract Enum.Role Enum { get; }

        public readonly PlayerControl? player;
        public BaseRole(PlayerControl? player = null)
        {
            this.player = player;
        }

        public abstract bool WinCriteriaMet();
    }
}
