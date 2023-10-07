namespace ImpostorsHandbook.Roles
{
    public class ImpostorRole : Role
    {
        public ImpostorRole() : base() { }

        public override bool WinConditionMet()
        {
            return false;
        }
    }
}