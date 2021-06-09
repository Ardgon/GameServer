using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Spell;

namespace Spells
{
    public class AkaliShadowDance : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            //TODO: Implement dash listeners
            _owner = owner;
            _spell = spell;
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
            AddBuff("AkaliTwilightShroudCD", 0.65f, 1, spell, owner, owner);
            RemoveBuff(owner, "AkaliTwilightShroud");
           
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            var target = spell.CastInfo.Targets[0].Unit;
            var current = new Vector2(owner.Position.X, owner.Position.Y);
            var to = Vector2.Normalize(new Vector2(target.Position.X, target.Position.Y) - current);
            var range = to * 800;

            var trueCoords = current + range;

            //TODO: Dash to the correct location (in front of the enemy IChampion) instead of far behind or inside them
            //ForceMovement(owner, target, "Spell4", 2200, 0, 0, 0, 20000);
            //ForceMovement(spell.CastInfo.Owner, "Spell4", trueCoords, 2200, 0, 0, 0);

            ForceMovement(owner, target, "Spell4", 2200, 0, 0, 0, 20000);

            AddParticleTarget(owner, target, "akali_shadowDance_tar.troy", target);
            ApplyEffects(owner, target, owner.GetSpell("AkaliShadowDance"), null);
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, ISpellMissile missile)
        {
            //var bonusAd = owner.Stats.AttackDamage.Total - owner.Stats.AttackDamage.BaseValue;
            var ap = owner.Stats.AbilityPower.Total * 0.5f;
            //var damage = 200 + spell.CastInfo.SpellLevel * 150 + bonusAd + ap;
            var damage = 100 + spell.CastInfo.SpellLevel * 75 + ap;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL,
                DamageSource.DAMAGE_SOURCE_SPELL, false);

            var MarkAPratio = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.5f;
            var MarkDamage = 45 + 25 * (owner.GetSpell("AkaliMota").CastInfo.SpellLevel - 1) + MarkAPratio;

            if (target.HasBuff("AkaliMota"))
            {
                target.TakeDamage(owner, MarkDamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_PROC, false);
                AddParticleTarget(owner, target, "akali_mark_impact_tar.troy", target, 1f);
                RemoveBuff(target, "AkaliMota");
                owner.Stats.CurrentMana += (15f + (5 * owner.GetSpell(0).CastInfo.SpellLevel));
            }
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

        public IObjAiBase _owner;
        public int prevKillCount = 0;
        public ISpell _spell;
        public void OnUpdate(float diff)
        {
            if (_owner.KillDeathCounter - prevKillCount > 0)
            {
                var spell = _spell as Spell;
                spell.AddCurrentAmmo(_owner.KillDeathCounter - prevKillCount);
            }

            prevKillCount = _owner.KillDeathCounter;
            
        }
    }
}
