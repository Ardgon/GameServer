using System.Linq;
using GameServerCore;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class KatarinaR : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {

            NotSingleTargetSpell = true,
            IsDamagingSpell = true,
            TriggersSpellCasts = true,
            ChannelDuration = 2.5f,
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellSectorHit.AddListener(this, new KeyValuePair<ISpell, IObjAiBase>(spell, owner), TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {           

            
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            //AddParticleTarget(owner, owner, "Katarina_deathLotus_cas.troy", null);
            PlayAnimation(owner, "KatarinaR");
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var AP = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.25f;
            var AD = spell.CastInfo.Owner.Stats.AttackDamage.TotalBonus * 0.6f;
            float damage = 5f + spell.CastInfo.SpellLevel * 35f + AP + AD;
            var MarkAP = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.15f;
            float MarkDamage = 15f * (owner.GetSpell("KatarinaR").CastInfo.SpellLevel) + MarkAP;

            if (target.HasBuff("KatarinaQMark"))
            {
                target.TakeDamage(owner, MarkDamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_PROC, false);
                RemoveBuff(target, "KatarinaQMark");
            }
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
            AddParticleTarget(owner, target, "Katarina_deathLotus_cas.troy", target, 1f);
            //AddParticleTarget(owner, target, "katarina_deathlotus_success.troy", target, 1f);
        }


        public ISpellSector sector;
        public void OnSpellChannel(ISpell spell)
        {
            if (sector != null)
            {
                sector.SetToRemove();
                sector = null;
            } 

            sector = spell.CreateSpellSector(new SectorParameters
            {
                HalfLength = 550f,
                Tickrate = 10,
                Type = SectorType.Area,
                CanHitSameTargetConsecutively = true,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes
            });

            var owner = spell.CastInfo.Owner;
            owner.StopMovement();
            if (owner is IObjAiBase ai)
            {
                ai.SetTargetUnit(null, true);
            }

            owner.Stats.SetActionState(ActionState.CAN_ATTACK, false);
            owner.Stats.SetActionState(ActionState.CAN_CAST, false);
            owner.Stats.SetActionState(ActionState.CAN_MOVE, false);
            owner.Stats.SetActionState(ActionState.CAN_NOT_ATTACK, true);
            owner.Stats.SetActionState(ActionState.CAN_NOT_MOVE, true);
        }

        public void OnSpellChannelCancel(ISpell spell)
        {
            sector.SetToRemove();
            sector = null;
            var owner = spell.CastInfo.Owner;
            //StopAnimation(owner, "KatarinaR", true, false, true);
            owner.Stats.SetActionState(ActionState.CAN_ATTACK, true);
            owner.Stats.SetActionState(ActionState.CAN_CAST, true);
            owner.Stats.SetActionState(ActionState.CAN_MOVE, true);
            owner.Stats.SetActionState(ActionState.CAN_NOT_ATTACK, false);
            owner.Stats.SetActionState(ActionState.CAN_NOT_MOVE, false);
        }

        public void OnSpellPostChannel(ISpell spell)
        {
            sector.SetToRemove();
            sector = null;

            var owner = spell.CastInfo.Owner;
            owner.Stats.SetActionState(ActionState.CAN_ATTACK, true);
            owner.Stats.SetActionState(ActionState.CAN_CAST, true);
            owner.Stats.SetActionState(ActionState.CAN_MOVE, true);
            owner.Stats.SetActionState(ActionState.CAN_NOT_ATTACK, false);
            owner.Stats.SetActionState(ActionState.CAN_NOT_MOVE, false);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
