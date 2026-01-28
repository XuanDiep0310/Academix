using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Academix.WinApp.Languages
{
    public static class LanguageManager
    {
        private static Dictionary<string, Dictionary<string, string>> _translations;
        private static string _currentLanguage = "vi"; // Mặc định tiếng Việt
        private static readonly string _languageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages");

        static LanguageManager()
        {
            LoadLanguages();
        }

        private static void LoadLanguages()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>();

            // Tạo thư mục Languages nếu chưa có
            if (!Directory.Exists(_languageFolder))
            {
                Directory.CreateDirectory(_languageFolder);
            }

            // Load file tiếng Việt
            var viFile = Path.Combine(_languageFolder, "vi.json");
            if (File.Exists(viFile))
            {
                var viContent = File.ReadAllText(viFile);
                _translations["vi"] = JsonConvert.DeserializeObject<Dictionary<string, string>>(viContent) ?? new Dictionary<string, string>();
            }
            else
            {
                // Tạo file mặc định tiếng Việt
                _translations["vi"] = GetDefaultVietnamese();
                SaveLanguage("vi", _translations["vi"]);
            }

            // Load file tiếng Anh
            var enFile = Path.Combine(_languageFolder, "en.json");
            if (File.Exists(enFile))
            {
                var enContent = File.ReadAllText(enFile);
                _translations["en"] = JsonConvert.DeserializeObject<Dictionary<string, string>>(enContent) ?? new Dictionary<string, string>();
            }
            else
            {
                // Tạo file mặc định tiếng Anh
                _translations["en"] = GetDefaultEnglish();
                SaveLanguage("en", _translations["en"]);
            }
        }

        public static void SetLanguage(string languageCode)
        {
            ChangeLanguage(languageCode);
        }

        public static string GetString(string key)
        {
            if (_translations.ContainsKey(_currentLanguage) &&
                _translations[_currentLanguage].ContainsKey(key))
            {
                return _translations[_currentLanguage][key];
            }

            // Fallback về tiếng Việt nếu không tìm thấy
            if (_translations.ContainsKey("vi") &&
                _translations["vi"].ContainsKey(key))
            {
                return _translations["vi"][key];
            }

            return key; // Trả về key nếu không tìm thấy
        }

        public static string CurrentLanguage => _currentLanguage;

        public static List<string> GetAvailableLanguages()
        {
            return _translations.Keys.ToList();
        }

        /// <summary>
        /// Event để thông báo khi ngôn ngữ thay đổi
        /// </summary>
        public static event EventHandler LanguageChanged;

        /// <summary>
        /// Thay đổi ngôn ngữ và trigger event
        /// </summary>
        public static void ChangeLanguage(string languageCode)
        {
            if (_translations.ContainsKey(languageCode) && _currentLanguage != languageCode)
            {
                _currentLanguage = languageCode;
                LanguageChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        private static void SaveLanguage(string langCode, Dictionary<string, string> translations)
        {
            var filePath = Path.Combine(_languageFolder, $"{langCode}.json");
            var json = JsonConvert.SerializeObject(translations, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        private static Dictionary<string, string> GetDefaultVietnamese()
        {
            return new Dictionary<string, string>
            {
                // FormSignIn
                {"LoginTitle", "Đăng nhập"},
                {"WelcomeBack", "Chào mừng bạn quay trở lại"},
                {"Email", "Email"},
                {"Password", "Mật khẩu"},
                {"Login", "Đăng nhập"},
                {"LoggingIn", "Đang đăng nhập..."},
                {"ForgotPassword", "Quên mật khẩu"},
                {"PleaseEnterEmail", "Vui lòng nhập email!"},
                {"PleaseEnterPassword", "Vui lòng nhập mật khẩu!"},
                {"LoginSuccess", "Đăng nhập thành công!"},
                {"LoginFailed", "Đăng nhập thất bại"},
                {"Error", "Lỗi"},
                {"Notification", "Thông báo"},
                {"InvalidSession", "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại!"},
                {"InvalidRole", "Role không hợp lệ!"},
                {"ErrorOpeningForm", "Lỗi khi mở form chính: {0}"},
                
                // Common
                {"Confirm", "Xác nhận"},
                {"Cancel", "Hủy"},
                {"Yes", "Có"},
                {"No", "Không"},
                {"OK", "OK"},
                {"Close", "Đóng"},
                {"Save", "Lưu"},
                {"Delete", "Xóa"},
                {"Edit", "Sửa"},
                {"Add", "Thêm"},
                {"Search", "Tìm kiếm"},
                {"Refresh", "Làm mới"},
                {"Loading", "Đang tải..."},
                {"NoData", "Không có dữ liệu"},
                {"Success", "Thành công"},
                {"Failed", "Thất bại"},
                
                // Logout
                {"ConfirmLogout", "Bạn có chắc muốn đăng xuất không?"},
                {"LogoutTitle", "Xác nhận đăng xuất"},
                {"Logout", "Đăng xuất"},
                
                // Change Password
                {"ChangePassword", "Đổi mật khẩu"},
                {"CurrentPassword", "Mật khẩu hiện tại"},
                {"NewPassword", "Mật khẩu mới"},
                {"ConfirmPassword", "Xác nhận mật khẩu"},
                
                // Admin Menu
                {"AdminDashboard", "Tổng quan"},
                {"AccountManagement", "Quản lý tài khoản"},
                {"ClassManagement", "Quản lý lớp học"},
                {"Settings", "Cài đặt"},
                {"Language", "Ngôn ngữ"},
                
                // Teacher Menu
                {"MyClasses", "Lớp học của tôi"},
                {"Materials", "Tài liệu"},
                {"QuestionBank", "Ngân hàng câu hỏi"},
                {"Exams", "Bài kiểm tra"},
                {"Results", "Kết quả"},
                {"Teacher", "Giáo viên"},
                
                // Student Menu
                {"MyClass", "Lớp học của tôi"},
                {"StudyMaterials", "Tài liệu học tập"},
                {"MyResults", "Kết quả của tôi"},
                {"Student", "Học sinh"},
                
                // Common Labels
                {"FullName", "Họ và tên"},
                {"Name", "Tên"},
                {"Description", "Mô tả"},
                {"Status", "Trạng thái"},
                {"Active", "Hoạt động"},
                {"Inactive", "Không hoạt động"},
                {"CreatedDate", "Ngày tạo"},
                {"UpdatedDate", "Ngày cập nhật"},
                {"Actions", "Thao tác"},
                {"View", "Xem"},
                {"Update", "Cập nhật"},
                {"Remove", "Xóa bỏ"},
                {"Select", "Chọn"},
                {"All", "Tất cả"},
                
                // Network
                {"NetworkError", "Có lỗi xảy ra:\n{0}\n\nVui lòng kiểm tra:\n1. API có đang chạy không?\n2. Đường dẫn API có đúng không?"},
                {"ConnectionError", "Lỗi kết nối"},
                {"TryAgain", "Thử lại"}
            };
        }

        private static Dictionary<string, string> GetDefaultEnglish()
        {
            return new Dictionary<string, string>
            {
                // FormSignIn
                {"LoginTitle", "Sign In"},
                {"WelcomeBack", "Welcome back"},
                {"Email", "Email"},
                {"Password", "Password"},
                {"Login", "Sign In"},
                {"LoggingIn", "Signing in..."},
                {"ForgotPassword", "Forgot Password"},
                {"PleaseEnterEmail", "Please enter your email!"},
                {"PleaseEnterPassword", "Please enter your password!"},
                {"LoginSuccess", "Login successful!"},
                {"LoginFailed", "Login failed"},
                {"Error", "Error"},
                {"Notification", "Notification"},
                {"InvalidSession", "Invalid session. Please login again!"},
                {"InvalidRole", "Invalid role!"},
                {"ErrorOpeningForm", "Error opening main form: {0}"},
                
                // Common
                {"Confirm", "Confirm"},
                {"Cancel", "Cancel"},
                {"Yes", "Yes"},
                {"No", "No"},
                {"OK", "OK"},
                {"Close", "Close"},
                {"Save", "Save"},
                {"Delete", "Delete"},
                {"Edit", "Edit"},
                {"Add", "Add"},
                {"Search", "Search"},
                {"Refresh", "Refresh"},
                {"Loading", "Loading..."},
                {"NoData", "No data"},
                {"Success", "Success"},
                {"Failed", "Failed"},
                
                // Logout
                {"ConfirmLogout", "Are you sure you want to logout?"},
                {"LogoutTitle", "Confirm Logout"},
                {"Logout", "Logout"},
                
                // Change Password
                {"ChangePassword", "Change Password"},
                {"CurrentPassword", "Current Password"},
                {"NewPassword", "New Password"},
                {"ConfirmPassword", "Confirm Password"},
                
                // Admin Menu
                {"AdminDashboard", "Dashboard"},
                {"AccountManagement", "Account Management"},
                {"ClassManagement", "Class Management"},
                {"Settings", "Settings"},
                {"Language", "Language"},
                
                // Teacher Menu
                {"MyClasses", "My Classes"},
                {"Materials", "Materials"},
                {"QuestionBank", "Question Bank"},
                {"Exams", "Exams"},
                {"Results", "Results"},
                {"Teacher", "Teacher"},
                
                // Student Menu
                {"MyClass", "My Class"},
                {"StudyMaterials", "Study Materials"},
                {"MyResults", "My Results"},
                {"Student", "Student"},
                
                // Common Labels
                {"FullName", "Full Name"},
                {"Name", "Name"},
                {"Description", "Description"},
                {"Status", "Status"},
                {"Active", "Active"},
                {"Inactive", "Inactive"},
                {"CreatedDate", "Created Date"},
                {"UpdatedDate", "Updated Date"},
                {"Actions", "Actions"},
                {"View", "View"},
                {"Update", "Update"},
                {"Remove", "Remove"},
                {"Select", "Select"},
                {"All", "All"},
                
                // Network
                {"NetworkError", "An error occurred:\n{0}\n\nPlease check:\n1. Is the API running?\n2. Is the API URL correct?"},
                {"ConnectionError", "Connection Error"},
                {"TryAgain", "Try Again"}
            };
        }
    }
}

