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
            {
                CommandType = CommandType.StoredProcedure
            };

            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new StudentScore
                {
                    Id    = rdr.GetInt32(rdr.GetOrdinal("Id")),
                    Name  = rdr.GetString(rdr.GetOrdinal("Name")),
                    Score = rdr.GetInt32(rdr.GetOrdinal("Score")),
                    Rank  = rdr.GetInt32(rdr.GetOrdinal("Rank"))
                });
            }

            return list;
        }
    }
}
