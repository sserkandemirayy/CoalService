namespace Domain.Constants;

public static class RtlsUserTypeCodes
{
    public const string System = "SYSTEM";
    public const string Employee = "EMPLOYEE";
    public const string Contractor = "CONTRACTOR";
    public const string Visitor = "VISITOR";
}

public static class RtlsRoleNames
{
    public const string SuperAdmin = "super_admin";
    public const string CompanyAdmin = "company_admin";
    public const string BranchAdmin = "branch_admin";
    public const string DispatchOperator = "dispatch_operator";
    public const string SafetyOperator = "safety_operator";
    public const string SecurityOperator = "security_operator";
    public const string MaintenanceOperator = "maintenance_operator";
    public const string FieldSupervisor = "field_supervisor";
    public const string Viewer = "viewer";

    public static readonly ISet<string> BuiltIn = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        SuperAdmin,
        CompanyAdmin,
        BranchAdmin,
        DispatchOperator,
        SafetyOperator,
        SecurityOperator,
        MaintenanceOperator,
        FieldSupervisor,
        Viewer
    };
}
