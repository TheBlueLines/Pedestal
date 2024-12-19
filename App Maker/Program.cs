using System.IO.Compression;
using System.Security.Cryptography;
using TTMC.Auram;
using TTMC.PMF;

namespace PedestalAppMaker
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Guid appID = Guid.NewGuid();
			//int x = 0;
			string[] fileNames = Directory.GetFiles("C:\\Users\\David\\Downloads\\First");
			foreach (string fileName in fileNames)
			{
				Manifest manifest = new();
				MD5 md5 = MD5.Create();
				FileStream fileStream = File.OpenRead(fileName);
				ZipArchive zipArchive = new(fileStream);
				foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries.Where(x => !x.FullName.EndsWith('/')))
				{
					Stream stream = zipArchiveEntry.Open();
					byte[] hash = md5.ComputeHash(stream);
					Content content = new()
					{
						path = zipArchiveEntry.FullName.Replace('/', '\\'),
						hash = hash,
						size = (uint)zipArchiveEntry.Length
					};
					manifest.register.Add(content);
					string hex = Convert.ToHexString(hash);
					Directory.CreateDirectory($"Content\\{hex[..2]}");
					FileStream opened = File.OpenWrite($"Content\\{hex[..2]}\\{hex}");
					stream.CopyTo(opened);
					stream.Close();
					stream.Dispose();
					opened.Close();
					opened.Dispose();
				}
				Directory.CreateDirectory($"Manifests\\{appID}");
				manifest.Save($"Manifests\\{appID}\\{Path.GetFileNameWithoutExtension(fileName)}.pmf");
				manifest.Close();
				Console.WriteLine(Path.GetFileNameWithoutExtension(fileName) + " created!");
				fileStream.Close();
				fileStream.Dispose();
			}
			Database metadata = new();
			metadata.Set("author", "TTMC Corporation");
			metadata.Set("description", "A very good arcade game made by TTMC! You are a cube and you need to complete levels without hitting any obstacle! There are a few game modes like Endless, Campaign and Level Editor! Map sharing is coming soon!");
			metadata.Set("title", "First");
			metadata.Set("release", Path.GetFileNameWithoutExtension(fileNames.Last()));
			metadata.Save($"Manifests\\{appID}\\Metadata.auram");
			metadata.Close();
		}
	}
}