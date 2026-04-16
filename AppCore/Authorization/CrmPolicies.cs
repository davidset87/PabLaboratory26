namespace AppCore.Authorization;

public enum CrmPolicies
{
    AdminOnly,
    SalesAccess,
    SalesManagerAccess,
    SupportAccess,
    ReadOnlyAccess,
    ActiveUser,
    SalesDepartment
}

public static class CrmPoliciesExtensions
{
    public static string Name(this CrmPolicies policy)
    {
        return policy.ToString();
    }
}