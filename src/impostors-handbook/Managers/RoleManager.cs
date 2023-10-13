using System;
using System.Collections.Generic;
using System.Linq;
using ImpostorsHandbook.Roles;
using Reactor.Utilities;

namespace ImpostorsHandbook.Managers
{
    public class RandomizationSettings
    {
        public int PlayerCount;
        public int ImpostorCount;
        public int NeutralMaxCount;

        public RandomizationSettings(int playerCount, int impostorCount, int neutralMaxCount) 
        {
            PlayerCount = playerCount;
            ImpostorCount = impostorCount;
            NeutralMaxCount = neutralMaxCount;
        }
    }

    public class RoleOpportunity
    {
        public BaseRole Role;
        public double Chance;

        public RoleOpportunity(BaseRole role, double chance)
        {
            Role = role;
            Chance = chance;
        }

        public RoleOpportunity(Enum.Role role, double chance)
        {
            Role = RoleManager.GetRole(role);
            Chance = chance;
        }
    }

    public static class RoleManager
    {
        public static List<Enum.Role> RandomizeRoles(RandomizationSettings randomizationSettings, List<RoleOpportunity> roleOpportunities, int? seed = null)
        {
            Random random;
            if (seed == null) random = new Random();
            else random = new Random((int) seed);

            int playerCount = 0;
            int impostorCount = 0;
            int neutralCount = 0;
            //List<RoleOpportunity> shuffledRoles = roleSettings.OrderBy(x => (int) x.Team * 1000 + random.Next()).ToList();

            List<Enum.Role> winners = new();

            /*foreach (RoleOpportunity opportunity in shuffledRoles)
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
            }*/

            List<RoleOpportunity> randomOpportunities = roleOpportunities.OrderBy(x => random.Next()).ToList();
            List<RoleOpportunity> impostorOpportunities = randomOpportunities.Where(x => x.Role.Team == Enum.Team.Impostor).ToList();
            List<RoleOpportunity> neutralOpportunities = randomOpportunities.Where(x => x.Role.Team == Enum.Team.Neutral).ToList();
            List<RoleOpportunity> crewmateOpportunities = randomOpportunities.Where(x => x.Role.Team == Enum.Team.Crewmate).ToList();

            foreach (RoleOpportunity impostorOpportunity in impostorOpportunities)
            {
                Logger<Plugin>.Info($"Rolling for IMPOSTOR, Role: {impostorOpportunity.Role}");
                double roll = random.NextDouble();
                Logger<Plugin>.Info($"Rolled. Chance: {impostorOpportunity.Chance}, Roll: {roll}");
                if (roll > impostorOpportunity.Chance) continue;
                Logger<Plugin>.Info($"Roll passed! Adding to winners.");
                winners.Add(impostorOpportunity.Role.Enum);
                impostorCount++;
                playerCount++;

                if (impostorCount >= randomizationSettings.ImpostorCount) break;
            }

            for (int i = impostorCount; i < randomizationSettings.ImpostorCount; i++)
            {
                Logger<Plugin>.Info($"Current impostor count: {impostorCount}/{randomizationSettings.ImpostorCount}");
                Logger<Plugin>.Info($"Adding extra impostor to reach quota.");
                winners.Add(Enum.Role.Impostor);
                playerCount++;
                impostorCount++;
            }

            foreach (RoleOpportunity neutralOpportunity in neutralOpportunities)
            {
                Logger<Plugin>.Info($"Rolling for NEUTRAL, Role: {neutralOpportunity.Role}");
                double roll = random.NextDouble();
                Logger<Plugin>.Info($"Rolled. Chance: {neutralOpportunity.Chance}, Roll: {roll}");
                if (roll > neutralOpportunity.Chance) continue;
                Logger<Plugin>.Info($"Roll passed! Adding to winners.");
                winners.Add(neutralOpportunity.Role.Enum);
                neutralCount++;
                playerCount++;

                if (neutralCount >= randomizationSettings.NeutralMaxCount) break;
            }

            foreach (RoleOpportunity crewmateOpportunity in crewmateOpportunities)
            {
                Logger<Plugin>.Info($"Rolling for CREWMATE, Role: {crewmateOpportunity.Role}");
                double roll = random.NextDouble();
                Logger<Plugin>.Info($"Rolled. Chance: {crewmateOpportunity.Chance}, Roll: {roll}");
                if (roll > crewmateOpportunity.Chance) continue;
                Logger<Plugin>.Info($"Roll passed! Adding to winners.");
                winners.Add(crewmateOpportunity.Role.Enum);
                playerCount++;

                if (playerCount >= randomizationSettings.PlayerCount) break;
            }

            for (int i = playerCount; i < randomizationSettings.PlayerCount; i++)
            {
                Logger<Plugin>.Info($"Current crewmate count: {playerCount}/{randomizationSettings.PlayerCount}");
                Logger<Plugin>.Info($"Adding extra crewmate to reach quota.");
                
                winners.Add(Enum.Role.Crewmate);
                playerCount++;
            }

            List<Enum.Role> output = winners.OrderBy(x => random.Next()).ToList();
            return output;
        }

        public static BaseRole GetRole(Enum.Role role, PlayerControl player)
        {
            return role switch
            {
                Enum.Role.Crewmate => new Crewmate(player),
                Enum.Role.Engineer => new Engineer(player),
                Enum.Role.Scientist => new Scientist(player),
                Enum.Role.GuardianAngel => new GuardianAngel(player),
                Enum.Role.Seer => new Seer(player),

                Enum.Role.Impostor => new Impostor(player),
                Enum.Role.Shapeshifter => new Shapeshifter(player),

                Enum.Role.Jester => new Jester(player),

                _ => new Crewmate(player),
            };
        }
        
        public static BaseRole GetRole(Enum.Role role)
        {
            return role switch
            {
                Enum.Role.Crewmate => Crewmate.Instance,
                Enum.Role.Engineer => Engineer.Instance,
                Enum.Role.Scientist => Scientist.Instance,
                Enum.Role.GuardianAngel => GuardianAngel.Instance,
                Enum.Role.Seer => Seer.Instance,

                Enum.Role.Impostor => Impostor.Instance,
                Enum.Role.Shapeshifter => Shapeshifter.Instance,

                Enum.Role.Jester => Jester.Instance,

                _ => Crewmate.Instance,
            };
        }
    }
}
