using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using System.Collections.Generic;

namespace Spells
{
    public class VeigarEventHorizon : ISpellScript
    {
        Vector2 truecoords;
        float timeSinceCast;
        IObjAiBase Owner;
        ISpell Spell;
        List<uint> stunnedUnits;
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,

        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            Owner = owner;
            Spell = spell;
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {

        }

        public void OnSpellCast(ISpell spell)
        {
            stunnedUnits = new List<uint>();
            var owner = spell.CastInfo.Owner as IChampion;
            var ownerSkinID = owner.Skin;
            truecoords = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var distance = Vector2.Distance(spell.CastInfo.Owner.Position, truecoords);
            if (distance > 650f)
            {
                truecoords = GetPointFromUnit(spell.CastInfo.Owner, 650f);
            }

            string cage = "";
            switch (ownerSkinID)
            {
                case 8:
                    cage = "Veigar_Skin08_E_cage_green.troy";
                    break;
                case 6:
                    cage = "Veigar_Skin06_E_cage_green.troy";
                    break;
                case 4:
                    cage = "Veigar_Skin04_E_cage_green.troy";
                    break;
                default:
                    cage = "Veigar_Base_E_cage_green.troy";
                    break;
            }
            AddParticle(owner, null, cage, truecoords, lifetime: 3f);
            timeSinceCast = 0;

            //TODO: Stun Hitbox & Buff
        }

        public void OnSpellPostCast(ISpell spell)
        {
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
            if (truecoords.X == 0)
                return;

            timeSinceCast += diff;
            if (timeSinceCast <= 3000)
            {
                var units = GetUnitsInRange(truecoords, 400f, true);

                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].NetId == Owner.NetId)
                        continue;

                    if (Vector2.Distance(units[i].Position, truecoords) >= 300f && Vector2.Distance(units[i].Position, truecoords) <= 370f)
                    {
                        //units[i].ApplyCrowdControl(stun, Owner);
                        if (!stunnedUnits.Contains(units[i].NetId))
                        {
                            stunnedUnits.Add(units[i].NetId);
                            float duration = 1.5f + (0.25f * Spell.CastInfo.SpellLevel);
                            AddBuff("VeigarEventHorizon", duration, 1, Spell, units[i], Owner);
                        }
                    }
                }
            }
            else
            {
                truecoords = new Vector2(0,0);
            }
        }
    }
}