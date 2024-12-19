using TTMC.Debris;
using TTMC.Tools;

namespace PedestalServer
{
	public class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "Pedestal Server";
			Directory.CreateDirectory("Manifests");
			Directory.CreateDirectory("Content");
			Config config = new();
			Handler handler = new();
			Server server = new(config.port, handler);
			if (config.logo)
			{
				Debug.Error("\r\nooooooooo.                   .o8                         .             oooo  \r\n`888   `Y88.                \"888                       .o8             `888  \r\n 888   .d88'  .ooooo.   .oooo888   .ooooo.   .oooo.o .o888oo  .oooo.    888  \r\n 888ooo88P'  d88' `88b d88' `888  d88' `88b d88(  \"8   888   `P  )88b   888  \r\n 888         888ooo888 888   888  888ooo888 `\"Y88b.    888    .oP\"888   888  \r\n 888         888    .o 888   888  888    .o o.  )88b   888 . d8(  888   888  \r\no888o        `Y8bod8P' `Y8bod88P\" `Y8bod8P' 8\"\"888P'   \"888\" `Y888\"\"8o o888o \r\n");
			}
			Debug.InsertDate();
			Debug.Info($"Pedestal server started on port {config.port}!");
		}
	}
}