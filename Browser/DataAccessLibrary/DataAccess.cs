using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using DataAccessLibrary.Classes;
using Windows.UI.Xaml.Media.Imaging;

namespace DataAccessLibrary
{
    public static class DataAccess
    {
		private static readonly string _dbPath = "webbrowser.db";
		private static readonly string _tableSearchTerms = "searchterms";
		public static readonly string TableHistory = "historyitems";
		public static readonly string TableBookmarks = "bookmarksitems";


		public static async void InitializeDatabase()
		{
			await ApplicationData.Current.LocalFolder.CreateFileAsync(_dbPath, CreationCollisionOption.OpenIfExists);
			string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, _dbPath);

			using (SqliteConnection connection = new SqliteConnection($"Filename={dbPath}"))
			{
				connection.Open();

				String searchTermsTableCommand = "CREATE TABLE IF NOT " +
												 $"EXISTS {_tableSearchTerms} (searchtermID INTEGER PRIMARY KEY," +
												 "SearchTerm VARCHAR(2048) NOT NULL," +
												 "DateSearched DATE)";


				String historyItemsTableCommand = "CREATE TABLE IF NOT " +
												$"EXISTS {TableHistory} (itemID INTEGER PRIMARY KEY," +
												"ItemTitle VARCHAR(2048) NOT NULL, " +
												"ItemUrl VARCHAR(2048) NOT NULL, " +
												"ItemDate DATE)";

				String bookmarksItemsTableCommand = "CREATE TABLE IF NOT " +
												$"EXISTS {TableBookmarks} (itemID INTEGER PRIMARY KEY," +
												"ItemTitle VARCHAR(2048) NOT NULL, " +
												"ItemUrl VARCHAR(2048) NOT NULL, " +
												"ItemDate DATE)";

				SqliteCommand createTableSearchTerms = new SqliteCommand(searchTermsTableCommand, connection);
				SqliteCommand createTableHistoryItems = new SqliteCommand(historyItemsTableCommand, connection);
				SqliteCommand createTableBookmarksItems = new SqliteCommand(bookmarksItemsTableCommand, connection);

				createTableSearchTerms.ExecuteReader();
				createTableHistoryItems.ExecuteReader();
				createTableBookmarksItems.ExecuteReader();
			}
		}


		public static void AddSearchTermToTable(string searchTerm, DateTime dateSearched)
		{
			string dp = Path.Combine(ApplicationData.Current.LocalFolder.Path, _dbPath);

			using (SqliteConnection connection = new SqliteConnection($"Filename={dp}"))
			{
				connection.Open();

				SqliteCommand insertCommand = new SqliteCommand();
				insertCommand.Connection = connection;

				insertCommand.CommandText = $"INSERT INTO {_tableSearchTerms} VALUES(NULL, @SearchTerm, @DateSearched)";
				insertCommand.Parameters.AddWithValue("@SearchTerm", searchTerm);
				insertCommand.Parameters.AddWithValue("@DateSearched", dateSearched);

				insertCommand.ExecuteReader();
			}
		}


		public static void DeleteSearchTermFromTable(string searchTerm)
		{
			string dp = Path.Combine(ApplicationData.Current.LocalFolder.Path, _dbPath);

			using (SqliteConnection connection = new SqliteConnection($"Filename={dp}"))
			{
				connection.Open();

				SqliteCommand insertCommand = new SqliteCommand();
				insertCommand.Connection = connection;

				insertCommand.CommandText = $"DELETE FROM {_tableSearchTerms} where SearchTerm='{searchTerm}'";

				insertCommand.ExecuteReader();
			}
		}


		public static List<string> GetAllSearhcedTerms()
		{
			List<string> terms = new List<string>();

			string dp = Path.Combine(ApplicationData.Current.LocalFolder.Path, _dbPath);

			using (SqliteConnection connection = new SqliteConnection($"Filename={dp}"))
			{
				connection.Open();

				SqliteCommand selectTermsCommand = new SqliteCommand($"SELECT SearchTerm FROM {_tableSearchTerms}", connection);

				SqliteDataReader reader = selectTermsCommand.ExecuteReader();

				while (reader.Read())
					terms.Add(reader.GetString(0));
			}

			return terms;
		}


		public static void AddItemToTable(string table, string ItemTitle, string ItemUrl, DateTime ItemDate)
		{
			string dp = Path.Combine(ApplicationData.Current.LocalFolder.Path, _dbPath);

			using (SqliteConnection connection = new SqliteConnection($"Filename={dp}"))
			{
				connection.Open();

				SqliteCommand insertCommand = new SqliteCommand();
				insertCommand.Connection = connection;

				insertCommand.CommandText = $"INSERT INTO {table} VALUES(NULL, @ItemTitle, @ItemUrl, @ItemDate)";
				insertCommand.Parameters.AddWithValue("@ItemTitle", ItemTitle);
				insertCommand.Parameters.AddWithValue("@ItemUrl", ItemUrl);
				insertCommand.Parameters.AddWithValue("@ItemDate", ItemDate);

				insertCommand.ExecuteReader();
			}
		}


		public static void DeleteItemFromTable(string table, string ItemUrl)
		{
			string dp = Path.Combine(ApplicationData.Current.LocalFolder.Path, _dbPath);

			using (SqliteConnection connection = new SqliteConnection($"Filename={dp}"))
			{
				connection.Open();

				SqliteCommand insertCommand = new SqliteCommand();
				insertCommand.Connection = connection;

				insertCommand.CommandText = $"DELETE FROM {table} where ItemUrl='{ItemUrl}'";

				insertCommand.ExecuteReader();
			}
		}


		public static void DeleteAllRecordsFromTable(string table)
		{
			string dp = Path.Combine(ApplicationData.Current.LocalFolder.Path, _dbPath);

			using (SqliteConnection connection = new SqliteConnection($"Filename={dp}"))
			{
				connection.Open();

				SqliteCommand insertCommand = new SqliteCommand();
				insertCommand.Connection = connection;

				insertCommand.CommandText = $"DELETE FROM {table}";

				insertCommand.ExecuteReader();
			}
		}


		public static List<ItemDetails> GetAllItems(string table)
		{
			List<ItemDetails> ItemsDetailsList = new List<ItemDetails>();

			string dp = Path.Combine(ApplicationData.Current.LocalFolder.Path, _dbPath);

			using (SqliteConnection connection = new SqliteConnection($"Filename={dp}"))
			{
				connection.Open();

				using (SqliteCommand selectTermsCommand = new SqliteCommand($"SELECT * FROM {table}", connection))
				{
					SqliteDataReader reader = selectTermsCommand.ExecuteReader();

					while (reader.Read())
					{
						ItemDetails item = new ItemDetails
						{
							Title		= reader.GetString(1),
							Url		    = reader.GetString(2),
							ImageSource = new BitmapImage (new Uri
														  (
															  "https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=" +
															  reader.GetString(2) +
															  "&size=50"
														  )),
							Date = Convert.ToDateTime(reader.GetString(3))
						};
						
						ItemsDetailsList.Add(item);
					}
				}
			}

			ItemsDetailsList.Reverse();
			return ItemsDetailsList;
		}
	}
}
