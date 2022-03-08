using System;
using System.Data;
using System.Data.SqlClient;
using DFSyclusStatusLogger.Helper;

namespace DFSyclusStatusLogger.StatusLoggers
{
    class StatusLoggerSqlServerExpresss : IStatusLogger
    {
        public string GetStatus(string filename)
        {
            string sqlConnString = DBHelper.GetConnectionString();
            SqlConnection sqlConn = new SqlConnection(sqlConnString);
            string sqlQuery = "SELECT TOP 1 Status FROM DFStatus WHERE SyclusID='{0}';";
            sqlQuery = string.Format(sqlQuery.ToString(), filename);
            string status = "";

            try
            {
                sqlConn.Open();

                SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter();
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
            SqlConnection sqlConn = new SqlConnection(sqlConnString);
            string sqlQuery = "SELECT TOP 10 SyclusID,,CreateDate FROM DFStatus WHERE Status='{0}' AND CreateDate='{1}' ORDER BY CreateDate ASC;";
            sqlQuery = string.Format(sqlQuery.ToString(), StatusLoggerConstants.NEW, datetime.ToString("yyyy-MM-dd"));
            DataTable dt = new DataTable();

            try
            {
                sqlConn.Open();

                SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter();
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
            SqlConnection sqlConn = new SqlConnection(sqlConnString);
            string sqlQuery = "INSERT INTO DFStatus(SyclusID,Status,CreateDate) values('{0}','{1}',{2});";
            sqlQuery = string.Format(sqlQuery.ToString(), filename, StatusLoggerConstants.NEW, "GETDATE()");

            try
            {
                sqlConn.Open();

                SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
                //SqlDataReader sqlReader = sqlComm.ExecuteReader();
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
            SqlConnection sqlConn = new SqlConnection(sqlConnString);
            string sqlQuery = "UPDATE DFStatus SET Status='{0}',UpdateDate={1} WHERE SyclusID='{2}' ;";
            sqlQuery = string.Format(sqlQuery.ToString(), StatusLoggerConstants.FAILED, "GETDATE()", filename);

            try
            {
                sqlConn.Open();

                SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
                //SqlDataReader sqlReader = sqlComm.ExecuteReader();
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
            SqlConnection sqlConn = new SqlConnection(sqlConnString);
            string sqlQuery = "UPDATE DFStatus SET Status='{0}',UpdateDate={1} WHERE SyclusID='{2}';";
            sqlQuery = string.Format(sqlQuery.ToString(), StatusLoggerConstants.SUCCESS, "GETDATE()", filename);

            try
            {
                sqlConn.Open();

                SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
                //SqlDataReader sqlReader = sqlComm.ExecuteReader();
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
