using System.Text;
using TTMC.Tools;

namespace TTMC.PMF
{
	public class Manifest
	{
		public string? path = null;
		public readonly string codesignature = "PEDM";
		public readonly uint codeversion = 1;
		public List<Content> register = new();
		private Stream? stream = null;
		public Manifest(string? path = null)
		{
			if (!string.IsNullOrEmpty(path))
			{
				this.path = path;
				stream = File.Open(path, FileMode.OpenOrCreate);
				Load();
			}
		}
		public Manifest(byte[] bytes)
		{
			stream = new MemoryStream(bytes);
			Load();
		}
		public void Load()
		{
			if (stream != null)
			{
				byte[] bytes = new byte[4];
				stream.Read(bytes, 0, 4);
				string signature = Encoding.UTF8.GetString(bytes);
				if (signature != codesignature)
				{
					throw new("Invalid file detected");
				}
				byte[] versionRaw = new byte[4];
				stream.Read(versionRaw, 0, 4);
				uint manifestVersion = BitConverter.ToUInt32(versionRaw);
				if (manifestVersion != codeversion)
				{
					throw new("Old version detected");
				}
				byte[] entriesRaw = new byte[4];
				stream.Read(entriesRaw, 0, 4);
				uint entries = BitConverter.ToUInt32(entriesRaw);
				while (stream.Length != stream.Position)
				{
					string path = GetString();
					byte[] hash = new byte[16];
					stream.Read(hash, 0, hash.Length);
					byte[] sizeRaw = new byte[4];
					stream.Read(sizeRaw, 0, sizeRaw.Length);
					uint size = BitConverter.ToUInt32(sizeRaw);
					Content content = new()
					{
						path = path,
						hash = hash,
						size = size
					};
					register.Add(content);
				}
			}
		}
		private string GetString()
		{
			if (stream == null)
			{
				throw new("Stream is null");
			}
			List<byte> list = new();
			while (true)
			{
				int x = stream.ReadByte();
				if (x == 0)
				{
					break;
				}
				list.Add((byte)x);
			}
			return Encoding.UTF8.GetString(list.ToArray());
		}
		public void Save(string path)
		{
			Stream fileStream = path == this.path && stream != null ? stream : File.Open(path, FileMode.OpenOrCreate);
			fileStream.Position = 0;
			fileStream.Write(Encoding.UTF8.GetBytes(codesignature));
			fileStream.Write(BitConverter.GetBytes(codeversion));
			fileStream.Write(BitConverter.GetBytes(register.Count));
			List<byte> bytes = new();
			foreach (Content content in register)
			{
				if (!string.IsNullOrEmpty(content.path) && content.hash != null && content.hash.Length == 16)
				{
					bytes.AddRange(Engine.Combine(Encoding.UTF8.GetBytes(content.path), new byte[1]));
					bytes.AddRange(content.hash);
					bytes.AddRange(BitConverter.GetBytes(content.size));
				}
			}
			fileStream.Write(bytes.ToArray());
			fileStream.SetLength(8 + bytes.Count);
			fileStream.Close();
			this.path = path;
			stream = File.Open(path, FileMode.OpenOrCreate);
		}
		public void Close()
		{
			if (stream != null)
			{
				stream.Close();
				stream.Dispose();
				stream = null;
			}
		}
	}
	public class Content
	{
		public string? path { get; set; }
		public byte[]? hash { get; set; }
		public uint size { get; set; }
	}
}