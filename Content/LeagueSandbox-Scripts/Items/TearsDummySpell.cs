using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using System;
using LeagueSandbox.GameServer.API;

namespace Spells
{
    public class TearsDummySpell : ISpellScript
    {
        float _previousMana;
        IObjAiBase _owner;
        float _manaGrantDelay;
        int _totalManaGranted;
        float _timesGranted;
        float _stackGrantDelay;

        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            _owner = owner;
            ApiEventManager.OnSpellCast.AddListener(owner, spell, TargetExecute);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            //AddBuff("RegenerationPotion", 15.0f, 1, spell, owner, owner);
        }

        private void TargetExecute(ISpell spell)
        {
            spell.CastInfo.Owner.Stats.ManaPoints.FlatBonus += 4f;
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
            // TODO: Remove item without selling
            //var champion = spell.CastInfo.Owner as Champion;
            //champion.Shop.HandleItemSellRequest(Convert.ToByte(spell.CastInfo.SpellSlot - 6));
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
            if (_totalManaGranted >= 750)
            {
                return;
            }

            // Give max mana on mana expenditure

            _stackGrantDelay += diff;

            if (_timesGranted == 2) 
            {
                if (_stackGrantDelay >= 8 * 1000f)
                {
                    _timesGranted = 0;
                    _stackGrantDelay = 0f;

                }
            } else
            {
                _stackGrantDelay = 0f;
            }

            var mana = _owner.Stats.CurrentMana;

            if (_timesGranted < 2)
            {
                if (mana < _previousMana)
                {
                    _owner.Stats.ManaPoints.FlatBonus += 4f;
                    _totalManaGranted += 4;
                    _timesGranted++;
                }                
            }

            _previousMana = mana;

            // Give mana over time
            if (_manaGrantDelay >= 8 * 1000f)
            {
                _owner.Stats.ManaPoints.FlatBonus += 1f;
                _manaGrantDelay = 0f;
                _totalManaGranted += 1;
            }

            _manaGrantDelay += diff;

        }
    }
}
