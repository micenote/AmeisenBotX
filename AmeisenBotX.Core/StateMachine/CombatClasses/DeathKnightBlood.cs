﻿using AmeisenBotX.Core.Character;
using AmeisenBotX.Core.Character.Comparators;
using AmeisenBotX.Core.Data;
using AmeisenBotX.Core.Data.Enums;
using AmeisenBotX.Core.Data.Objects.WowObject;
using AmeisenBotX.Core.Hook;
using AmeisenBotX.Core.Statemachine.Enums;
using AmeisenBotX.Pathfinding.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Statemachine.CombatClasses
{
    public class DeathknightBlood : ICombatClass
    {
        public DeathknightBlood(IObjectManager objectManager, ICharacterManager characterManager, IHookManager hookManager)
        {
            ObjectManager = objectManager;
            CharacterManager = characterManager;
            HookManager = hookManager;
        }

        public string Author => "Jamsbaer";

        public WowClass Class => WowClass.Deathknight;

        public Dictionary<string, dynamic> Configureables { get; set; } = new Dictionary<string, dynamic>();

        public string Description => "FCFS based CombatClass for the Blood Deathknight spec.";

        public string Displayname => "[WIP] Blood Deathknight";

        public bool HandlesMovement => false;

        public bool HandlesTargetSelection => false;

        public bool IsMelee => true;

        public IWowItemComparator ItemComparator => null;

        public CombatClassRole Role => CombatClassRole.Tank;

        public string Version => "1.0";

        private ICharacterManager CharacterManager { get; }

        private DateTime HeroicStrikeLastUsed { get; set; }

        private IHookManager HookManager { get; }

        private Vector3 LastPosition { get; set; }

        private IObjectManager ObjectManager { get; }

        public void Execute()
        {
            ulong targetGuid = ObjectManager.TargetGuid;
            WowUnit target = ObjectManager.WowObjects.OfType<WowUnit>().FirstOrDefault(t => t.Guid == targetGuid);
            if (target != null)
            {
                // make sure we're auto attacking
                if (!ObjectManager.Player.IsAutoAttacking)
                {
                    HookManager.StartAutoAttack();
                }

                HandleAttacking(target);
            }
        }

        public void OutOfCombatExecute()
        {
        }

        private void HandleAttacking(WowUnit target)
        {
            double playerRunePower = ObjectManager.Player.Runeenergy;
            double distanceToTarget = ObjectManager.Player.Position.GetDistance(target.Position);
            double targetHealthPercent = (target.Health / (double)target.MaxHealth) * 100;
            double playerHealthPercent = (ObjectManager.Player.Health / (double)ObjectManager.Player.MaxHealth) * 100.0;
            (string, int) targetCastingInfo = HookManager.GetUnitCastingInfo(WowLuaUnit.Target);
            //List<string> myBuffs = HookManager.GetBuffs(WowLuaUnit.Player.ToString());
            //myBuffs.Any(e => e.Equals("Chains of Ice"))

            if (HookManager.GetSpellCooldown("Death Grip") <= 0 && distanceToTarget <= 30)
            {
                HookManager.CastSpell("Death Grip");
            }
            if (target.IsFleeing && distanceToTarget <= 30)
            {
                HookManager.CastSpell("Chains of Ice");
            }

            if (HookManager.GetSpellCooldown("Army of the Dead") <= 0 &&
                IsOneOfAllRunesReady())
            {
                HookManager.CastSpell("Army of the Dead");
            }

            List<WowUnit> unitsNearPlayer = ObjectManager.WowObjects
                .OfType<WowUnit>()
                .Where(e => e.Position.GetDistance(ObjectManager.Player.Position) <= 10)
                .ToList();

            if (unitsNearPlayer.Count > 2 &&
                HookManager.GetSpellCooldown("Blood Boil") <= 0 &&
                HookManager.IsRuneReady(0) ||
                HookManager.IsRuneReady(1))
            {
                HookManager.CastSpell("Blood Boil");
            }

            List<WowUnit> unitsNearTarget = ObjectManager.WowObjects
                .OfType<WowUnit>()
                .Where(e => e.Position.GetDistance(target.Position) <= 30)
                .ToList();

            if (unitsNearTarget.Count > 2 &&
                HookManager.GetSpellCooldown("Death and Decay") <= 0 &&
                IsOneOfAllRunesReady())
            {
                HookManager.CastSpell("Death and Decay");
                HookManager.ClickOnTerrain(target.Position);
            }

            if (HookManager.GetSpellCooldown("Icy Touch") <= 0 &&
                HookManager.IsRuneReady(2) ||
                HookManager.IsRuneReady(3))
            {
                HookManager.CastSpell("Icy Touch");
            }
        }

        private bool IsOneOfAllRunesReady()
            => HookManager.IsRuneReady(0)
            || HookManager.IsRuneReady(1)
            && HookManager.IsRuneReady(2)
            || HookManager.IsRuneReady(3)
            && HookManager.IsRuneReady(4)
            || HookManager.IsRuneReady(5);
    }
}