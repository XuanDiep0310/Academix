using Academix.WinApp.Api;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class UC_QLTaiKhoan : UserControl
    {
        private const int ACTION_COLUMN_WIDTH = 60;
        private readonly UserApi _userApi;
        private List<UserData> _currentUsers;

        public UC_QLTaiKhoan()
        {
            InitializeComponent();
            _userApi = new UserApi(Config.Get("ApiSettings:BaseUrl"));
            RegisterEvents();
        }

        #region Events
        private void RegisterEvents()
        {
            this.Load += UC_QLTaiKhoan_Load;
            dgvTaiKhoan.CellContentClick += DgvTaiKhoan_CellContentClick;
            //dgvTaiKhoan.CellFormatting += DgvTaiKhoan_CellFormatting;
            dgvTaiKhoan.DataError += DgvTaiKhoan_DataError;
        }

        private void DgvTaiKhoan_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            Debug.WriteLine($"DataError at row {e.RowIndex}, col {e.ColumnIndex}: {e.Exception.Message}");
        }
        #endregion

        #region Load Data
        private async void UC_QLTaiKhoan_Load(object sender, EventArgs e)
        {
            await LoadTaiKhoanAsync();
        }

        private int _currentPage = 1;
        private int _pageSize = 20;
        private int _totalPages = 1;


        private async Task LoadTaiKhoanAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                var result = await _userApi.GetAllUsersAsync(
                    page: _currentPage,
                    pageSize: _pageSize,
                    sortBy: "CreatedAt",
                    sortOrder: "desc"
                );

                _totalPages = result.TotalPages > 0 ? result.TotalPages : 1;

                dgvTaiKhoan.DataSource = new BindingList<UserData>(result.Users);

                ConfigureColumns();
                AddActionColumns();

                lblPageInfo.Text = $"Trang {_currentPage}/{_totalPages}";

                UpdateButtons();
            }
            catch (Exception ex)
            {
                HandleError("Lỗi tải dữ liệu", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private void UpdateButtons()
        {
            btnPrevious.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }


        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                await LoadTaiKhoanAsync();
            }
        }


        private async void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadTaiKhoanAsync();
            }
        }

        #endregion

        #region Configure DataGridView
        private void ConfigureColumns()
        {
            if (dgvTaiKhoan.Columns == null || dgvTaiKhoan.Columns.Count == 0) return;

            string[] visibleCols =
{
    "FullName", "Email", "Role", "IsActive", "CreatedAt",
    "btnEdit", "btnLock", "btnDelete"
};

            foreach (DataGridViewColumn col in dgvTaiKhoan.Columns)
                col.Visible = visibleCols.Contains(col.Name);


            ConfigureColumn("FullName", "Họ tên", autoSize: DataGridViewAutoSizeColumnMode.Fill, minWidth: 150);
            ConfigureColumn("Email", "Email", autoSize: DataGridViewAutoSizeColumnMode.Fill, minWidth: 200);
            ConfigureColumn("Role", "Vai trò", width: 120, backColor: Color.LightBlue, foreColor: Color.DarkBlue, alignment: DataGridViewContentAlignment.MiddleCenter);
            ConfigureColumn("IsActive", "Trạng thái", width: 100, alignment: DataGridViewContentAlignment.MiddleCenter);
            ConfigureColumn("CreatedAt", "Ngày tạo", width: 120, format: "dd/MM/yyyy", alignment: DataGridViewContentAlignment.MiddleCenter);

            int index = 0;
            string[] order = { "FullName", "Email", "Role", "IsActive", "CreatedAt" };
            foreach (var name in order)
            {
                if (dgvTaiKhoan.Columns[name] != null)
                    dgvTaiKhoan.Columns[name].DisplayIndex = index++;
            }

            dgvTaiKhoan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvTaiKhoan.AllowUserToAddRows = false;
            dgvTaiKhoan.AllowUserToDeleteRows = false;
            dgvTaiKhoan.ReadOnly = true;
            dgvTaiKhoan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTaiKhoan.MultiSelect = false;
            dgvTaiKhoan.RowHeadersVisible = false;
            dgvTaiKhoan.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void ConfigureColumn(string columnName, string headerText,
            int? width = null,
            DataGridViewAutoSizeColumnMode? autoSize = null,
            int? minWidth = null,
            Color? backColor = null,
            Color? foreColor = null,
            string format = null,
            DataGridViewContentAlignment? alignment = null)
        {
            var column = dgvTaiKhoan.Columns[columnName];
            if (column == null) return;

            column.HeaderText = headerText;
            if (width.HasValue) column.Width = width.Value;
            if (autoSize.HasValue) column.AutoSizeMode = autoSize.Value;
            if (minWidth.HasValue) column.MinimumWidth = minWidth.Value;
            if (backColor.HasValue) column.DefaultCellStyle.BackColor = backColor.Value;
            if (foreColor.HasValue) column.DefaultCellStyle.ForeColor = foreColor.Value;
            if (!string.IsNullOrEmpty(format)) column.DefaultCellStyle.Format = format;
            if (alignment.HasValue) column.DefaultCellStyle.Alignment = alignment.Value;
        }
        #endregion

        #region Action Columns
        private void AddActionColumns()
        {
            if (dgvTaiKhoan.Columns["btnEdit"] != null) return;

            AddButtonColumn("btnEdit", "Thao tác", "✏️");
            AddButtonColumn("btnLock", "", "🔒");
            AddButtonColumn("btnDelete", "", "🗑️");

            int lastIndex = dgvTaiKhoan.Columns.Count;
            dgvTaiKhoan.Columns["btnEdit"].DisplayIndex = lastIndex - 3;
            dgvTaiKhoan.Columns["btnLock"].DisplayIndex = lastIndex - 2;
            dgvTaiKhoan.Columns["btnDelete"].DisplayIndex = lastIndex - 1;
        }

        private void AddButtonColumn(string name, string headerText, string text)
        {
            var btn = new DataGridViewButtonColumn
            {
                Name = name,
                HeaderText = headerText,
                Text = text,
                UseColumnTextForButtonValue = true,
                Width = ACTION_COLUMN_WIDTH,
                FlatStyle = FlatStyle.Flat
            };
            dgvTaiKhoan.Columns.Add(btn);
        }
        #endregion

        #region Cell Formatting & Click
        //private void DgvTaiKhoan_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        //{
        //    if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
        //    if (dgvTaiKhoan.Columns[e.ColumnIndex].Name == "IsActive")
        //    {
        //        var user = dgvTaiKhoan.Rows[e.RowIndex].DataBoundItem as UserData;
        //        if (user != null)
        //        {
        //            e.Value = user.IsActive ? "Hoạt động" : "Đã khóa";
        //            e.FormattingApplied = true;
        //        }
        //    }
        //}


        private async void DgvTaiKhoan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var user = dgvTaiKhoan.Rows[e.RowIndex].DataBoundItem as UserData;
            if (user == null && _currentUsers != null && e.RowIndex < _currentUsers.Count)
                user = _currentUsers[e.RowIndex];

            if (user == null)
            {
                MessageBox.Show("Không thể lấy thông tin user.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // DEBUG
            Debug.WriteLine($"Selected: {user.FullName}, UserId={user.UserId}, Id={user.UserId}");

            string col = dgvTaiKhoan.Columns[e.ColumnIndex].Name;
            switch (col)
            {
                case "btnEdit": await HandleEditUser(user); break;
                case "btnLock": await HandleLockUnlockUser(user); break;
                case "btnDelete": await HandleDeleteUser(user); break;
            }
        }
        #endregion

        #region User Actions
        private async Task HandleEditUser(UserData user)
        {
            if (user == null || user.UserId <= 0)
            {
                MessageBox.Show($"User không hợp lệ. Id={user?.UserId}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using var form = new FormEditUser(user);
                if (form.ShowDialog() == DialogResult.OK)
                    await LoadTaiKhoanAsync();
            }
            catch (Exception ex)
            {
                HandleError("Lỗi mở form sửa", ex);
            }
        }

        private async Task HandleLockUnlockUser(UserData user)
        {
            if (user == null || user.UserId <= 0) return;

            var action = user.IsActive ? "khóa" : "mở khóa";
            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn {action} tài khoản:\n\n{user.FullName} ({user.Email})?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                Cursor = Cursors.WaitCursor;

                var response = await _userApi.UpdateUserAsync(
                    user.UserId,
                    user.FullName,
                    user.Email,
                    !user.IsActive);

                if (response.Success)
                {
                    ShowInfo($"Đã {action} tài khoản thành công!");
                    await LoadTaiKhoanAsync();
                }
                else
                {
                    MessageBox.Show($"Không thể {action}: {response.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                HandleError($"Lỗi {action} tài khoản", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task HandleDeleteUser(UserData user)
        {
            if (user == null || user.UserId <= 0) return;

            var confirm = MessageBox.Show(
                $"⚠️ XÓA vĩnh viễn tài khoản:\n\n{user.FullName} ({user.Email})?\n\nKHÔNG THỂ HOÀN TÁC!",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                Cursor = Cursors.WaitCursor;

                // Gọi API Delete
                var result = await _userApi.DeleteUserAsync(user.UserId);

                if (result.Success)
                {
                    ShowInfo($"✅ Xóa thành công: {user.FullName}");
                    await LoadTaiKhoanAsync(); // refresh danh sách user
                }
                else
                {
                    string errors = result.Errors != null ? string.Join("\n", result.Errors) : "";
                    MessageBox.Show($"❌ Xóa thất bại: {result.Message}\n{errors}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                HandleError($"Lỗi xóa tài khoản {user.FullName}", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }


        #endregion

        #region Button Handlers
        private async void btnThemGiaoVien_Click_1(object sender, EventArgs e)
        {
            try
            {
                using var form = new FormThemGiaoVien();
                if (form.ShowDialog() == DialogResult.OK)
                    await LoadTaiKhoanAsync();
            }
            catch (Exception ex)
            {
                HandleError("Lỗi mở form thêm giáo viên", ex);
            }
        }

        private async void btnThemHocSinh_Click_1(object sender, EventArgs e)
        {
            try
            {
                using var form = new FormThemHocSinh();
                if (form.ShowDialog() == DialogResult.OK)
                    await LoadTaiKhoanAsync();
            }
            catch (Exception ex)
            {
                HandleError("Lỗi mở form thêm học sinh", ex);
            }
        }
        #endregion

        #region Helpers
        private void ShowInfo(string message, string title = "Thông báo")
            => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void HandleError(string message, Exception ex)
        {
            Debug.WriteLine($"{message}: {ex}");
            MessageBox.Show($"{message}: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        
    }
}


