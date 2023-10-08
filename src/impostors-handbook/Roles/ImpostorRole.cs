using System;

namespace ImpostorsHandbook.Roles
{
    public abstract class ImpostorRole : BaseRole
    {
        public ImpostorRole(PlayerControl? player = null) : base(player) { }

        public override bool WinCriteriaMet()
        {
            throw new NotImplementedException();
        }
    }
}
