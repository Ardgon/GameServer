using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;

namespace LeagueSandbox.GameServer.GameObjects
{
    class MonsterCamp : IMonsterCamp
    {
        public MonsterCampType CampType { get; }

        public Vector2 Position { get; }

        public List<MonsterSpawnType> MonsterTypes { get; }
        public List<Vector2> MonsterSpawnPositions { get; }

        public float RespawnCooldown { get; }
        public float NextSpawnTime { get; protected set; } = 0f;

        private Game _game;
        private bool _notifiedClient;
        private bool _isAlive;
        private bool _setToSpawn;

        List<Monster> monsters = new List<Monster>();

        public MonsterCamp(Game game, MonsterCampType campType, Vector2 position, List<MonsterSpawnType> monsterTypes, List<Vector2> monsterSpawnPositions = null, float respawnCooldown = 1)
        {
            _game = game;
            CampType = campType;
            Position = position;
            MonsterTypes = monsterTypes;
            RespawnCooldown = respawnCooldown;
            MonsterSpawnPositions = monsterSpawnPositions;
        }

        private static string GetMonsterModel(MonsterSpawnType type)
        {
            var typeDictionary = new Dictionary<MonsterSpawnType, string>
            {
                {MonsterSpawnType.MINION_TYPE_BARON, "Worm"},
                {MonsterSpawnType.MINION_TYPE_DRAGON, "Dragon"},
                {MonsterSpawnType.MINION_TYPE_GROMP, "SRU_Gromp"},
                {MonsterSpawnType.MINION_TYPE_ANCIENT_GOLEM, "AncientGolem"},
                {MonsterSpawnType.MINION_TYPE_YOUNG_LIZARD_ANCIENT, "YoungLizard"},
                {MonsterSpawnType.MINION_TYPE_GIANT_WOLF, "GiantWolf"},
                {MonsterSpawnType.MINION_TYPE_WOLF, "Wolf"},
                {MonsterSpawnType.MINION_TYPE_WRAITH, "Wraith"},
                {MonsterSpawnType.MINION_TYPE_LESSER_WRAITH, "LesserWraith"},
                {MonsterSpawnType.MINION_TYPE_ELDER_LIZARD, "LizardElder"},
                {MonsterSpawnType.MINION_TYPE_YOUNG_LIZARD_ELDER, "YoungLizard"},
                {MonsterSpawnType.MINION_TYPE_GOLEM, "Golem"},
                {MonsterSpawnType.MINION_TYPE_LESSER_GOLEM, "SmallGolem"},
                //{MonsterSpawnType.MINION_TYPE_BARON, "SRU_Baron"},
                //{MonsterSpawnType.MINION_TYPE_DRAGON, "Dragon"},
                //{MonsterSpawnType.MINION_TYPE_GROMP, "SRU_Gromp"},
                //{MonsterSpawnType.MINION_TYPE_ANCIENT_GOLEM, "SRU_Blue"},
                //{MonsterSpawnType.MINION_TYPE_YOUNG_LIZARD_ANCIENT, "SRU_BlueMini"},
                //{MonsterSpawnType.MINION_TYPE_GIANT_WOLF, "SRU_Murkwolf"},
                //{MonsterSpawnType.MINION_TYPE_WOLF, "SRU_MurkwolfMini"},
                //{MonsterSpawnType.MINION_TYPE_WRAITH, "SRU_Razorbeak"},
                //{MonsterSpawnType.MINION_TYPE_LESSER_WRAITH, "SRU_RazorbeakMini"},
                //{MonsterSpawnType.MINION_TYPE_ELDER_LIZARD, "SRU_Red"},
                //{MonsterSpawnType.MINION_TYPE_YOUNG_LIZARD_ELDER, "SRU_RedMini"},
                //{MonsterSpawnType.MINION_TYPE_GOLEM, "SRU_Krug"},
                //{MonsterSpawnType.MINION_TYPE_LESSER_GOLEM, "SRU_KrugMini"},
            };

            if (!typeDictionary.ContainsKey(type))
            {
                return string.Empty;
            }

            return $"{typeDictionary[type]}";
        }

        private static string GetMinimapIcon(MonsterCampType type)
        {
            switch (type)
            {
                case MonsterCampType.BARON:
                    return "Baron";
                case MonsterCampType.DRAGON:
                    return "Camp";
                case MonsterCampType.BLUE_ANCIENT_GOLEM:
                    return "Camp";
                case MonsterCampType.RED_ANCIENT_GOLEM:
                    return "Camp";
                case MonsterCampType.BLUE_LIZARD_ELDER:
                    return "Camp";
                case MonsterCampType.RED_LIZARD_ELDER:
                    return "Camp";
                default:
                    return "LesserCamp";
            }
        }

        // TODO: method is used to evaluate whether camp should respawn as well as funcitoning as a getter for _isAlive,
        // should probably split that functionality into separate methods
        public bool IsAlive()
        {
            if (NextSpawnTime > _game.GameTime)
            {
                return false;
            }

            bool alive = false;
            foreach (var monster in monsters)
            {
                if (monster != null && !monster.IsDead)
                {
                    alive = true;
                }
            }

            _isAlive = alive;

            if (!_isAlive && !_setToSpawn)
            {
                NextSpawnTime = _game.GameTime + RespawnCooldown * 1000f;
                _game.PacketNotifier.NotifyMonsterCampEmpty(this, null);
                _setToSpawn = true;
            }

            return _isAlive;
        }

        public void Spawn()
        {
            if (!_notifiedClient)
            {
                _game.PacketNotifier.NotifyCreateMonsterCamp(Position, (byte)CampType, 0, GetMinimapIcon(CampType));
                _notifiedClient = true;
            }

            if (_isAlive) return;

            monsters = new List<Monster>();

            int i = 0;
            foreach (var type in MonsterTypes)
            {
                if (MonsterSpawnPositions != null)
                {
                    var m = new Monster(_game, MonsterSpawnPositions[i], Position, type, GetMonsterModel(type), GetMonsterModel(type), CampType);
                    monsters.Add(m);
                    _game.ObjectManager.AddObject(m);
                } else
                {
                    var m = new Monster(_game, Position, Position, type, GetMonsterModel(type), GetMonsterModel(type), CampType);
                    monsters.Add(m);
                    _game.ObjectManager.AddObject(m);
                }
                i++;
            }

            _setToSpawn = false;
            _isAlive = true;
        }
    }
}
