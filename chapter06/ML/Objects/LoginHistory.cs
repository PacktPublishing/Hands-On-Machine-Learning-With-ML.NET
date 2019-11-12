using Microsoft.ML.Data;

namespace chapter06.ML.Objects
{
    public class LoginHistory
    {
        [LoadColumn(0)]
        public float UserID { get; set; }

        [LoadColumn(1)]
        public float CorporateNetwork { get; set; }

        [LoadColumn(2)] 
        public float HomeNetwork { get; set; }

        [LoadColumn(3)] 
        public float WithinWorkHours { get; set; }

        [LoadColumn(4)] 
        public float WorkDay { get; set; }

        [LoadColumn(5)] 
        public float Label { get; set; }
    }
}