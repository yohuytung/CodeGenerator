using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class SQLHelper
    {
        public SqlConnection _con;

        public void SetCon(string connectionString)
        {
            _con = new SqlConnection(connectionString);
        }

        private SqlConnection Con
        {
            get { return _con; }
        }

        public bool TestCon(string connectionString)
        {
            SqlConnection tempcon = new SqlConnection(connectionString);
            try
            {
                tempcon.Open();
            }
            catch (Exception)
            {
                return false;
            }
            tempcon.Close();
            return true;
        }

        public DataTable GetDBSchema()
        {
            return GetDataList("sp_tables");
        }

        public DataTable GetDataList(string cmdStr)
        {
            DataTable tb = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmdStr, Con);
            adapter.Fill(tb);
            return tb;
        }


        public DataTable GetDataSchema(string cmdStr)
        {
            DataTable tb = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmdStr, Con);
            adapter.FillSchema(tb, SchemaType.Source);
            return tb;
        }

        public List<TableFieldInfo> GetTableInfo(string tableName)
        {
            List<TableFieldInfo> r = new List<TableFieldInfo>();
            string sql = @"SELECT  
                                --[Table Name] = OBJECT_NAME(c.object_id),
                            [ColumnName] = c.name,
                            [Description] = ex.value
                            FROM
                                sys.columns c
                            LEFT OUTER JOIN
                                sys.extended_properties ex
                            ON
                                ex.major_id = c.object_id
                                AND ex.minor_id = c.column_id
                                AND ex.name = 'MS_Description'
                            WHERE
                                OBJECTPROPERTY(c.object_id, 'IsMsShipped') = 0
                                AND OBJECT_NAME(c.object_id) = @tableName
                            ORDER
                                BY OBJECT_NAME(c.object_id), c.column_id";


            r = _con.Query<TableFieldInfo>(sql, new { tableName }).ToList();
            return r;
        }
    }
}
