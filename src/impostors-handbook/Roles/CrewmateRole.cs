using ImpostorsHandbook.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostorsHandbook.Roles
{
    public abstract class CrewmateRole : BaseRole
    {
        public CrewmateRole(PlayerControl? player = null) : base(player) { }
    }
}
