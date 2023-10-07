using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.Linq;
using ImpostorsHandbook.Roles;

namespace ImpostorsHandbook.Managers
{
    public class RandomizationSettings
    {
        public int PlayerCount;
        public int ImpostorCount;
        public int NeutralMaxCount;

        public RandomizationSettings(int playerCount, int impostorCount, int neutralMaxCount) 
        {
            this.PlayerCount = playerCount;
            this.ImpostorCount = impostorCount;
            this.NeutralMaxCount = neutralMaxCount;
        }
    }

    public class RoleOpportunity
    {
        public Enum.Role Role;
        public Enum.Team Team;
        public double Chance;

        public RoleOpportunity(Enum.Role role, Enum.Team team, double chance)
        {
            this.Role = role;
            this.Team = team;
            this.Chance = chance;
        }
    }

    public static class RoleManager
    {
        public static List<Enum.Role> RandomizeRoles(RandomizationSettings randomizationSettings, List<RoleOpportunity> roleSettings, int? seed = null)
        {
            Random random;
            if (seed == null) random = new Random();
            else random = new Random((int) seed);

            int playerCount = 0;
            int impostorCount = 0;
            int neutralCount = 0;
            List<RoleOpportunity> shuffledRoles = roleSettings.OrderBy(x => random.Next()).ToList();

            List<Enum.Role> winners = new();

            foreach (RoleOpportunity opportunity in shuffledRoles)
            {
                if (playerCount >= randomizationSettings.PlayerCount) continue;
                if (opportunity.Team == Enum.Team.Impostor && impostorCount >= randomizationSettings.ImpostorCount) continue;
                if (opportunity.Team == Enum.Team.Neutral && neutralCount >= randomizationSettings.NeutralMaxCount) continue;

                double roll = random.NextDouble();
                if (roll <= opportunity.Chance)
                {
                    winners.Add(opportunity.Role);
                    playerCount++;
                    if (opportunity.Team == Enum.Team.Impostor) impostorCount++;
                    if (opportunity.Team == Enum.Team.Neutral) neutralCount++;
                }
            }

            for (int i = impostorCount; i < randomizationSettings.ImpostorCount; i++)
            {
                winners.Add(Enum.Role.Impostor);
            }

            for (int i = playerCount; i < randomizationSettings.PlayerCount; i++)
            {
                winners.Add(Enum.Role.Crewmate);
            }

            List<Enum.Role> output = winners.OrderBy(x => random.Next()).ToList();
            return output;
        }

        public static Role EnumToRole(Enum.Role role)
        {
            return role switch
            {
                Enum.Role.Crewmate => new Crewmate(),
                Enum.Role.Impostor => new Impostor(),
                Enum.Role.Jester => new Jester(),
                _ => new Crewmate(),
            };
        }
    }
}
