using Microsoft.AspNetCore.Mvc.TagHelpers;
using PlacementPortal.BLL.Enums;

namespace PlacementPortal.BLL.Constants;

public class RoleConsts
{
    public static readonly string Admin = RoleEnum.Admin.ToString();

    public static readonly string TPO = RoleEnum.TPO.ToString();

    public static readonly string Student = RoleEnum.Student.ToString();

    public static readonly string Unknown = "Unknown";

    public const string AdminRole = "Admin";

    public const string TPORole = "TPO";

    public const string StudentRole = "Student";

}
