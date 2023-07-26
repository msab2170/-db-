using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using Serilog;

namespace UpdateMailInfo
{
    class DBConnector
    {
        readonly string connectionString;
		
        public DBConnector(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /*
		 쿼리와 db의 컬럼명은 모두 지우거나 변경하였음.
		 */

        // 기록된 문서번호 뒤부터 select해오는 쿼리
        public List<MsgProperties> selectDBInfo(int currentDocNumber)
		{
			List<MsgProperties> msgFiles = new List<MsgProperties>();

			try
			{
				string[] MailApplicationArr = HandleConfiguration.transformConfigData("MailApplication");
				if (MailApplicationArr != null)
				{
					using (OleDbConnection connection = new OleDbConnection(connectionString))
					{
						connection.Open();
						string sql = @"쿼리는 모두 삭제 했다.";						
						OleDbCommand command = new OleDbCommand(sql, connection);
						command.Parameters.AddWithValue("@p1", currentDocNumber);
						Log.Debug($"DBConnector.selectDBInfo - sql string = {sql}，currentDocNumber＝{currentDocNumber}");
						OleDbDataReader reader = command.ExecuteReader();

						Log.Information("DBConnector.selectDBInfo - connection success");

						while (reader.Read())
						{
                            MsgProperties msgFile = new MsgProperties
                            {
                                DocNumber = (int)reader["a"],
                                FullPath = reader["b"].ToString(),
                                Originator = reader["c"].ToString(),
                                Addressee = reader["d"].ToString(),
                                CC = reader["e"].ToString(),
                                BCC = reader["f"].ToString(),
                                SentDate = (DateTime)reader["g"],
                                ReceivedDate = (DateTime)reader["h"],
                            };
                            msgFiles.Add(msgFile);

							Log.Information($"DBConnector.selectDBInfo - {msgFile}");
						}
						reader.Close();
					}
				} else
                {
					Log.Error("DBConnector.selectDBInfo - MailApplication in config.txt is incorrect");
				}
				return msgFiles;
			}
			catch (Exception ex)
			{
				Log.Error($"DBConnector.selectDBInfo - code [{ex.HResult}] Error Message : {ex.StackTrace}");
			}
			return msgFiles;
		}

		// 확인한 DocNumber
		public int countDBInfo(int currentDocNumber)
		{
			int count = -1;
			try
			{
				string[] MailApplicationArr = HandleConfiguration.transformConfigData("MailApplication");
				if (MailApplicationArr != null)
				{
					using (OleDbConnection connection = new OleDbConnection(connectionString))
					{
						connection.Open();

						string sql = @"쿼리는 모두 삭제하였다.";

						OleDbCommand command = new OleDbCommand(sql, connection);
						command.Parameters.AddWithValue("@p1", currentDocNumber);
						Log.Debug($"DBConnector.countDBInfo - sql string = {sql}，currentDocNumber＝{currentDocNumber}");

						OleDbDataReader reader = command.ExecuteReader();
						Log.Information("DBConnector.countDBInfo - connection success");

						while (reader.Read())
						{
							if (reader["MAX_DOCNUMBER"] != DBNull.Value)
							{
								count = (int)reader["a"];
								Log.Information($"DBConnector.countDBInfo - a = {count}");
							}
						}
						reader.Close();
					}
					return count;
				}
				else
                {
					throw new Exception("DBConnector.countDBInfo - MailApplication in config.txt is incorrect");
				}
			}
			catch (Exception ex)
			{
				Log.Error($"DBConnector.countDBInfo - code [{ex.HResult}] Error Message : {ex.StackTrace}");
			}
			return count;
		}

		// update 쿼리
		public int updateDBInfo(int targetDocNumber, MsgProperties properties)
        {
			int result = -1;
			try
			{
				using (OleDbConnection connection = new OleDbConnection(connectionString))
				{
					connection.Open();

					string sql = @"쿼리는 모두 삭제하였다.";

					int orignatorLength = Int32.Parse(HandleConfiguration.configData["CONFIG_FILE_PARAM1"]);
					int addresseeLength = Int32.Parse(HandleConfiguration.configData["CONFIG_FILE_PARAM2"]);
					int CCLength = Int32.Parse(HandleConfiguration.configData["CONFIG_FILE_PARAM3"]);
					int BCCLength = Int32.Parse(HandleConfiguration.configData["CONFIG_FILE_PARAM4"]);

					OleDbCommand command = new OleDbCommand(sql, connection);
					if (String.IsNullOrEmpty(properties.Originator))
					{
						command.Parameters.AddWithValue("@p1", DBNull.Value);
					}
					else
					{
						if (properties.Originator.Length > orignatorLength)
						{
							properties.Originator = properties.Originator.Substring(0, orignatorLength);
						}
						command.Parameters.AddWithValue("@p1", properties.Originator);
					}

					if (String.IsNullOrEmpty(properties.Addressee))
					{
						command.Parameters.AddWithValue("@p2", DBNull.Value);
					}
					else
					{
						if (properties.Addressee.Length > addresseeLength)
						{
							properties.Addressee = properties.Addressee.Substring(0, addresseeLength);
						}
						command.Parameters.AddWithValue("@p2", properties.Addressee);
					}

					if (String.IsNullOrEmpty(properties.CC))
					{
						command.Parameters.AddWithValue("@p3", DBNull.Value);
					}
					else
					{
						if (properties.CC.Length > CCLength)
						{
							properties.CC = properties.CC.Substring(0, CCLength);
						}
						command.Parameters.AddWithValue("@p3", properties.CC);
					}

					if (String.IsNullOrEmpty(properties.BCC))
					{
						command.Parameters.AddWithValue("@p4", DBNull.Value);
					}
					else
					{
						if (properties.BCC.Length > BCCLength)
						{
							properties.BCC = properties.BCC.Substring(0, BCCLength);
						}
						command.Parameters.AddWithValue("@p4", properties.BCC);
					}

					if (properties.SentDate == DateTime.MinValue)
					{
						command.Parameters.AddWithValue("@p5", DBNull.Value);
					}
					else
					{
						command.Parameters.AddWithValue("@p5", properties.SentDate.ToString("yyyy-MM-dd HH:mm:ss"));
					}
					if (properties.ReceivedDate == DateTime.MinValue)
					{
						command.Parameters.AddWithValue("@p6", DBNull.Value);
					}
					else
					{
						command.Parameters.AddWithValue("@p6",properties.ReceivedDate.ToString("yyyy-MM-dd HH:mm:ss"));
					}
					command.Parameters.AddWithValue("@p7", targetDocNumber);

					Log.Debug($"DBConnector.updateDBInfo - sql string = {sql}, " +
						$"\np1＝{properties.Originator}, p2＝{properties.Addressee}, p3＝{properties.CC}, p4＝{properties.BCC}, " +
						$"p5＝{properties.SentDate}, p6＝{properties.ReceivedDate}, p7＝{targetDocNumber}");
					result = command.ExecuteNonQuery();
					Log.Information($"DBConnector.updateDBInfo - update target = {properties}" +
						$", isSuccess = {result == 1}");
				}
			} catch (Exception ex)
            {
				Log.Error($"DBConnector.updateDBInfo - code[{ex.HResult}] Error Message : {ex.StackTrace}");
			}
			return result;
        }

		public List<MsgProperties> selectRangeDBInfo(string firstDocNumberString, string lastDocNumberString)
        {
			List<MsgProperties> msgFiles = new List<MsgProperties>();
			try
			{
				string[] MailApplicationArr = HandleConfiguration.transformConfigData("MailApplication");
				if (MailApplicationArr != null)
				{
					using (OleDbConnection connection = new OleDbConnection(connectionString))
					{
						connection.Open();
						int firstDocNumber = Int32.Parse(firstDocNumberString);
						int lastDocNumber = Int32.Parse(lastDocNumberString);

						string sql = @"쿼리는 모두 삭제하였다.";

						OleDbCommand command = new OleDbCommand(sql, connection);
						command.Parameters.AddWithValue("@p1", firstDocNumber);
						command.Parameters.AddWithValue("@p2", lastDocNumber);
						Log.Debug($"DBConnector.selectRangeDBInfo - sql string = {sql}, p1＝{firstDocNumber}, p2＝{lastDocNumber}");
						OleDbDataReader reader = command.ExecuteReader();

						Log.Information($"DBConnector.selectRangeDBInfo - connection success, range ({firstDocNumberString}, {lastDocNumberString})");

						while (reader.Read())
						{
                            MsgProperties msgFile = new MsgProperties
                            {
                                DocNumber = (int)reader["a"],
                                FullPath = reader["b"].ToString(),
                                Originator = reader["c"].ToString(),
                                Addressee = reader["d"].ToString(),
                                CC = reader["e"].ToString(),
                                BCC = reader["f"].ToString(),
                                SentDate = (DateTime)reader["g"],
                                ReceivedDate = (DateTime)reader["h"],
                                Version = (int)reader["i"]
                            };

                            msgFiles.Add(msgFile);

							Log.Information($"DBConnector.selectRangeDBInfo - {msgFile.ToString()}");
						}
						reader.Close();
					}
				}
				else
				{
					Log.Error("DBConnector.selectRangeDBInfo - MailApplication in config.txt is incorrect");
				}
				return msgFiles;
			}
			catch (Exception ex)
			{
				Log.Error($"DBConnector.selectRangeDBInfo - Error Message : {ex.Message}");
			}

			return msgFiles;
		}
	}
}