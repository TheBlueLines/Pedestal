using TTMC.Debris;
using System.Net.Sockets;

namespace TTMC.Pedestal
{
	internal class Handler : Handle
	{
		public override Packet? Message(Packet packet, NetworkStream stream)
		{
			Pedestal.packets[packet.id] = packet;
			return null;
		}
	}
}