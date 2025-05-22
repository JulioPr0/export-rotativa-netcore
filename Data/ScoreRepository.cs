using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RotativaDemo.Models;

namespace RotativaDemo.Data
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly string _connString;

            public ScoreRepository(IConfiguration config)
            {
                _connString = config
                    .GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Missing connection string");
            }

            public IEnumerable<StudentScore> GetAllScores()
            {
                var list = new List<StudentScore>();
                using var conn = new SqlConnection(_connString);
                using var cmd  = new SqlCommand("sp_GetStudentScores", conn)
                { CommandType = CommandType.StoredProcedure };
                conn.Open();
                using var rdr = cmd.ExecuteReader();

                int iId    = rdr.GetOrdinal("Id");
                int iNo = rdr.GetOrdinal("No");
                int iName  = rdr.GetOrdinal("Nama");
                int iScore = rdr.GetOrdinal("Nilai");

                while(rdr.Read())
                {
                    list.Add(new StudentScore {
                        Id    = rdr.GetInt32(   rdr.GetOrdinal("Id")),
                        No    = rdr.GetInt32(   rdr.GetOrdinal("No")),
                        Nama  = rdr.GetString(  rdr.GetOrdinal("Nama")),
                        Nilai = rdr.GetInt32(   rdr.GetOrdinal("Nilai")),
                    });
                }

                return list;
            }
        }
}
