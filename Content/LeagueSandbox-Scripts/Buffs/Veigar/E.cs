using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;


namespace VeigarEventHorizon 
{
    internal class VeigarEventHorizon : IBuffGameScript
    {
        public BuffType BuffType => BuffType.STUN;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; }

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            unit.StopMovement();
            if(unit is IObjAiBase ai)
            {
                ai.SetTargetUnit(null, true);
            }

            unit.Stats.SetActionState(ActionState.CAN_ATTACK, false);
            unit.Stats.SetActionState(ActionState.CAN_CAST, false);
            unit.Stats.SetActionState(ActionState.CAN_MOVE, false);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            unit.Stats.SetActionState(ActionState.CAN_ATTACK, true);
            unit.Stats.SetActionState(ActionState.CAN_CAST, true);
            unit.Stats.SetActionState(ActionState.CAN_MOVE, true);
            if (unit is IMonster ai && buff.SourceUnit is IChampion ch)
            {
                ai.SetTargetUnit(ch, true);
            }
        }

        public void OnUpdate(float diff)
        {

        }
    }
}

