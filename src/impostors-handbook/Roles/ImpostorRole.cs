using Epic.OnlineServices.AntiCheatCommon;
using ImpostorsHandbook.Enum;
using ImpostorsHandbook.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostorsHandbook.Roles
{
    public abstract class ImpostorRole : BaseRole
    {
        public ImpostorRole(PlayerControl? player = null) : base(player) { }
    }
}
