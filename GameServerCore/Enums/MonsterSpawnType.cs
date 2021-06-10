namespace GameServerCore.Enums
{
    public enum MonsterSpawnType : byte
    {
        MINION_TYPE_BARON = 0x00,
        MINION_TYPE_ANCIENT_GOLEM = 0x01,
        MINION_TYPE_YOUNG_LIZARD_ANCIENT = 0x02,
        MINION_TYPE_ELDER_LIZARD = 0x03,
        MINION_TYPE_YOUNG_LIZARD_ELDER = 0x04,
        MINION_TYPE_GOLEM = 0x05,
        MINION_TYPE_LESSER_GOLEM = 0x06,
        MINION_TYPE_GIANT_WOLF = 0x07,
        MINION_TYPE_WOLF = 0x08,
        MINION_TYPE_GROMP = 0x09,
        MINION_TYPE_DRAGON = 0x0A,
        MINION_TYPE_WRAITH = 0x0B,
        MINION_TYPE_LESSER_WRAITH = 0x0C,
    }

    public enum MonsterCampType : byte
    {
        BLUE_GOLEMS = 0x00,
        BLUE_ANCIENT_GOLEM = 0x01,
        BLUE_WRAITHS = 0x02,
        BLUE_WOLVES = 0x03,
        BLUE_LIZARD_ELDER = 0x04,
        BLUE_GROMP = 0x05,
        RED_GOLEMS = 0x06,
        RED_ANCIENT_GOLEM = 0x07,
        RED_WRAITHS = 0x08,
        RED_WOLVES = 0x09,
        RED_LIZARD_ELDER = 0x0A,
        RED_GROMP = 0x0B,
        DRAGON = 0x0C,
        BARON = 0x0D,
    }
}