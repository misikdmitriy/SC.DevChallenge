using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SC.DevChallenge.Core.Services.Contracts;
using SC.DevChallenge.Db.Contexts;
using SC.DevChallenge.Db.Factories.Contracts;
using SC.DevChallenge.Db.Models;

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
			// 1. Compare hash of last file parsed with DB hash
			// In case if hashes equal - do nothing
			// else - cleanup DB
			var md5 = GetFileMd5(filepath);

			using (var context = _factory.CreateContext())
			{
				if (context.ContentHistories.Any(x => x.Hash == md5))
				{
					_logger.LogInformation("Skip DB update");
					return;
				}

				_logger.LogInformation("Cleanup all DB tables");
				CleanupDb(context);
			}

			// 2. Parse CSV
			_logger.LogInformation("Start CSV parsing");
			using (var context = _factory.CreateContext())
			{
				var conn = context.Database.GetDbConnection();

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

			// 3. Save file hash
			_logger.LogInformation("Update content history");
			using (var context = _factory.CreateContext())
			{
				var history = new ContentHistory
				{
					Hash = md5,
					LastUpdate = DateTime.Now
				};

				context.ContentHistories.Add(history);

				context.SaveChanges();
			}
		}

		private void CleanupDb(AppDbContext context)
		{
			context.ContentHistories.RemoveRange(context.ContentHistories);
			context.InstrumentOwners.RemoveRange(context.InstrumentOwners);
			context.Instruments.RemoveRange(context.Instruments);
			context.Portfolios.RemoveRange(context.Portfolios);
			context.PriceModels.RemoveRange(context.PriceModels);

			context.SaveChanges();
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

				// ignore first line
				file.ReadLine();

				while (!file.EndOfStream)
				{
					var line = file.ReadLine();
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

		private byte[] GetFileMd5(string filepath)
		{
			using (var file = File.OpenRead(filepath))
			using (var md5 = MD5.Create())
			{
				return md5.ComputeHash(file);
			}
		}
	}
}