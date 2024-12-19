using TTMC.Pedestal;
using TTMC.Auram;
using TTMC.Tools;

namespace PedestalClient
{
	internal class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Debug.Print("Pedestal v0.1 (TTMC Corporation)\n", ConsoleColor.Cyan);
				Debug.Print("Usage:\n\tlistapps (List all available apps on the server)\n\tget <id> (Download app from server)", ConsoleColor.Gray);
			}
			else
			{
				if (args[0] == "listapps")
				{
					Pedestal pedestal = new();
					List<Guid> guids = pedestal.GetApps();
					Debug.Info("Apps found: " + guids.Count);
					foreach (Guid guid in guids)
					{
						Database metadata = new();
						metadata.Import(pedestal.GetMetadata(guid));
						Console.WriteLine($"{metadata.Get<string>("title")} ({guid})");
					}
					pedestal.Disconnect();
				}
				else if (args.Length >= 2)
				{
					if (args[0] == "info")
					{
						Pedestal pedestal = new();
						Guid guid = new(args[1]);
						Database metadata = new();
						metadata.Import(pedestal.GetMetadata(guid));
						Debug.Print("Application info of ", ConsoleColor.Gray, false);
						Debug.Print(guid.ToString(), ConsoleColor.Red);
						foreach (string key in metadata.keys)
						{
							Debug.Print(key.ToUpper() + ": ", ConsoleColor.DarkGray, false);
							Debug.Info(metadata.Get<string>(key));
						}
						pedestal.Disconnect();
					}
				}
				else
				{
					Debug.Error("Unknown command");
				}
			}
		}
	}
}