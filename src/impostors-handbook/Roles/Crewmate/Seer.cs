using AmongUs.GameOptions;
using UnityEngine;
using ImpostorsHandbook.Enum;
using ImpostorsHandbook.Managers;
using Reactor.Utilities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ImpostorsHandbook.Roles
{
    internal class Seer : CrewmateRole
    {
        public override string Name => "Seer";
        public override string Description => "";
        public override string Blurb => "Placeholder.";
        public override RoleTypes Type => RoleTypes.Crewmate;
        public override Color32 Color => new(255, 140, 255, 255);
        public override Team Team => Team.Crewmate;
        public override Role Enum => Role.Seer;

        public static Seer Instance = new();

        // SEER ORB BUTTON:
        // Place: Put the orb on the nearest player, 5 second cooldown
        // Retrieve: Returns the orb to the player. No cooldown.

        public Seer(PlayerControl? player = null) : base(player) {
            Logger<Plugin>.Info($"Player: {player}");
            if (player == null) return;
            Logger<Plugin>.Info($"Am Owner: {player.AmOwner}");
            if (!player.AmOwner) return;

            abilityButton = DestroyableSingleton<HudManager>.Instance.AbilityButton;

            abilityButton.SetInfiniteUses();
            abilityButton.graphic.sprite = AssetManager.buttonPlaceOrb;
            abilityButton.graphic.SetCooldownNormalizedUvs();
            abilityButton.buttonLabelText.text = "Place";

            abilityButton.SetEnabled();
            abilityButton.gameObject.SetActive(true);
            abilityButton.SetCoolDown(0f, 1f);

            abilityCooldown = 0;
            hasOrb = true;

            Logger<Plugin>.Info("Created orb button.");
            Logger<Plugin>.Info($"Sprite: {AssetManager.buttonPlaceOrb}");
        }

        // Look at RoleBehaviour#SetPlayerTarget(PlayerControl target)

        public override void FixedUpdate() {
            if (this != PlayerManager.MyRole) return;

            abilityCooldown -= Time.fixedDeltaTime;
            abilityCooldown = Math.Max(abilityCooldown, 0);
            abilityButton?.SetCoolDown(abilityCooldown, 5);

            if (Patches.HudPatch.HudActive && GameManager.Instance != null) abilityButton?.Show();
            else abilityButton?.Hide();

            PlayerControl? closestTarget = PlayerManager.FindClosestTarget();

            if (hasOrb && abilityButton != null) abilityButton.buttonLabelText.text = "Place";
            else if (abilityButton != null) abilityButton.buttonLabelText.text = "Retrieve";

            if (hasOrb)
            {
                if (closestTarget != null)
                {
                    if (closestTarget != currentTarget) currentTarget?.ToggleHighlight(false, RoleTeamTypes.Crewmate);
                    currentTarget = closestTarget;
                    currentTarget.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>((Color)Color));
                    abilityButton?.SetEnabled();
                }
                else
                {
                    currentTarget?.ToggleHighlight(false, RoleTeamTypes.Crewmate);
                    currentTarget = null;
                    abilityButton?.SetDisabled();
                }
            }
            else
            {
                currentTarget?.ToggleHighlight(false, RoleTeamTypes.Crewmate);
                currentTarget = null;
                abilityButton?.SetEnabled();
            }
        }

        public void OnAbilityButtonPressed()
        {
            Logger<Plugin>.Info("Seer button pressed!");
            if (abilityCooldown > 0) return;
            if (hasOrb && currentTarget != null)
            {
                targetPlayer = currentTarget;
                RpcManager.SendSeerTarget(targetPlayer.PlayerId);
                hasOrb = false;
                abilityCooldown = 5;
            } 
            else
            {
                hasOrb = true;
                targetPlayer = null;
                RpcManager.SendSeerTarget(null);
            }
        }

        public bool hasOrb;
        public PlayerControl? targetPlayer;
        public PlayerControl? currentTarget;
        public float abilityCooldown;
        public AbilityButton? abilityButton;
    }
}
