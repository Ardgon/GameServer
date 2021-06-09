using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;
using System;

namespace RedBuff

{
    internal class RedBuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; }

        IParticle p;
        IParticle p2;
        IObjAiBase _owner;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            _owner = ownerSpell.CastInfo.Owner;

            p = AddParticleTarget(_owner, unit, "NeutralMonster_buf_red_offense_big.troy", unit, buff.Duration);
            p2 = AddParticleTarget(_owner, unit, "SRU_JungleBuff_Redbuff_Health.troy", unit);

            ApiEventManager.OnHitUnit.AddListener(_owner, _owner, OnHit, false);
        }

        private void OnHit(IAttackableUnit unit, bool isCrit)
        {
            float damage = 8 + (2 * _owner.Stats.Level);
            damage = damage * 2;
            unit.TakeDamage(_owner, damage, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_SPELL, false);

            // TODO: Implement Slow
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);
            RemoveParticle(p2);
        }

        public void OnUpdate(float diff)
        {

        }
    }
}

