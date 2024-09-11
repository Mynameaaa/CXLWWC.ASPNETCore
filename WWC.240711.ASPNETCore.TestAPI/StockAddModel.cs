namespace WWC._240711.ASPNETCore.TestAPI
{
    public class StockAddModel
    {

        public string StockLocation { get; set; }

        public string Size { get; set; }

        public StockType StockType { get; set; }   

    }

    public enum StockType
    {
        [EnumDescription("创建")]
        Add = 1,

        [EnumDescription("修改")]
        Update = 2,
    }
}
