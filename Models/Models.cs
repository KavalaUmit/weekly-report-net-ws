using System;
using System.Collections.Generic;

namespace WeeklyReportWS.Models
{

public class User
{
    public int UserID { get; set; }
    public string? WindowName { get; set; }
    public string? FullName { get; set; }
    public string? Title { get; set; }
    public byte? PositionNumber { get; set; }
    public int? DepartmentID { get; set; }
    public string? DepartmentName { get; set; }
    public int? UnitID { get; set; }
    public string? UnitName { get; set; }
    public int? LineID { get; set; }
    public string? LineName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateUserRequest
{
    public string? WindowName { get; set; }
    public string? FullName { get; set; }
    public int? DepartmentID { get; set; }
    public int? UnitID { get; set; }
    public int? LineID { get; set; }
    public string? Title { get; set; }
    public byte? PositionNumber { get; set; }
}

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public int? DepartmentID { get; set; }
    public int? UnitID { get; set; }
    public int? LineID { get; set; }
    public string? Title { get; set; }
    public byte? PositionNumber { get; set; }
}

public class ActionType
{
    public int TypeID { get; set; }
    public string? TypeName { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CreateActionTypeRequest
{
    public string? TypeName { get; set; }
    public int SortOrder { get; set; }
}

public class UpdateActionTypeRequest
{
    public string? TypeName { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Week
{
    public int WeekID { get; set; }
    public byte WeekNumber { get; set; }
    public short Year { get; set; }
}

public class CreateWeekRequest
{
    public byte WeekNumber { get; set; }
    public short Year { get; set; }
}

public class ActionStatus
{
    public int StatusID { get; set; }
    public string? StatusKey { get; set; }
    public string? StatusLabel { get; set; }
    public string? ColorHex { get; set; }
    public string? BgColorHex { get; set; }
}

public class CreateActionStatusRequest
{
    public string? StatusKey { get; set; }
    public string? StatusLabel { get; set; }
    public string? ColorHex { get; set; }
    public string? BgColorHex { get; set; }
}

public class UpdateActionStatusRequest
{
    public string? StatusLabel { get; set; }
    public string? ColorHex { get; set; }
    public string? BgColorHex { get; set; }
}

public class ActionItem
{
    public long ItemID { get; set; }
    public long ActionID { get; set; }
    public byte SortOrder { get; set; }
    public string? ItemType { get; set; }
    public string? ItemValue { get; set; }
}

public class ActionItemInput
{
    public string? type { get; set; }
    public string? value { get; set; }
}

public class CreateActionItemRequest
{
    public string? ItemType { get; set; }
    public string? ItemValue { get; set; }
    public byte SortOrder { get; set; }
}

public class UpdateActionItemRequest
{
    public string? ItemType { get; set; }
    public string? ItemValue { get; set; }
    public byte SortOrder { get; set; }
}

public class Action
{
    public long ActionID { get; set; }
    public int UserID { get; set; }
    public int WeekID { get; set; }
    public int TypeID { get; set; }
    public DateTime? ActionDate { get; set; }
    public int? StatusID { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte WeekNumber { get; set; }
    public short Year { get; set; }
    public string? TypeName { get; set; }
    public string? StatusKey { get; set; }
    public string? StatusLabel { get; set; }
    public string? ColorHex { get; set; }
    public string? BgColorHex { get; set; }
    public string? FullName { get; set; }
    public int? LineID { get; set; }
    public int? UnitID { get; set; }
    public List<ActionItem>? ActionItems { get; set; }
}

public class CreateActionRequest
{
    public int UserID { get; set; }
    public int WeekID { get; set; }
    public int TypeID { get; set; }
    public string? ActionDate { get; set; }
    public int? StatusID { get; set; }
    public List<ActionItemInput>? actionItems { get; set; }
}

public class UpdateActionRequest
{
    public int? WeekID { get; set; }
    public int? TypeID { get; set; }
    public string? ActionDate { get; set; }
    public int? StatusID { get; set; }
    public List<ActionItemInput>? actionItems { get; set; }
}

public class PatchActionStatusRequest
{
    public int? StatusID { get; set; }
    public int ChangedBy { get; set; }
}

public class ActionStatusHistory
{
    public long HistoryID { get; set; }
    public long ActionID { get; set; }
    public int? StatusID { get; set; }
    public int ChangedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string? StatusKey { get; set; }
    public string? StatusLabel { get; set; }
    public string? ColorHex { get; set; }
    public string? ChangedByName { get; set; }
}

public class CreateStatusHistoryRequest
{
    public int? StatusID { get; set; }
    public int ChangedBy { get; set; }
}

public class Line
{
    public int LineID { get; set; }
    public string? LineName { get; set; }
}

public class CreateLineRequest
{
    public string? LineName { get; set; }
}

public class Unit
{
    public int UnitID { get; set; }
    public int LineID { get; set; }
    public string? UnitName { get; set; }
    public string? LineName { get; set; }
}

public class CreateUnitRequest
{
    public int LineID { get; set; }
    public string? UnitName { get; set; }
}

public class Department
{
    public int DepartmentID { get; set; }
    public int UnitID { get; set; }
    public string? DepartmentName { get; set; }
    public string? UnitName { get; set; }
    public int? LineID { get; set; }
    public string? LineName { get; set; }
}

public class CreateDepartmentRequest
{
    public int UnitID { get; set; }
    public string? DepartmentName { get; set; }
}

} // namespace WeeklyReportWS.Models
