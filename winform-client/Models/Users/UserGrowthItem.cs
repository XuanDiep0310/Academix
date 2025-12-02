using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Academix.WinApp.Models.Dashboard
{
    // ===== User Statistics =====
    public class UserGrowthItem
    {
        [JsonProperty("month")]
        public string Month { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }

    public class DashboardUserStatisticDto
    {
        [JsonProperty("totalUsers")]
        public int TotalUsers { get; set; }

        [JsonProperty("totalAdmins")]
        public int TotalAdmins { get; set; }

        [JsonProperty("totalTeachers")]
        public int TotalTeachers { get; set; }

        [JsonProperty("totalStudents")]
        public int TotalStudents { get; set; }

        [JsonProperty("activeUsers")]
        public int ActiveUsers { get; set; }

        [JsonProperty("inactiveUsers")]
        public int InactiveUsers { get; set; }

        [JsonProperty("userGrowth")]
        public List<UserGrowthItem> UserGrowth { get; set; } = new();
    }

    // ===== Class Statistics =====
    public class ClassGrowthItem
    {
        [JsonProperty("month")]
        public string Month { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }

    public class ClassDashboardData
    {
        [JsonProperty("totalClasses")]
        public int TotalClasses { get; set; }

        [JsonProperty("activeClasses")]
        public int ActiveClasses { get; set; }

        [JsonProperty("inactiveClasses")]
        public int InactiveClasses { get; set; }

        [JsonProperty("totalTeachers")]
        public int TotalTeachers { get; set; }

        [JsonProperty("totalStudents")]
        public int TotalStudents { get; set; }

        [JsonProperty("averageStudentsPerClass")]
        public double AverageStudentsPerClass { get; set; }

        [JsonProperty("classGrowth")]
        public List<ClassGrowthItem> ClassGrowth { get; set; } = new();
    }

    // ===== Material Statistics =====
    public class MaterialByType
    {
        [JsonProperty("additionalProp1")]
        public int AdditionalProp1 { get; set; }

        [JsonProperty("additionalProp2")]
        public int AdditionalProp2 { get; set; }

        [JsonProperty("additionalProp3")]
        public int AdditionalProp3 { get; set; }
    }

    public class TopUploader
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("materialCount")]
        public int MaterialCount { get; set; }
    }

    public class MaterialDashboardData
    {
        [JsonProperty("totalMaterials")]
        public int TotalMaterials { get; set; }

        [JsonProperty("materialsByType")]
        public MaterialByType MaterialsByType { get; set; } = new();

        [JsonProperty("totalStorageUsed")]
        public int TotalStorageUsed { get; set; }

        [JsonProperty("totalStorageUsedFormatted")]
        public string TotalStorageUsedFormatted { get; set; }

        [JsonProperty("materialsUploadedToday")]
        public int MaterialsUploadedToday { get; set; }

        [JsonProperty("materialsUploadedThisWeek")]
        public int MaterialsUploadedThisWeek { get; set; }

        [JsonProperty("materialsUploadedThisMonth")]
        public int MaterialsUploadedThisMonth { get; set; }

        [JsonProperty("topUploaders")]
        public List<TopUploader> TopUploaders { get; set; } = new();
    }

    
}
