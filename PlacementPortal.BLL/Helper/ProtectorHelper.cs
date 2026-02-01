using Microsoft.AspNetCore.DataProtection;

namespace PlacementPortal.BLL.Helper;

public class ProtectorHelper
{
    private readonly IDataProtector _protector;

    public ProtectorHelper(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector("PlacementPortalProtector");
    }

    public string ProtectId(int id)
    {
        string protectedId = _protector.Protect(id.ToString());
        return protectedId;
    }

    public int UnProtectId(string protectedId)
    {
        string unprotectedId = _protector.Unprotect(protectedId);
        int idInt = int.Parse(unprotectedId);
        return idInt;
    }

    public string ProtectString(string toProtect)
    {
        string protectedString = _protector.Protect(toProtect);

        return protectedString;
    }

    public string UnProtectString(string protectedThing)
    {
        string unProtectedString = _protector.Unprotect(protectedThing);

        return unProtectedString;
    }

}
