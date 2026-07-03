using System;
using System.Collections.Generic;
using System.Configuration;
using EmployeeInfo.Models;

namespace EmployeeInfo
{
    public class EmployeeInfoService
    {
        public EmployeeSearchResult GetEmployeeInfo(string windowsAccountName)
        {
            if (string.IsNullOrWhiteSpace(windowsAccountName))
            {
                return null;
            }

            var normalized = windowsAccountName.Trim();
            var shortName = normalized.Contains("\\")
                ? normalized.Substring(normalized.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                : normalized;

            var titleLevel = 30;

            return new EmployeeSearchResult
            {
                UserId = 1,
                Name = "UMIT KAVALA",
                Title = "MANAGER",
                TitleLevel = titleLevel,
                TitleStatus = GetTitleStatus(titleLevel),
                Position = "DOKÜMAN PLATFORMS MANAGER",
                WindowsUsername = shortName.ToUpperInvariant(),
                DepartmentId = 3000,
                DepartmentName = "DOKUMAN PLATFORMLARI",
                UnitId = 2000,
                UnitName = "SÜREÇ PLATFORMLARI",
                UnitCode = "1",
                DivisionId = 1,
                DivisionName = "DIVISION"
            };
        }
  public List<DivisionHierarchyResult> GetDivisionHierarchy(bool useCache = true)
    {
        return new List<DivisionHierarchyResult>
        {
            new DivisionHierarchyResult
            {
                DivisionId = 1,
                DivisionName = "MİMARİ",
                Units = new List<UnitHierarchyResult>
                {
                    new UnitHierarchyResult
                    {
                        UnitId = 2000,
                        UnitName = "SÜREÇ PLATFORMLARI",
                        UnitCode = "1",
                        Departments = new List<DepartmentHierarchyResult>
                        {
                            new DepartmentHierarchyResult
                            {
                                DepartmentId = 3000,
                                DepartmentName = "DOKÜMAN PLATFORMLARI"
                            },
                            new DepartmentHierarchyResult
                            {
                                DepartmentId = 3001,
                                DepartmentName = "SÜREÇ PLATFORMLARI"
                            }
                        }
                    },
                    new UnitHierarchyResult
                    {
                        UnitId = 2001,
                        UnitName = "GÜVENLİK",
                        UnitCode = "2",
                        Departments = new List<DepartmentHierarchyResult>
                        {
                            new DepartmentHierarchyResult
                            {
                                DepartmentId = 3006,
                                DepartmentName = "ALTYAPI GÜVENLİĞİ"
                            }
                        }
                    }
                }
            },
             new DivisionHierarchyResult
            {
                DivisionId = 10,
                DivisionName = "ALTYAPI",
                Units = new List<UnitHierarchyResult>
                {
                    new UnitHierarchyResult
                    {
                        UnitId = 8000,
                        UnitName = "VERİ",
                        UnitCode = "20",
                        Departments = new List<DepartmentHierarchyResult>
                        {
                            new DepartmentHierarchyResult
                            {
                                DepartmentId = 6000,
                                DepartmentName = "SQL"
                            },
                            new DepartmentHierarchyResult
                            {
                                DepartmentId = 6001,
                                DepartmentName = "ORACLE"
                            }
                        }
                    },
                    new UnitHierarchyResult
                    {
                        UnitId = 2001,
                        UnitName = "GÜVENLİK",
                        UnitCode = "2",
                        Departments = new List<DepartmentHierarchyResult>
                        {
                            new DepartmentHierarchyResult
                            {
                                DepartmentId = 3006,
                                DepartmentName = "ALTYAPI GÜVENLİĞİ"
                            }
                        }
                    }
                }
            }
        };
    }
        private static string GetTitleStatus(int titleLevel)
        {
            if (titleLevel == GetConfiguredTitleLevel("EmployeeInfo.GeneralManagerTitleLevel", 10))
                return "GeneralManager";

            if (titleLevel == GetConfiguredTitleLevel("EmployeeInfo.EVPTitleLevel", 20))
                return "EVP";

            if (titleLevel == GetConfiguredTitleLevel("EmployeeInfo.UnitManagerTitleLevel", 25))
                return "UnitManager";

            if (titleLevel == GetConfiguredTitleLevel("EmployeeInfo.ManagerTitleLevel", 30))
                return "Manager";

            if (titleLevel >= GetConfiguredTitleLevel("EmployeeInfo.TeamMemberTitleLevel", 31))
                return "TeamMember";

            return "Unknown";
        }

        private static int GetConfiguredTitleLevel(string key, int defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            return int.TryParse(value, out var parsed) ? parsed : defaultValue;
        }
    }
}
