using TTMC.Tools;

namespace PedestalServer
{
	public class Config
	{
		public ushort port = 34343;
		public bool logo = true;
		private string path;
		public Config(string path = "config.cfg")
		{
			this.path = path;
			if (!File.Exists(path))
			{
				CreateConfig();
				return;
			}
			LoadConfig();
		}
		private void CreateConfig()
		{
			string[] lines = { "# Pedestal Server Configuration File", $"# Created on: {DateTime.Now}", $"port = {port}", $"logo = {logo}" };
			File.WriteAllLines(path, lines);
		}
		private void LoadConfig()
		{
			foreach (string line in File.ReadLines(path))
			{
				string text = Crimp(line);
				if (!text.StartsWith('#') && text.Contains('='))
				{
					string[] temp = text.Split('=');
					switch (temp[0].ToLower())
					{
						case "port":
							ushort.TryParse(temp[1], out port);
							break;
						case "logo":
							bool.TryParse(temp[1], out logo);
							break;
					}
				}
			}
		}
		private string Crimp(string text)
		{
			string resp = string.Empty;
			bool god = false;
			foreach (char c in text)
			{
				if (god || (c != ' ' && c != '\t'))
				{
					resp += c;
					if (c == '\"')
					{
						god = !god;
					}
				}
			}
			return resp;
		}
	}
}