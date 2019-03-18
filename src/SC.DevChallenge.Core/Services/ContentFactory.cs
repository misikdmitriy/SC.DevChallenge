using System.Data;
using System.Data.Common;
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

		public void ParseContentFromCsv(string filepath)
		{
			// 1. If any data exists in DB - skip CSV parsing
			using (var context = _factory.CreateContext())
			{
				if (context.PriceModels.Any())
				{
					_logger.LogInformation("Skip DB update");
					return;
				}
			}

			// 2. Parse CSV
			_logger.LogInformation("Start CSV parsing");
			using (var context = _factory.CreateContext())
			using (var conn = context.Database.GetDbConnection())
			{
				if (conn.State != ConnectionState.Open)
				{
					conn.Open();
				}

				using (var transaction = conn.BeginTransaction())
				{
					ParseCsv(filepath, conn, transaction);

					transaction.Commit();
				}
			}
			_logger.LogInformation("CSV parsing successfully finished");
		}

		private void ParseCsv(string inputPath, DbConnection conn, DbTransaction transaction)
		{
			var command = string.Empty;

			using (var stream = Assembly.GetExecutingAssembly()
				.GetManifestResourceStream("SC.DevChallenge.Core.Scripts.script.sql"))
			using (var reader = new StreamReader(stream))
			{
				command = reader.ReadToEnd();
			}

			using (var file = new StreamReader(inputPath))
			{
				if (file.EndOfStream)
				{
					return;
				}

				file.ReadLine();

				if (file.EndOfStream)
				{
					return;
				}
			}

			var lines = File.ReadLines(inputPath);

			foreach (var line in lines.Skip(1))
			{
				var separated = line.Split(',');

				using (var cmd = conn.CreateCommand())
				{
					cmd.Transaction = transaction;
					cmd.CommandText = command
						.Replace("'portfolio'", $"'{separated[0]}'")
						.Replace("'owner'", $"'{separated[1]}'")
						.Replace("'instrument'", $"'{separated[2]}'")
						.Replace("'date'", $"'{separated[3]}'")
						.Replace("'price'", separated[4]);

					cmd.ExecuteNonQuery();
				}
			}
		}
	}
}