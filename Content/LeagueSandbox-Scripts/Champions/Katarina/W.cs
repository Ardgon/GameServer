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
    public class KatarinaW : ISpellScript //Fix this shit not working at all
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {

            NotSingleTargetSpell = true,
            IsDamagingSpell = true,
            TriggersSpellCasts = true

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
            owner.StopChanneling(ChannelingStopCondition.Cancel, ChannelingStopSource.Move);
            spell.CreateSpellSector(new SectorParameters
            {
                HalfLength = 400f,
                SingleTick = true,
                Type = SectorType.Area,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes
            });
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            AddParticleTarget(owner, owner, "katarina_w_cas.troy", owner, bone: "C_BUFFBONE_GLB_CHEST_LOC");
            PlayAnimation(owner, "KatarinaW", 0.3f);
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
            float MarkDamage = 15f * (owner.GetSpell("KatarinaQ").CastInfo.SpellLevel) + MarkAP;

            if (target.HasBuff("KatarinaQMark"))
            {
                target.TakeDamage(owner, MarkDamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_PROC, false);
                RemoveBuff(target, "KatarinaQMark");
            }
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
            AddParticleTarget(owner, target, "katarina_w_tar.troy", target, 1f);
            AddBuff("KatarinaWHaste", 1f, 1, spell, owner, owner);
        }


        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
