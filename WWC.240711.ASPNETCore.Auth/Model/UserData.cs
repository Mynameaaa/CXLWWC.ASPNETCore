namespace WWC._240711.ASPNETCore.Auth;

public class UserData
{
    public static List<UserData> UserList { get; set; } = new List<UserData>()
    {
        new UserData()
        {
            Role = "Admin,User,BackAdmin",
            UserName = "ZWJJ",
            UserID = 666
        },
        new UserData()
        {
            Role = "User",
            UserName = "ZSFF",
            UserID = 777
        },
        new UserData()
        {
            Role = "Admin",
            UserName = "XLXL",
            UserID = 888
        },
    };

    public int UserID { get; set; }

    public string UserName { get; set; }

    public string Role { get; set; }

}
