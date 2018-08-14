using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceDataToDb
{
    public class SqlOpreation
    {
        private string sqlConnString = ConfigurationManager.ConnectionStrings["SpringerDBConnection"].ConnectionString;       // 数据库连接字符串
        private SqlConnection sqlConn = new SqlConnection();    // SQL数据库连接对象

        private ConnectionState sqlConnPreState = ConnectionState.Closed;   //原来数据库的连接状态
  
  
        /// <summary>
        /// 数据库连接字符串属性。
        /// </summary>
        public string SqlConnectionString
        {
            get
            {
                return sqlConnString;
            }
            set
            {
                sqlConnString = value;
                sqlConn.ConnectionString = sqlConnString;
            }
        }
  
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="strSqlCon">数据库连接字符串。</param>
        //public SqlOpreation(string strSqlCon)
        //{
        //    sqlConnString = strSqlCon;
        //    sqlConn.ConnectionString = sqlConnString;
  
        //}


        /// <summary>
        /// 获取查询的数据集。
        /// </summary>
        /// <param name="strSQL">要查询的SQL语句。</param>
        /// <param name="parametes">传入的参数，无参数时使用NULL。</param>
        /// <returns></returns>
        public DataSet GetDataSet(string strSQL)
        {
            DataSet ds = new DataSet();
            try
            {
                if (sqlConn.State == ConnectionState.Closed && sqlConn.State == ConnectionState.Closed)        //若原来的状态为关闭且当前连接未打开
                {
                    sqlConn.ConnectionString = sqlConnString;
                    sqlConn.Open();
                }
                SqlCommand sqlCmd = sqlConn.CreateCommand();
                sqlCmd.CommandText = strSQL;
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnPreState == ConnectionState.Closed && sqlConn.State != ConnectionState.Closed)  //若原来的状态为关闭且者当前连接未关闭则关闭
                {
                    sqlConn.Close();
                }
            }
            return ds;
        }

        /// <summary>
        /// 获取查询的数据表。
        /// </summary>
        /// <param name="strSQL">要查询的SQL语句。</param>
        /// <param name="parametes">传入的参数，无参数时使用NULL。</param>
        /// <returns></returns>
        public DataTable GetDataTable(string strSQL)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            try
            {
                //sqlConn.Open();
                if (sqlConn.State == ConnectionState.Closed && sqlConn.State == ConnectionState.Closed)        //若原来的状态为关闭且当前连接未打开
                {
                    sqlConn.Open();
                }

                SqlCommand sqlCmd = sqlConn.CreateCommand();
                sqlCmd.CommandText = strSQL;
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnPreState == ConnectionState.Closed && sqlConn.State != ConnectionState.Closed)  //若原来的状态为关闭且者当前连接未关闭则关闭
                {
                    sqlConn.Close();
                }
            }
            return dt;

        }


        /// <summary>
        /// 返回SqlDataReader对象。该函数需要在外部打开和关闭连接操作。
        /// </summary>
        /// <param name="strSQL">传入的SQL语句。</param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string strSQL)
        {
            SqlDataReader reader;
            try
            {
                if (sqlConn.State == ConnectionState.Closed && sqlConn.State == ConnectionState.Closed)        //若原来的状态为关闭且当前连接未打开
                {
                    sqlConn.ConnectionString = sqlConnString;
                    sqlConn.Open();
                }
                SqlCommand sqlCmd = sqlConn.CreateCommand();
                sqlCmd.CommandText = strSQL;
                reader = sqlCmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnPreState == ConnectionState.Closed && sqlConn.State != ConnectionState.Closed)  //若原来的状态为关闭且者当前连接未关闭则关闭
                {
                    sqlConn.Close();
                }
            }
            return reader;
        }
    

        public int ExecuteNonQuery(string strSQL)//, params SqlParameter[] parametes
        {

            int sqlInt = -1;
            try
            {
                 if (sqlConn.State == ConnectionState.Closed && sqlConn.State == ConnectionState.Closed)        //若原来的状态为关闭且当前连接未打开
                {
                    sqlConn.ConnectionString = sqlConnString;
                    sqlConn.Open();                   
                }

                SqlCommand sqlCmd = sqlConn.CreateCommand();
                sqlCmd.CommandText = strSQL;

                //if (parametes != null)
                //{
                //    sqlCmd.Parameters.Clear();
                //    sqlCmd.Parameters.AddRange(parametes);
                //}

                sqlInt = sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnPreState == ConnectionState.Closed && sqlConn.State != ConnectionState.Closed)  //若原来的状态为关闭且者当前连接未关闭则关闭
                {
                    sqlConn.Close();
                }
            }
            return sqlInt;

        }

        public int ExecuteScala(string strSQL)//, params SqlParameter[] parametes
        {
            int  sqlInt;
            try
            {
                if (sqlConn.State == ConnectionState.Closed && sqlConn.State == ConnectionState.Closed)        //若原来的状态为关闭且当前连接未打开
                {
                    sqlConn.ConnectionString = sqlConnString;
                    sqlConn.Open();
                }

                SqlCommand sqlCmd = sqlConn.CreateCommand();
                sqlCmd.CommandText = strSQL;

                //if (parametes != null)
                //{
                //    sqlCmd.Parameters.Clear();
                //    sqlCmd.Parameters.AddRange(parametes);
                //}

                sqlInt = Convert.ToInt32(sqlCmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnPreState == ConnectionState.Closed && sqlConn.State != ConnectionState.Closed)  //若原来的状态为关闭且者当前连接未关闭则关闭
                {
                    sqlConn.Close();
                }
            }
            return sqlInt;

        }
    }
}
