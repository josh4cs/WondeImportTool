namespace GradeUp.Models
{
    public class WondeImports
    {
        public string request_id { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public int time_taken { get; set; }
    }
    public class WondeImportSummary
    {
        public string request_id { get; set; }
        public string school_id { get; set; }
        public string school_name { get; set; }
        public string table_name { get; set; }
        public string instance { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public int time_taken { get; set; }
        public int rows_processed { get; set; }
        public int request_ended { get; set; }
    }
}
