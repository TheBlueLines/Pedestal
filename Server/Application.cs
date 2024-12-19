namespace PedestalServer
{
	public class Application
	{
		private Guid guid;
		public Application(Guid guid)
		{
			this.guid = guid;
		}
		public string[] versions
		{
			get
			{
				if (Directory.Exists($"Manifests\\{guid}"))
				{
					return Directory.GetFiles($"Manifests\\{guid}").Where(x => Path.GetExtension(x).ToLower() == ".pmf").Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
				}
				return [];
			}
		}
		public byte[] GetManifest(string? version = null)
		{
			if (Directory.Exists($"Manifests\\{guid}"))
			{
				if (version == null)
				{
					string? file = Directory.GetFiles($"Manifests\\{guid}").Where(x => Path.GetExtension(x).ToLower() == ".pmf").LastOrDefault();
					if (file != default)
					{
						return File.ReadAllBytes(file);
					}
				}
				else
				{
					string path = $"Manifests\\{guid}\\{version}.pmf";
					if (File.Exists(path))
					{
						return File.ReadAllBytes(path);
					}
				}
			}
			return [];
		}
		public byte[] metadata
		{
			get
			{
				string path1 = $"Manifests\\{guid}\\Metadata.auram";
				if (File.Exists(path1))
				{
					return File.ReadAllBytes(path1);
				}
				return [];
			}
		}
	}
}