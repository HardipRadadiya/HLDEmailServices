using System.Data.SqlClient;

namespace WarmingBlast.Helper
{
    public class ConnectionHelper
    {

        private SqlConnection objSqlConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionHelper"/> class.
        /// </summary>
        public ConnectionHelper()
        {
            objSqlConnection = new SqlConnection();
        }
        ~ConnectionHelper()
        {
            if (objSqlConnection != null && objSqlConnection.State != System.Data.ConnectionState.Closed)
            {
                objSqlConnection.Close();
            }
        }

        /// <summary>
        /// Opens the specified connection string.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        /// <returns></returns>
        public SqlConnection Open(string ConnectionString)
        {
            objSqlConnection.ConnectionString = ConnectionString;
            objSqlConnection.Open();
            return objSqlConnection;
        }
        /// <summary>
        /// Closes the specified con.
        /// </summary>
        /// <param name="con">The con.</param>
        public void Close(SqlConnection con)
        {
            if (con != null && con.State != System.Data.ConnectionState.Closed)
            {
                con.Close();
            }
        }
    }
}
