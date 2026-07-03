-- Migration: Add LineID, UnitID, DepartmentID to tbl_weekly_report_Actions
-- Run once against the WeeklyReport database

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('tbl_weekly_report_Actions') AND name = 'LineID'
)
    ALTER TABLE tbl_weekly_report_Actions ADD LineID INT NULL;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('tbl_weekly_report_Actions') AND name = 'UnitID'
)
    ALTER TABLE tbl_weekly_report_Actions ADD UnitID INT NULL;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('tbl_weekly_report_Actions') AND name = 'DepartmentID'
)
    ALTER TABLE tbl_weekly_report_Actions ADD DepartmentID INT NULL;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('tbl_weekly_report_Actions') AND name = 'WindowsUser'
)
    ALTER TABLE tbl_weekly_report_Actions ADD WindowsUser NVARCHAR(100) NULL;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('tbl_weekly_report_Actions') AND name = 'UserFullName'
)
    ALTER TABLE tbl_weekly_report_Actions ADD UserFullName NVARCHAR(200) NULL;
