using System;

namespace ImpostorsHandbook.Roles
{
    public abstract class CrewmateRole : BaseRole
    {
        public CrewmateRole(PlayerControl? player = null) : base(player) { }

        public override bool WinCriteriaMet()
        {
            throw new NotImplementedException();
        }
    }
}
