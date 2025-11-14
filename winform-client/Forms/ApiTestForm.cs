//using Academix.WinApp.Api;
//using System;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace Academix.WinApp.Forms
//{
//    public partial class ApiTestForm : Form
//    {
//        private readonly ApiClient _api;

//        public ApiTestForm()
//        {
//            InitializeComponent();
//            _api = new ApiClient("http://localhost:5000/api");
//        }

//        private async void btnTestApi_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                var loginData = new
//                {
//                    email = "test@gmail.com",
//                    password = "123456"
//                };

//                var result = await _api.PostAsync<object>("/Auth/register", loginData);
//                Console.WriteLine(result);

//                MessageBox.Show("Đăng ký thành công!");
//            }
//            catch (HttpRequestException httpEx)
//            {
//                MessageBox.Show($"Lỗi HTTP: {httpEx.Message}");
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Lỗi gọi API: {ex.Message}");
//            }
//        }
//    }
//}
