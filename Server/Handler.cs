using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using TTMC.Debris;
using TTMC.Tools;

namespace PedestalServer
{
	internal class Handler : Handle
	{
		private Dictionary<Guid, Application> applications = new();
		private MD5 md5 = MD5.Create();
		private Application App(Guid app)
		{
			if (!applications.ContainsKey(app))
			{
				applications[app] = new(app);
			}
			return applications[app];
		}
		public override Packet? Message(Packet packet, NetworkStream stream)
		{
			switch (packet.id)
			{
				// Get Apps
				case 1:
					List<Guid> apps = [];
					foreach (string value in Directory.GetDirectories("Manifests"))
					{
						Guid guid = new(Path.GetFileName(value));
						apps.Add(guid);
					}
					return new Packet() { id = packet.id, data = Engine.Serialize(apps.ToArray()) };
				// Download Manifest
				case 2:
					Guid app = new(packet.data[..16]);
					Application application = App(app);
					byte[] data = application.GetManifest((packet.data.Length == 16) ? null : Encoding.UTF8.GetString(packet.data[16..]));
					return new Packet() { id = packet.id, data = data };
				// Download Content
				case 3:
					byte[] hash0 = packet.data;
					string hex = Convert.ToHexString(hash0);
					string path = $"Content\\{hex[..2]}\\{hex}";
					return new Packet() { id = packet.id, data = File.Exists(path) ? File.ReadAllBytes(path) : [] };
				// Manifest MD5 Hash
				case 4:
					Application app0 = App(new(packet.data));
					byte[] hash1 = md5.ComputeHash(app0.GetManifest((packet.data.Length == 16) ? null : Encoding.UTF8.GetString(packet.data[16..])));
					return new Packet() { id = packet.id, data = hash1 };
				// Download Metadata
				case 5:
					Application app1 = App(new(packet.data));
					return new Packet() { id = packet.id, data = app1.metadata };
				// Get App Versions
				case 6:
					Application app2 = App(new(packet.data));
					return new Packet() { id = packet.id, data = Engine.Serialize(app2.versions) };
			}
			return null;
		}
	}
}