using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;

namespace AkaliTwilightShroud
{
    class AkaliTwilightShroud : IBuffGameScript
    {
        public BuffType BuffType => BuffType.INVISIBILITY;

        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;

        public bool IsHidden => false;

        public int MaxStacks => 1;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            StatsModifier.Armor.FlatBonus += 10 * ownerSpell.CastInfo.SpellLevel;
            StatsModifier.MagicResist.FlatBonus += 10 * ownerSpell.CastInfo.SpellLevel;

            unit.AddStatModifier(StatsModifier);

            var champ = ownerSpell.CastInfo.Owner as Champion;
            champ.StealthEnter();
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var champ = ownerSpell.CastInfo.Owner as Champion;
            champ.StealthExit();
        }
        public void OnUpdate(float diff)
        {

        }

    }
}
