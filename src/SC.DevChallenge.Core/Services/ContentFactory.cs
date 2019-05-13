using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SC.DevChallenge.Core.Services.Contracts;
using SC.DevChallenge.Db.Contexts;
using SC.DevChallenge.Db.Factories.Contracts;

namespace SC.DevChallenge.Core.Services
{
	public class ContentFactory : IContentFactory
	{
		private readonly IDbContextFactory<AppDbContext> _factory;
		private readonly ILogger<ContentFactory> _logger;

		public ContentFactory(IDbContextFactory<AppDbContext> factory,
			ILogger<ContentFactory> logger)
		{
			_factory = factory;
			_logger = logger;
		}

		public bool IsUpdateRequired()
		{
			using (var context = _factory.CreateContext())
			{
				return !context.PriceModels.Any();
			}
		}

		public void ParseContentFromCsv(string filepath)
		{
			_logger.LogInformation("Start CSV parsing");

			using (var context = _factory.CreateContext())
			using (var conn = context.Database.GetDbConnection())
			{
				if (conn.State != ConnectionState.Open)
				{
					conn.Open();
				}

				var fullPath = new FileInfo(filepath).FullName;

				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SC.DevChallenge.Core.Scripts.script.sql"))
				using (var reader = new StreamReader(stream))
				using (var cmd = conn.CreateCommand())
				{
					// 'bulk insert' not work for linux
					// know issue
					// https://stackoverflow.com/questions/41393887/sql-server-on-linux-bulk-import-error
					cmd.CommandText = reader.ReadToEnd().Replace("@path", fullPath);

					cmd.ExecuteNonQuery();
				}
			}

			_logger.LogInformation("CSV parsing successfully finished");
		}
	}
}