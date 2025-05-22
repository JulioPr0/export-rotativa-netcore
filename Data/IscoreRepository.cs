using System.Collections.Generic;
using RotativaDemo.Models;

namespace RotativaDemo.Data
{
    public interface IScoreRepository
    {
        IEnumerable<StudentScore> GetAllScores();
    }
}
