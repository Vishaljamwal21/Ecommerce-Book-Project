using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project_Ecomm_1.DataAccess.Data;
using Project_Ecomm_1.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_1.DataAccess.Repository
{
    public class SP_Call : ISP_Call
    {
        private readonly ApplicationDbContext _context;
        private static string ConnectionString = "";
        public SP_Call(ApplicationDbContext context)
        {
            _context = context;
            ConnectionString = _context.Database.GetDbConnection().ConnectionString;       
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Execute(string ProcedureName, DynamicParameters Param = null)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCon.Open();
                SqlCon.Execute(ProcedureName, Param, commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<T> List<T>(string ProcedureName, DynamicParameters Param = null)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCon.Open();
                return SqlCon.Query<T>(ProcedureName, Param, commandType: CommandType.StoredProcedure);
            }
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string ProcedureName, DynamicParameters Param = null)
        {
            using(SqlConnection SqlCon=new SqlConnection(ConnectionString))
            {
                SqlCon.Open();
                var result = SqlCon.QueryMultiple(ProcedureName, Param, commandType: CommandType.StoredProcedure);
                var item1 = result.Read<T1>();
                var item2 = result.Read<T2>();
                if (item1 != null && item2 != null)
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());
            }
        }

        public T oneRecord<T>(string ProcedureName, DynamicParameters Param = null)
        {
            using (SqlConnection SqlCon= new SqlConnection(ConnectionString))
            {
                SqlCon.Open();
                var value = SqlCon.Query<T>(ProcedureName, Param, commandType: CommandType.StoredProcedure);
                return value.FirstOrDefault();
            }
        }

        public T single<T>(string ProcedureName, DynamicParameters Param = null)
        {
            using (SqlConnection SqlCon = new SqlConnection(ConnectionString))
            {
                SqlCon.Open();
                return SqlCon.ExecuteScalar<T>(ProcedureName, Param, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
