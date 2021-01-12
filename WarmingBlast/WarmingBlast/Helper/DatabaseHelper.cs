using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace WarmingBlast.Helper
{
    public class DatabaseHelper
    {

        ConnectionHelper obj_connection = new ConnectionHelper();
        private string ConnectionString;
        private SqlConnection objConnection = new SqlConnection();
        private SqlCommand objCommand;
        private SqlDataAdapter objAdapter;
        private SqlDataReader objReader;
        private DataTable dtMaster = new DataTable();
        private DataSet ds = new DataSet();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHelper"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public DatabaseHelper(string connectionString)
        {
            ConnectionString = connectionString;
        }
        ~DatabaseHelper()
        {
            objConnection = null;
            objCommand = null;
            objAdapter = null;
            objReader = null;
            dtMaster = null;
        }

        /// <summary>
        /// returning data in data-table without passing any parameter.
        /// </summary>
        /// <param name="spProcedureName">Name of the stored procedure.</param>
        /// <returns></returns>
        public DataTable DataTable(string spProcedureName)
        {

            try
            {
                dtMaster = null;
                DataSet ds = new DataSet();
                objConnection = obj_connection.Open(ConnectionString);
                objCommand = new SqlCommand();
                objCommand.Connection = objConnection;
                objCommand.CommandText = spProcedureName;
                objCommand.CommandType = CommandType.StoredProcedure;
                DebugCommand(objCommand);
                objAdapter = new SqlDataAdapter(objCommand);
                objAdapter.Fill(ds, "a");
                dtMaster = ds.Tables["a"];
                ds.Dispose();
            }
            finally
            {
                obj_connection.Close(objConnection);
            }

            return dtMaster;
        }


        /// <summary>
        /// returning data in data-table after passing parameter....
        /// </summary>
        /// <param name="strProcedureName">Name of the string procedure.</param>
        /// <param name="pParameter">The p parameter.</param>
        /// <returns></returns>
        public DataTable DataTable(string strProcedureName, SqlParameter[] pParameter)
        {
            try
            {
                dtMaster = null;
                DataSet ds = new DataSet();
                objConnection = obj_connection.Open(ConnectionString);
                objCommand = new SqlCommand();
                objCommand.Connection = objConnection;
                objCommand.CommandText = strProcedureName;

                objCommand.CommandType = CommandType.StoredProcedure;

                if (pParameter != null)
                {
                    foreach (SqlParameter p in pParameter)
                    {
                        objCommand.Parameters.Add(p);
                    }
                }

                DebugCommand(objCommand);
                objAdapter = new SqlDataAdapter(objCommand);
                objAdapter.Fill(ds, "a");
                dtMaster = ds.Tables["a"];
                ds.Dispose();
            }
            finally
            {
                obj_connection.Close(objConnection);
            }
            return dtMaster;
        }


        /// <summary>
        /// this method is only use to export data record.
        /// </summary>
        /// <param name="spProcedureName">Name of the stored procedure.</param>
        /// <param name="pParameter">The parameters array to execute particular stored procedure.</param>
        /// <returns></returns>
        public DataTable DataTableForExportData(string spProcedureName, SqlParameter[] pParameter)
        {
            try
            {
                dtMaster = new DataTable();

                objConnection = obj_connection.Open(ConnectionString);
                objCommand = new SqlCommand();
                objCommand.Connection = objConnection;
                objCommand.CommandTimeout = 120;
                objCommand.CommandText = spProcedureName;

                objCommand.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in pParameter)
                {
                    objCommand.Parameters.Add(p);
                }

                SqlDataReader reader = objCommand.ExecuteReader();
                if (reader.HasRows)
                    dtMaster.Load(reader);
            }
            finally
            {
                obj_connection.Close(objConnection);
            }

            return dtMaster;
        }

        /// <summary>
        /// reading data through data-reader with parameter that returning data-reader object.
        /// </summary>
        /// <param name="spProcedureName">Name of the stored procedure.</param>
        /// <returns></returns>
        private SqlDataReader DataReaderData(string spProcedureName)
        {
            try
            {
                objConnection = obj_connection.Open(ConnectionString);
                objCommand = new SqlCommand();
                objCommand.Connection = objConnection;
                objCommand.CommandText = spProcedureName;
                objCommand.CommandType = CommandType.StoredProcedure;
                DebugCommand(objCommand);
                objReader = objCommand.ExecuteReader();

            }
            finally
            {
                obj_connection.Close(objConnection);
            }
            return objReader;
        }

        /// <summary>
        /// reading data through data-reader with parameter that returning data-reader object.
        /// </summary>
        /// <param name="spProcedureName">Name of the stored procedure.</param>
        /// <param name="pParameter">The p parameter.</param>
        /// <returns></returns>
        private SqlDataReader DataReader(string spProcedureName, SqlParameter[] pParameter)
        {
            try
            {
                objConnection = obj_connection.Open(ConnectionString);
                objCommand = new SqlCommand();
                objCommand.Connection = objConnection;
                objCommand.CommandText = spProcedureName;

                objCommand.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in pParameter)
                {
                    objCommand.Parameters.Add(p);
                }
                DebugCommand(objCommand);
                objReader = objCommand.ExecuteReader();

            }
            finally
            {
                obj_connection.Close(objConnection);
            }
            return objReader;
        }

        /// <summary>
        /// Gets the dataset data.
        /// </summary>
        /// <param name="spProcedureName">Name of the stored procedure.</param>
        /// <param name="pParameter">The p parameter.</param>
        /// <returns></returns>
        public DataSet GetDatasetData(string spProcedureName, SqlParameter[] pParameter)
        {
            try
            {
                dtMaster = null;
                ds = new DataSet();
                objConnection = obj_connection.Open(ConnectionString);
                objCommand = new SqlCommand();
                objCommand.CommandText = spProcedureName;
                objCommand.Connection = objConnection;
                objCommand.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in pParameter)
                {
                    objCommand.Parameters.Add(p);
                }

                if (objConnection.State == ConnectionState.Closed)
                    objConnection.Open();

                objAdapter = new SqlDataAdapter(objCommand);
                objAdapter.Fill(ds);
            }
            finally
            {
                obj_connection.Close(objConnection);
            }

            return ds;
        }

        /// <summary>
        /// return scalar value from database.
        /// </summary>
        /// <param name="spProcedureName">Name of the stored procedure.</param>
        /// <param name="pParameter">The p parameter.</param>
        /// <returns></returns>
        public string ExecuteScalar(string spProcedureName, SqlParameter[] pParameter)
        {
            string strValue = "";
            try
            {
                objConnection = obj_connection.Open(ConnectionString);

                objCommand = new SqlCommand();
                objCommand.Connection = objConnection;
                objCommand.CommandText = spProcedureName;

                objCommand.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in pParameter)
                {
                    objCommand.Parameters.Add(p);
                }

                DebugCommand(objCommand);
                strValue = Convert.ToString(objCommand.ExecuteScalar());
            }
            finally
            {
                obj_connection.Close(objConnection);
            }
            return strValue;

        }

        /// <summary>
        /// Update the database.
        /// </summary>
        /// <param name="spStoredProcedure">The sp stored procedure.</param>
        /// <param name="pParameter">The p parameter.</param>
        /// <returns></returns>
        public bool ExecuteNonQuery(string spStoredProcedure, SqlParameter[] pParameter)
        {
            bool boolSuccess = false;
            try
            {
                objConnection = obj_connection.Open(ConnectionString);
                objCommand = new SqlCommand();
                objCommand.Connection = objConnection;
                objCommand.CommandText = spStoredProcedure;

                objCommand.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in pParameter)
                {
                    objCommand.Parameters.Add(p);
                }
                DebugCommand(objCommand);
                int intReturnValue = objCommand.ExecuteNonQuery();

                boolSuccess = intReturnValue > 0;
            }
            finally
            {
                obj_connection.Close(objConnection);
            }
            return boolSuccess;

        }

        /// <summary>
        /// reading data through data-reader with parameter that returning ArrayList.
        /// </summary>
        /// <param name="ProcedureName">Name of the procedure.</param>
        /// <returns></returns>
        public ArrayList DataReader(string ProcedureName)
        {
            ArrayList arrData = new ArrayList();
            try
            {
                SqlDataReader reader = DataReaderData(ProcedureName);

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        arrData.Add(reader[i].ToString());
                    }
                }
                reader.Close();
            }
            finally
            {
                obj_connection.Close(objConnection);
            }
            return arrData;
        }

        /// <summary>
        /// Executes the non query string.
        /// </summary>
        /// <param name="spStoredProcedure">The sp stored procedure.</param>
        /// <param name="pParameter">The p parameter.</param>
        /// <returns></returns>
        public string ExecuteNonQueryString(string spStoredProcedure, SqlParameter[] pParameter)
        {
            string strRetrun = string.Empty;
            try
            {
                objConnection = obj_connection.Open(ConnectionString);
                objCommand = new SqlCommand();
                objCommand.Connection = objConnection;
                objCommand.CommandText = spStoredProcedure;
                objCommand.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in pParameter)
                {
                    objCommand.Parameters.Add(p);
                }
                DebugCommand(objCommand);
                int intReturnValue = objCommand.ExecuteNonQuery();
                strRetrun = pParameter[1].Value.ToString();
            }
            finally
            {
                obj_connection.Close(objConnection);
            }
            return strRetrun;

        }

        /// <summary>
        /// Debugs the command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        [Conditional("DEBUG")]
        private void DebugCommand(SqlCommand cmd)
        {
            Debug.WriteLine(cmd.CommandText);
        }

        /// <summary>
        /// This function Execute Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DataTable ExecuteQuery(string query)
        {
            try
            {
                dtMaster = null;
                DataSet ds = new DataSet();
                objConnection = obj_connection.Open(ConnectionString);
                objCommand = new SqlCommand();
                objCommand.Connection = objConnection;
                objCommand.CommandText = query;
                objCommand.CommandType = CommandType.Text;
                DebugCommand(objCommand);
                objAdapter = new SqlDataAdapter(objCommand);
                objAdapter.Fill(ds, "a");
                dtMaster = ds.Tables["a"];
                ds.Dispose();
            }
            finally
            {
                obj_connection.Close(objConnection);
            }

            return dtMaster;
        }
        /// <summary>
        /// this function Execute Update of given query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool ExecuteUpdate(string query)
        {
            bool boolSuccess = false;
            try
            {
                objConnection = obj_connection.Open(ConnectionString);
                objCommand = new SqlCommand();
                objCommand.Connection = objConnection;
                objCommand.CommandText = query;

                objCommand.CommandType = CommandType.Text;

                //foreach (SqlParameter p in pParameter)
                //{
                //    objCommand.Parameters.Add(p);
                //}
                //DebugCommand(objCommand);
                int intReturnValue = objCommand.ExecuteNonQuery();

                boolSuccess = intReturnValue > 0;
            }
            finally
            {
                obj_connection.Close(objConnection);
            }
            return boolSuccess;
        }

        /// <summary>
        /// Gets the dataset data without parameter.
        /// </summary>
        /// <param name="spProcedureName">Name of the stored procedure.</param>
        /// <returns></returns>
        public DataSet GetDatasetData(string spProcedureName)
        {
            try
            {
                dtMaster = null;
                ds = new DataSet();
                objConnection = obj_connection.Open(ConnectionString);
                objCommand = new SqlCommand();
                objCommand.CommandText = spProcedureName;
                objCommand.Connection = objConnection;
                objCommand.CommandType = CommandType.StoredProcedure;

                if (objConnection.State == ConnectionState.Closed)
                    objConnection.Open();

                objAdapter = new SqlDataAdapter(objCommand);
                objAdapter.Fill(ds);
            }
            finally
            {
                obj_connection.Close(objConnection);
            }

            return ds;
        }
    }
}
