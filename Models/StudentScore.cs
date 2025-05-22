namespace RotativaDemo.Models
{
    public class StudentScore
    {
        public int Id
        {
            get; set;
        }
        public int No
        {
            get; set;
        }
        public string Nama
        {
            get; set;
        } = string.Empty; 
        public int Nilai
        {
            get; set;
        }
        public int Rank
        {
            get; set;
        }
    }
}
