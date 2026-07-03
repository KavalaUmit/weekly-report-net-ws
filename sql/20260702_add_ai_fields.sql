-- Add AI Kazanımları fields to tbl_weekly_report_Actions if missing
IF COL_LENGTH('dbo.tbl_weekly_report_Actions','Project') IS NULL
  ALTER TABLE dbo.tbl_weekly_report_Actions ADD Project NVARCHAR(200) NULL;
IF COL_LENGTH('dbo.tbl_weekly_report_Actions','GainType') IS NULL
  ALTER TABLE dbo.tbl_weekly_report_Actions ADD GainType NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.tbl_weekly_report_Actions','UpdatedEffort') IS NULL
  ALTER TABLE dbo.tbl_weekly_report_Actions ADD UpdatedEffort NVARCHAR(50) NULL;
IF COL_LENGTH('dbo.tbl_weekly_report_Actions','EffortGain') IS NULL
  ALTER TABLE dbo.tbl_weekly_report_Actions ADD EffortGain NVARCHAR(50) NULL;
IF COL_LENGTH('dbo.tbl_weekly_report_Actions','TTMGain') IS NULL
  ALTER TABLE dbo.tbl_weekly_report_Actions ADD TTMGain NVARCHAR(50) NULL;
