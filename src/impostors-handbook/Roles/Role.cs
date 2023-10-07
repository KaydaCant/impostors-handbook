using AmongUs.GameOptions;
using UnityEngine;

namespace ImpostorsHandbook.Roles
{
    public abstract class Role
    {
        protected string Name { get; set; }
        protected string Description { get; set; }
        protected string Blurb { get; set; }
        protected RoleTypes BaseRole { get; set; }
        protected Enum.Role RoleEnum { get; set; }
        protected Enum.Team TeamEnum { get; set; }
        protected Color32 Color { get; set; }

        protected Role()
        {
            Name ??= "???"; Description ??= "???"; Blurb ??= "???"; // Set values if null
        }

        public abstract bool WinConditionMet();

        public string GetName() { return Name; }
        public string GetDescription() {  return Description; }
        public string GetBlurb() {  return Blurb; }
        public RoleTypes GetBaseRole() { return BaseRole; }
        public Enum.Role GetRole() { return RoleEnum; }
        public Enum.Team GetTeam() { return TeamEnum; }
        public Color32 GetColor() { return Color; }

    }
}