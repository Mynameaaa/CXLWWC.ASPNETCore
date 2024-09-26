using Microsoft.AspNetCore.Authorization;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLAuthorizeAttribute : AuthorizeAttribute
{
    // 策略名前缀
    public const string PolicyPrefix = "CXLCustomAgeValidation";

    // 通过构造函数传入最小年龄
    public CXLAuthorizeAttribute(int minimumAge) =>
        MinimumAge = minimumAge;
    public int MinimumAge
    {
        get
        {
            // 从策略名中解析出最小年龄
            if (int.TryParse(Policy[PolicyPrefix.Length..], out var age))
            {
                return age;
            }

            return default;
        }
        set
        {
            // 生成动态的策略名，如 MinimumAge18 表示最小年龄为18的策略
            Policy = $"{PolicyPrefix}{value}";
        }
    }

}