namespace WWC._240711.ASPNETCore.Extensions
{
    public class TestStockAddModel
    {

        public string StockLocation { get; set; }

        public string Size { get; set; }

        public TestStockType StockType { get; set; }   

    }

    public enum TestStockType
    {
        [EnumDescription("创建")]
        Add = 1,

        [EnumDescription("修改")]
        Update = 2,
    }
}
