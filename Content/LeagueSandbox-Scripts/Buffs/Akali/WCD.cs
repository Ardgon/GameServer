using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;

namespace AkaliTwilightShroudCD
{
    class AkaliTwilightShroudCD : IBuffGameScript
    {
        public BuffType BuffType => BuffType.SUPPRESSION;

        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;

        public bool IsHidden => false;

        public int MaxStacks => 1;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }
       
        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }
        public void OnUpdate(float diff)
        {
        }
    }
}