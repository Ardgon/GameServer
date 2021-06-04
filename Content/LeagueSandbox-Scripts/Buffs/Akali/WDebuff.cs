using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;

namespace AkaliTwilightShroudDebuff
{
    class AkaliTwilightShroudDebuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_DEHANCER;

        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;

        public bool IsHidden => false;

        public int MaxStacks => 1;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {

            StatsModifier.MoveSpeed.PercentBaseBonus += (0.14f + 0.04f * ownerSpell.CastInfo.SpellLevel) * -1;

            unit.AddStatModifier(StatsModifier);

        }
       
        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }
        public void OnUpdate(float diff)
        {
        }
    }
}