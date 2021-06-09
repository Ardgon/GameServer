using GameServerCore.Enums;
using GameServerCore.Packets.Enums;

namespace PacketDefinitions420.PacketDefinitions.S2C
{
    public class AmmoUpdate : BasePacket
    {
        public AmmoUpdate(TeamId team, int hp)
            : base(PacketCmd.PKT_S2C_AMMO_UPDATE)
        {
            Write((uint)team);
            Write(hp);
        }
    }
}