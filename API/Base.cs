using System.Text;
using TTMC.Debris;
using TTMC.PMF;
using TTMC.Tools;

namespace TTMC.Pedestal
{
	public class Pedestal
	{
		internal Client client;
		public static Dictionary<int, Packet> packets = new();
		public Pedestal(string server = "127.0.0.1", ushort port = 34343)
		{
			Handler handler = new Handler();
			client = new(server, port) { handle = handler };
		}
		public void Disconnect()
		{
			client.Disconnect();
		}
		public List<Guid> GetApps()
		{
			Packet resp = GetPacket(new Packet() { id = 1, data = [] });
			return Engine.Deserialize<Guid[]>(resp.data).ToList();
		}
		public Manifest GetManifest(Guid app, string? version = null)
		{
			Packet resp = GetPacket(new Packet() { id = 2, data = Engine.Combine(app.ToByteArray(), version == null ? [] : Encoding.UTF8.GetBytes(version)) });
			return new(resp.data);
		}
		public byte[] DownloadFile(byte[] hash)
		{
			Packet resp = GetPacket(new Packet() { id = 3, data = hash });
			return resp.data;
		}
		public byte[] GetManifestHash(Guid app)
		{
			Packet resp = GetPacket(new Packet() { id = 4, data = app.ToByteArray() });
			return resp.data;
		}
		public byte[] GetMetadata(Guid app)
		{
			Packet resp = GetPacket(new Packet() { id = 5, data = app.ToByteArray() });
			if (resp.data.Length == 0)
			{
				throw new("Unknown app");
			}
			return resp.data;
		}
		public string[] GetAllVersions(Guid app)
		{
			Packet resp = GetPacket(new Packet() { id = 6, data = app.ToByteArray() });
			if (resp.data.Length == 0)
			{
				throw new("Unknown app");
			}
			return Engine.Deserialize<string[]>(resp.data);
		}
		internal Packet GetPacket(Packet packet)
		{
			client.SendPacket(packet);
			while (!packets.ContainsKey(packet.id)) 
			{
				Thread.Sleep(10);
			}
			Packet resp = packets[packet.id];
			packets.Remove(packet.id);
			return resp;
		}
	}
	public class Application
	{
		public string? title { get; set; }
		public string? description { get; set; }
		public string? author { get; set; }
		public string? version { get; set; }
	}
}