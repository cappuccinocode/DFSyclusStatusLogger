using MySql.Data.MySqlClient;
using System;
using System.Data;
using DFSyclusStatusLogger.Helper;

namespace DFSyclusStatusLogger.StatusLoggers
{
    class StatusLoggerMySql : IStatusLogger
    {
        public string GetStatus(string filename)
        {
            string sqlConnString = DBHelper.GetConnectionString();
            MySqlConnection sqlConn = new MySqlConnection(sqlConnString);
            string sqlQuery = "SELECT Status FROM DFStatus WHERE SyclusID='{0}' LIMIT 1;";
            sqlQuery = string.Format(sqlQuery.ToString(), filename);
            string status = "";

            try
            {
                sqlConn.Open();

                MySqlCommand sqlComm = new MySqlCommand(sqlQuery, sqlConn);
                MySqlDataAdapter sqlAdapter = new MySqlDataAdapter();
                sqlAdapter.SelectCommand = sqlComm;
                DataTable dt = new DataTable();
                sqlAdapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    status = dt.Rows[0]["Status"].ToString();
                }
            }
            catch (Exception ex)
            {
                EventLogger.DebugEntry("Error occured while running GetStatus", System.Diagnostics.EventLogEntryType.Error, ex);
            }
            finally
            {
                if (sqlConn != null)
                {
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }

            return status;
        }

        public DataTable GetNewSyclusIdList(DateTime datetime)
        {
            string sqlConnString = DBHelper.GetConnectionString();
            MySqlConnection sqlConn = new MySqlConnection(sqlConnString);
            string sqlQuery = "SELECT SyclusID,CreateDate FROM DFStatus WHERE Status='{0}' AND DATE(CreateDate)='{1}' ORDER BY CreateDate ASC LIMIT 10;";
            sqlQuery = string.Format(sqlQuery.ToString(), StatusLoggerConstants.NEW, datetime.ToString("yyyy-MM-dd"));
            DataTable dt = new DataTable();

            try
            {
                sqlConn.Open();

                MySqlCommand sqlComm = new MySqlCommand(sqlQuery, sqlConn);
                MySqlDataAdapter sqlAdapter = new MySqlDataAdapter();
                sqlAdapter.SelectCommand = sqlComm;
                sqlAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                EventLogger.DebugEntry("Error occured while running GetNewSyclusIdList", System.Diagnostics.EventLogEntryType.Error, ex);
            }
            finally
            {
                if (sqlConn != null)
                {
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }

            return dt;
        }

        public void SetCreateStatus(string filename)
        {
            string sqlConnString = DBHelper.GetConnectionString();
            MySqlConnection sqlConn = new MySqlConnection(sqlConnString);
            string sqlQuery = "INSERT INTO DFStatus(SyclusID,Status,CreateDate) values('{0}','{1}',{2});";
            sqlQuery = string.Format(sqlQuery.ToString(), filename, StatusLoggerConstants.NEW, "NOW()");

            try
            {
                sqlConn.Open();

                MySqlCommand sqlComm = new MySqlCommand(sqlQuery, sqlConn);
                //MySqlDataReader sqlReader = sqlComm.ExecuteReader();
                //while (sqlReader.Read())
                //{
                //}
                int result = sqlComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                EventLogger.DebugEntry("Error occured while running SetCreateStatus", System.Diagnostics.EventLogEntryType.Error, ex);
            }
            finally
            {
                if (sqlConn != null)
                {
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }
        }

        public void SetFailedStatus(string filename)
        {
            string sqlConnString = DBHelper.GetConnectionString();
            MySqlConnection sqlConn = new MySqlConnection(sqlConnString);
            string sqlQuery = "UPDATE DFStatus SET Status='{0}',UpdateDate={1} WHERE SyclusID='{2}';";
            sqlQuery = string.Format(sqlQuery.ToString(), StatusLoggerConstants.FAILED, "NOW()", filename);

            try
            {
                sqlConn.Open();

                MySqlCommand sqlComm = new MySqlCommand(sqlQuery, sqlConn);
                //MySqlDataReader sqlReader = sqlComm.ExecuteReader();
                //while (sqlReader.Read())
                //{
                //}
                int result = sqlComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                EventLogger.DebugEntry("Error occured while running SetFailedStatus", System.Diagnostics.EventLogEntryType.Error, ex);
            }
            finally
            {
                if (sqlConn != null)
                {
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }
        }

        public void SetSuccessStatus(string filename)
        {
            string sqlConnString = DBHelper.GetConnectionString();
            MySqlConnection sqlConn = new MySqlConnection(sqlConnString);
            string sqlQuery = "UPDATE DFStatus SET Status='{0}',UpdateDate={1} WHERE SyclusID='{2}';";
            sqlQuery = string.Format(sqlQuery.ToString(), StatusLoggerConstants.SUCCESS, "NOW()", filename);

            try
            {
                sqlConn.Open();

                MySqlCommand sqlComm = new MySqlCommand(sqlQuery, sqlConn);
                //MySqlDataReader sqlReader = sqlComm.ExecuteReader();
                //while (sqlReader.Read())
                //{
                //}
                int result = sqlComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                EventLogger.DebugEntry("Error occured while running SetSuccessStatus", System.Diagnostics.EventLogEntryType.Error, ex);
            }
            finally
            {
                if (sqlConn != null)
                {
                    if (sqlConn.State == System.Data.ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }
        }
    }
}
