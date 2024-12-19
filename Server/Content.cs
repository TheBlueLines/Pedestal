using TTMC.PMF;

namespace PedestalServer
{
	internal class Content
	{
		public Dictionary<string, Manifest> manifests = new();
		public Content()
		{
			foreach (string text in Directory.GetFiles("Manifests"))
			{
				Manifest manifest = new(text);
				manifests.Add(Path.GetFileNameWithoutExtension(text), manifest);
			}
		}
	}
}