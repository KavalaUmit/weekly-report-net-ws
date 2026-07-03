using System.Collections.Generic;
using Newtonsoft.Json;

namespace EmployeeInfo.Models
{
    public class DivisionHierarchyResult
    {
        public int DivisionId { get; set; }

        public string DivisionName { get; set; }

        public List<UnitHierarchyResult> Units { get; set; }
    }

    public class UnitHierarchyResult
    {
        public int UnitId { get; set; }

        public string UnitName { get; set; }

        public string UnitCode { get; set; }

        public List<DepartmentHierarchyResult> Departments { get; set; }
    }

    public class DepartmentHierarchyResult
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }
    }


    public class Root
    {
        [JsonProperty("GMboxIsAlani")]
        public GeneralManagerArea GeneralManagerArea { get; set; }
    }

    public class GeneralManagerArea
    {
        [JsonProperty("gmBoxLine")]
        public List<GeneralManagerLine> GeneralManagerLines { get; set; }
    }

    public class GeneralManagerLine
    {
        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("Ad")]
        public string Name { get; set; }

        [JsonProperty("Personel")]
        public EmployeeContainer Employee { get; set; }

        [JsonProperty("isAlani")]
        public List<DivisionItem> Divisions { get; set; }
    }

    public class DivisionItem
    {
        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("Ad")]
        public string Name { get; set; }

        [JsonProperty("Personel")]
        public EmployeeContainer Employee { get; set; }

        [JsonProperty("Birim")]
        public UnitContainer UnitContainer { get; set; }
    }

    public class UnitContainer
    {
        [JsonProperty("birim")]
        public List<UnitItem> Units { get; set; }
    }

    public class UnitItem
    {
        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("Ad")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("Personel")]
        public EmployeeContainer Employee { get; set; }

        [JsonProperty("Bolum")]
        public DepartmentContainer DepartmentContainer { get; set; }
    }

    public class DepartmentContainer
    {
        [JsonProperty("bolum")]
        public List<DepartmentItem> Departments { get; set; }
    }

    public class DepartmentItem
    {
        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("Ad")]
        public string Name { get; set; }

        [JsonProperty("Personel")]
        public EmployeeContainer Employee { get; set; }
    }

    public class EmployeeContainer
    {
        [JsonProperty("personel")]
        public EmployeeInfo Employee { get; set; }
    }

    public class EmployeeInfo
    {
        [JsonProperty("personel")]
        public List<EmployeeItem> Employees { get; set; }

        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("ad")]
        public string Name { get; set; }

        [JsonProperty("pozisyon")]
        public string Position { get; set; }

        [JsonProperty("unvan")]
        public string Title { get; set; }

        [JsonProperty("unvanSeviyesi")]
        public int TitleLevel { get; set; }

        [JsonProperty("windowsKullaniciAdi")]
        public string WindowsUsername { get; set; }
    }

    public class EmployeeItem
    {
        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("ad")]
        public string Name { get; set; }

        [JsonProperty("pozisyon")]
        public string Position { get; set; }

        [JsonProperty("unvan")]
        public string Title { get; set; }

        [JsonProperty("unvanSeviyesi")]
        public int TitleLevel { get; set; }

        [JsonProperty("windowsKullaniciAdi")]
        public string WindowsUsername { get; set; }
    }

 public class EmployeeSearchResult
    {
        public int UserId { get; set; }
        
        public string Name { get; set; }

        public string Title { get; set; }

        public int TitleLevel { get; set; }

        public string TitleStatus { get; set; }

        public string Position { get; set; }

        public string WindowsUsername { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int UnitId { get; set; }

        public string UnitName { get; set; }

        public string UnitCode { get; set; }

        public int DivisionId { get; set; }

        public string DivisionName { get; set; }

    }

}
