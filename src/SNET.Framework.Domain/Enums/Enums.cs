using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNET.Framework.Domain;

public enum StatusUser
{
    Active = 1,
    Inactive = 2,
    Locked = 3
}

public enum RoleUser
{
    Guest = 1,
    Admin = 2,
    Super = 3
}

public enum AuditCrudOperation
{
    Insert = 1,
    Update = 2,
    Delete = 3,
    Upsert = 4
}

public enum AuditLevel

{
    Trace = 1,
    Debug = 2,
    Information = 3,
    Warning = 4,
    Error = 5,
    Critical = 6
}

