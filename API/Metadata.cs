using TTMC.Auram;

namespace TTMC.Pedestal
{
	public class Metadata
	{
		public Metadata(byte[] metadata)
		{
			Database database = new();
			database.Import(metadata);
			author = database.Get<string>("author");
			description = database.Get<string>("description");
			title = database.Get<string>("title");
			release = database.Get<string>("release");
		}
		public string author { get; set; }
		public string description { get; set; }
		public string title { get; set; }
		public string release { get; set; }
	}
}