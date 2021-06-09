using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;
using System;

namespace BlueBuff

{
    internal class BlueBuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;
        IObjAiBase _owner;

        public IStatsModifier StatsModifier { get; private set; }

        float _regenModifier = 0f;
        IParticle p;
        IParticle p2;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            _owner = ownerSpell.CastInfo.Owner;

            p = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "NeutralMonster_buf_blue_defense_big.troy", unit, buff.Duration);
            p2 = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "SRU_JungleBuff_Bluebuff_Mana.troy", unit);

            //StatsModifier.MoveSpeed.PercentBonus += 0.05f + (0.1f * ownerSpell.CastInfo.SpellLevel);
            //StatsModifier.ManaRegeneration.FlatBonus += 25f;

            float manaBonus = 0;
            if (ownerSpell.CastInfo.Owner.Stats.ManaPoints.Total < 0)
            {
                manaBonus = ownerSpell.CastInfo.Owner.Stats.ManaPoints.Total * 0.005f;
            }
            
            float totalRegenModifier = 5f + manaBonus;
            _regenModifier = totalRegenModifier;

            ownerSpell.CastInfo.Owner.Stats.CooldownReduction.FlatBonus += 0.1f;
            ownerSpell.CastInfo.Owner.Stats.ManaRegeneration.FlatBonus += totalRegenModifier;
            //unit.AddStatModifier(StatsModifier);

        }


        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            ownerSpell.CastInfo.Owner.Stats.CooldownReduction.FlatBonus -= 0.1f;
            ownerSpell.CastInfo.Owner.Stats.ManaRegeneration.FlatBonus = _regenModifier;
            RemoveParticle(p);
            RemoveParticle(p2);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}

