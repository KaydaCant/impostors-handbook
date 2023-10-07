namespace ImpostorsHandbook.Roles
{
    public class CrewmateRole : Role
    {
        public CrewmateRole() : base() { }

        public override bool WinConditionMet()
        {
            return false;
        }
    }
}
