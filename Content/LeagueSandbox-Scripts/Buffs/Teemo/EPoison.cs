using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace ToxicShot

{
    internal class ToxicShot : IBuffGameScript
    {
        public BuffType BuffType => BuffType.POISON;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IObjAiBase owner;
        IAttackableUnit Unit;
        IParticle p;
        float damage;
        float timeSinceLastTick = 500f;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner;
            var e = owner.GetSpell(2);
            Unit = unit;
            float APratio = owner.Stats.AbilityPower.Total * 0.1f;
            damage = 6 * e.CastInfo.SpellLevel + APratio;

            p = AddParticleTarget(owner, unit, "Global_Poison.troy", unit, buff.Duration, bone: "BUFFBONE_GLB_CHANNEL_LOC");
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);
        }

        public void OnUpdate(float diff)
        {
            timeSinceLastTick += diff;
            if (timeSinceLastTick >= 1000f && !Unit.IsDead && Unit != null)
            {
                Unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_PERIODIC, false);
                timeSinceLastTick = 0;
            }
        }
    }
}

