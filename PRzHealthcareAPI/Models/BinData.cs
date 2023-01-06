namespace PRzHealthcareAPI.Models
{
    public class BinData
    {
        public int Bin_Id { get; set; }
        public string Bin_Name { get; set; }
        public string Bin_Data { get; set; }
        public DateTime Bin_InsertedDate { get; set; }
        public int Bin_InsertedAccId { get; set; }
        public DateTime Bin_ModifiedDate { get; set; }
        public int Bin_ModifiedAccId { get; set; }
    }
}
