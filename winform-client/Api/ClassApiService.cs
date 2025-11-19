using Academix.WinApp.Models.Classes;
using Academix.WinApp.Models.Teacher;
using Academix.WinApp.Models.Common;
using Academix.WinApp.Utils;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Academix.WinApp.Models.Classes.Responses;

namespace Academix.WinApp.Api
{
    public class ClassApiService
    {
        private readonly string _baseUrl;
        private readonly string _token;

        public ClassApiService()
        {
            _baseUrl = Config.GetApiBaseUrl() + "/api/Classes";
            _token = SessionManager.Token; // Lấy token đã lưu sau khi đăng nhập
        }

        // Lấy danh sách lớp mà giáo viên dạy
        public async Task<List<MyClassResponseDto>> GetMyClassesAsync()
        {
            var url = $"{_baseUrl}/my-classes";

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {error}");
            }

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponse<List<MyClassResponseDto>>>();

            return apiResponse?.Data ?? new List<MyClassResponseDto>();
        }


        // Lấy danh sách học viên trong lớp
        public async Task<List<ClassStudentDto>> GetStudentsByClassAsync(int classId)
        {
            using HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}/students";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result =
                await response.Content.ReadFromJsonAsync<ApiResponse<List<ClassStudentDto>>>();

            return result?.Data ?? new List<ClassStudentDto>();
        }

        public async Task<ClassPagedResult> GetClassesAsync(
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc")
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", SessionManager.Token);

            string url = $"{_baseUrl}?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                string err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result = await response.Content.ReadFromJsonAsync<
                ApiResponse<ClassPagedResult>
            >();

            return result?.Data ?? new ClassPagedResult();
        }

        public async Task<MyClassResponseDto> CreateClassAsync(string className, string classCode, string description)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var payload = new
            {
                className,
                classCode,
                description
            };

            var response = await client.PostAsJsonAsync(_baseUrl, payload);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            // đọc dữ liệu trả về theo schema ApiResponse
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<MyClassResponseDto>>();

            if (apiResponse == null || !apiResponse.Success)
                throw new Exception($"API ERROR: {apiResponse?.Errors?.FirstOrDefault() ?? "Không có dữ liệu"}");

            return apiResponse.Data;
        }

        // Lấy tất cả thành viên (học sinh + giáo viên) trong lớp
        public async Task<List<ClassMember>> GetAllMembersAsync(int classId)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}/members";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ClassMember>>>();
            return result?.Data ?? new List<ClassMember>();
        }

        // Lấy danh sách học sinh trong lớp
        public async Task<List<ClassMember>> GetStudentsAsync(int classId)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}/students";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ClassMember>>>();
            return result?.Data ?? new List<ClassMember>();
        }

        // Lấy danh sách giáo viên trong lớp
        public async Task<List<ClassMember>> GetTeachersAsync(int classId)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}/teachers";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ClassMember>>>();
            return result?.Data ?? new List<ClassMember>();
        }
        // Thêm học sinh vào lớp
        public async Task<ApiResponse<object>> AddStudentsToClassAsync(int classId, List<int> userIds)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}/members/students";

            var payload = new { userIds };

            var response = await client.PostAsJsonAsync(url, payload);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return result ?? new ApiResponse<object> { Success = false, Message = "Không có dữ liệu trả về" };
        }

        // Thêm giáo viên vào lớp
        public async Task<ApiResponse<object>> AddTeachersToClassAsync(int classId, List<int> userIds)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}/members/teachers";

            var payload = new { userIds };

            var response = await client.PostAsJsonAsync(url, payload);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return result ?? new ApiResponse<object> { Success = false, Message = "Không có dữ liệu trả về" };
        }
        // Lấy chi tiết 1 lớp (bao gồm giáo viên, học sinh)
        public async Task<ClassDetailDto?> GetClassDetailAsync(int classId)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ClassDetailDto>>();
            return result?.Data;
        }
        public async Task RemoveMemberAsync(int classId, int userId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}/members/{userId}";

            var response = await client.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }
        }

        public async Task UpdateClassAsync(int classId, string className, string description, bool isActive)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var payload = new
            {
                className = className,
                description = description,
                isActive = isActive
            };

            string url = $"{_baseUrl}/{classId}";
            var response = await client.PutAsJsonAsync(url, payload);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }
        }

        // Lấy thông tin lớp theo id
        public async Task<ClassDto> GetClassByIdAsync(int classId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}";

            // Gọi GET API
            var response = await client.GetAsync(url);

            // Nếu API trả về lỗi HTTP
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            // Deserialize JSON về ApiResponse<ClassData>
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ClassDto>>();

            if (result == null)
                throw new Exception("API trả về dữ liệu null.");

            if (!result.Success)
                throw new Exception($"API lỗi: {result.Message}");

            if (result.Data == null)
                throw new Exception("Không lấy được dữ liệu lớp từ API.");

            return result.Data;
        }

        public async Task DeleteClassAsync(int classId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            string url = $"{_baseUrl}/{classId}";

            var response = await client.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR {response.StatusCode}: {err}");
            }

            // Nếu muốn, có thể đọc JSON trả về
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            if (result == null)
                throw new Exception("API trả về dữ liệu null khi xóa lớp.");

            if (!result.Success)
                throw new Exception($"Xóa lớp thất bại: {result.Message}");
        }

    }
}
