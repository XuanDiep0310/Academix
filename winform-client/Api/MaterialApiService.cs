using Academix.WinApp.Models;
using Academix.WinApp.Models.Teacher;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Academix.WinApp.Api
{
    public class MaterialApiService
    {
        private readonly string _baseUrl;
        public MaterialApiService()
        {
            _baseUrl = Config.GetApiBaseUrl();
        }
        //public async Task<List<MaterialResponseDto>> GetMaterialsAsync(int classId, string? typeFilter)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(_baseUrl);
        //        client.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Bearer", SessionManager.Token);

        //        string url = $"/api/classes/{classId}/materials";

        //        if (!string.IsNullOrEmpty(typeFilter))
        //            url += $"?type={typeFilter}";

        //        var response = await client.GetAsync(url);
        //        response.EnsureSuccessStatusCode();

        //        var result = await response.Content.ReadFromJsonAsync<ApiResponse<MaterialListResponseDto>>();
        //        return result?.Data?.Materials ?? new();
        //    }
        //}
        public async Task<MaterialPagedResult> GetMaterialsPagedAsync(int classId, string? typeFilter, int page, int pageSize)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            string url = $"/api/classes/{classId}/materials?page={page}&pageSize={pageSize}";

            if (!string.IsNullOrEmpty(typeFilter))
                url += $"&type={typeFilter}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<MaterialListResponseDto>>();

            return new MaterialPagedResult
            {
                Materials = result.Data.Materials,
                Page = result.Data.Page,
                PageSize = result.Data.PageSize,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount
            };
        }
        public async Task<bool> UpdateMaterialAsync(int classId, MaterialResponseDto material)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var payload = new
            {
                Title = material.Title,
                Description = material.Description,
                MaterialType = material.MaterialType
            };

            var response = await client.PutAsJsonAsync(
                $"/api/classes/{classId}/materials/{material.MaterialId}",
                payload);

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteMaterialAsync(int classId, int materialId)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var response = await client.DeleteAsync(
                $"/api/classes/{classId}/materials/{materialId}");

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DownloadAsync(int classId, MaterialResponseDto m)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var response = await client.GetAsync(
                $"/api/classes/{classId}/materials/{m.MaterialId}/download");

            if (!response.IsSuccessStatusCode) return false;

            var bytes = await response.Content.ReadAsByteArrayAsync();

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = m.FileName;
            dlg.Filter = "All files|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllBytes(dlg.FileName, bytes);
                return true;
            }

            return false;
        }

        public async Task<bool> CreateMaterialAsync(int classId, CreateMaterialRequestDto dto)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            var response = await client.PostAsJsonAsync(
                $"/api/classes/{classId}/materials",
                dto);

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UploadMaterialFileAsync(int classId, UploadMaterialRequestDto dto)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(dto.Title), "Title");
            form.Add(new StringContent(dto.Description ?? ""), "Description");

            var fileContent = new StreamContent(File.OpenRead(dto.FilePath));
            form.Add(fileContent, "File", Path.GetFileName(dto.FilePath));

            var response = await client.PostAsync($"/api/classes/{classId}/materials/upload", form);

            return response.IsSuccessStatusCode;
        }

        



    }
}
