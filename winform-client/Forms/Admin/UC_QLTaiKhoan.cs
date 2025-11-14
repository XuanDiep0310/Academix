using Academix.WinApp.Api;
using Academix.WinApp.Models;
using Academix.WinApp.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Admin
{
    public partial class UC_QLTaiKhoan : UserControl
    {
        #region Fields
        private readonly UserApi _userApi;
        private const int ACTION_COLUMN_WIDTH = 60;
        #endregion

        #region Constructor & Initialization
        public UC_QLTaiKhoan()
        {
            InitializeComponent();
            _userApi = new UserApi(Config.Get("ApiSettings:BaseUrl"));
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            this.Load += UC_QLTaiKhoan_Load;
            dgvTaiKhoan.CellContentClick += DgvTaiKhoan_CellContentClick;
            dgvTaiKhoan.DataError += DgvTaiKhoan_DataError;
            dgvTaiKhoan.CellFormatting += DgvTaiKhoan_CellFormatting;
        }
        #endregion

        #region Load Data
        private async void UC_QLTaiKhoan_Load(object sender, EventArgs e)
        {
            await LoadTaiKhoanAsync();
        }

        private async Task LoadTaiKhoanAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                var users = await _userApi.GetAllUsersAsync();
                Debug.WriteLine($"Loaded {users.Count} users");

                if (users.Count > 0)
                {
                    dgvTaiKhoan.DataSource = users;
                    ConfigureColumns();
                    AddActionColumns();
                }
                else
                {
                    dgvTaiKhoan.DataSource = null;
                    ShowInfo("Không có dữ liệu tài khoản.");
                }
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
        #endregion

        #region DataGridView Configuration
        private void ConfigureColumns()
        {
            if (!IsDataGridViewReady()) return;

            LogColumns();
            SetColumnVisibility();
            ConfigureDataColumns();
            SetColumnOrder();
            ConfigureGridAppearance();
        }

        private bool IsDataGridViewReady()
        {
            bool isReady = dgvTaiKhoan?.Columns != null && dgvTaiKhoan.Columns.Count > 0;
            if (!isReady)
            {
                Debug.WriteLine("⚠️ DataGridView chưa sẵn sàng");
            }
            return isReady;
        }

        private void LogColumns()
        {
            Debug.WriteLine("=== DANH SÁCH CỘT ===");
            foreach (DataGridViewColumn col in dgvTaiKhoan.Columns)
            {
                Debug.WriteLine($"  {col.Name} | {col.HeaderText}");
            }
        }

        private void SetColumnVisibility()
        {
            string[] visibleColumns = { "FullName", "Email", "Role", "IsActive", "CreatedAt" };

            foreach (DataGridViewColumn col in dgvTaiKhoan.Columns)
            {
                col.Visible = visibleColumns.Contains(col.Name);
            }
        }

        private void ConfigureDataColumns()
        {
            ConfigureColumn("FullName", "Họ tên",
                autoSize: DataGridViewAutoSizeColumnMode.Fill,
                minWidth: 150);

            ConfigureColumn("Email", "Email",
                autoSize: DataGridViewAutoSizeColumnMode.Fill,
                minWidth: 200);

            ConfigureColumn("Role", "Vai trò",
                width: 120,
                backColor: Color.LightBlue,
                foreColor: Color.DarkBlue,
                alignment: DataGridViewContentAlignment.MiddleCenter);

            ConfigureColumn("IsActive", "Trạng thái",
                width: 100,
                alignment: DataGridViewContentAlignment.MiddleCenter);

            ConfigureColumn("CreatedAt", "Ngày tạo",
                width: 120,
                format: "dd/MM/yyyy",
                alignment: DataGridViewContentAlignment.MiddleCenter);
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

        private void SetColumnOrder()
        {
            try
            {
                int index = 0;
                SetColumnIndex("FullName", ref index);
                SetColumnIndex("Email", ref index);
                SetColumnIndex("Role", ref index);
                SetColumnIndex("IsActive", ref index);
                SetColumnIndex("CreatedAt", ref index);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Lỗi sắp xếp cột: {ex.Message}");
            }
        }

        private void SetColumnIndex(string columnName, ref int displayIndex)
        {
            var column = dgvTaiKhoan.Columns[columnName];
            if (column != null)
            {
                column.DisplayIndex = displayIndex++;
            }
        }

        private void ConfigureGridAppearance()
        {
            dgvTaiKhoan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvTaiKhoan.AllowUserToAddRows = false;
            dgvTaiKhoan.AllowUserToDeleteRows = false;
            dgvTaiKhoan.ReadOnly = true;
            dgvTaiKhoan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTaiKhoan.MultiSelect = false;
            dgvTaiKhoan.RowHeadersVisible = false;
            dgvTaiKhoan.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        }
        #endregion

        #region Action Columns
        private void AddActionColumns()
        {
            if (dgvTaiKhoan.Columns["btnEdit"] != null)
            {
                Debug.WriteLine("Action columns đã tồn tại");
                return;
            }

            AddButtonColumn("btnEdit", "Thao tác", "✏️");
            AddButtonColumn("btnLock", "", "🔒");
            AddButtonColumn("btnDelete", "", "🗑️");

            SetActionColumnOrder();
        }

        private void AddButtonColumn(string name, string headerText, string text)
        {
            var btnColumn = new DataGridViewButtonColumn
            {
                Name = name,
                HeaderText = headerText,
                Text = text,
                UseColumnTextForButtonValue = true,
                Width = ACTION_COLUMN_WIDTH,
                FlatStyle = FlatStyle.Flat
            };
            dgvTaiKhoan.Columns.Add(btnColumn);
        }

        private void SetActionColumnOrder()
        {
            int lastIndex = dgvTaiKhoan.Columns.Count;
            dgvTaiKhoan.Columns["btnEdit"].DisplayIndex = lastIndex - 3;
            dgvTaiKhoan.Columns["btnLock"].DisplayIndex = lastIndex - 2;
            dgvTaiKhoan.Columns["btnDelete"].DisplayIndex = lastIndex - 1;
        }
        #endregion

        #region Event Handlers
        private void DgvTaiKhoan_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Debug.WriteLine($"⚠️ DataGridView Error: Row {e.RowIndex}, Col {e.ColumnIndex}");
            Debug.WriteLine($"⚠️ {e.Exception.Message}");
            e.ThrowException = false;
            e.Cancel = true;
        }

        private void DgvTaiKhoan_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
                if (dgvTaiKhoan.Columns[e.ColumnIndex].Name != "IsActive") return;

                bool isActive = GetActiveStatus(e.Value);
                FormatActiveCell(e, isActive);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ CellFormatting Error: {ex.Message}");
                e.FormattingApplied = false;
            }
        }

        private bool GetActiveStatus(object value)
        {
            return value switch
            {
                bool b => b,
                string s => s.Contains("Hoạt động") || s.Contains("✓"),
                _ => false
            };
        }

        private void FormatActiveCell(DataGridViewCellFormattingEventArgs e, bool isActive)
        {
            // Hiển thị text thay vì checkbox
            e.Value = isActive ? "✓ Hoạt động" : "✗ Khóa";
            e.FormattingApplied = true;

            var backColor = isActive ? Color.LightGreen : Color.LightCoral;
            var foreColor = isActive ? Color.DarkGreen : Color.DarkRed;

            e.CellStyle.BackColor = backColor;
            e.CellStyle.ForeColor = foreColor;
            e.CellStyle.SelectionBackColor = backColor;
            e.CellStyle.SelectionForeColor = foreColor;
            e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
        }

        private async void DgvTaiKhoan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var user = dgvTaiKhoan.Rows[e.RowIndex].DataBoundItem as UserData;
            if (user == null) return;

            string columnName = dgvTaiKhoan.Columns[e.ColumnIndex].Name;

            switch (columnName)
            {
                case "btnEdit":
                    HandleEditUser(user);
                    break;
                case "btnLock":
                    await HandleLockUnlockUser(user);
                    break;
                case "btnDelete":
                    await HandleDeleteUser(user);
                    break;
            }
        }
        #endregion

        #region User Actions
        private void HandleEditUser(UserData user)
        {
            try
            {
                ShowInfo($"Chỉnh sửa: {user.FullName}\nEmail: {user.Email}", "Chỉnh sửa");

                // TODO: Implement edit form
                // using var form = new FormEditUser(user);
                // if (form.ShowDialog() == DialogResult.OK)
                //     await LoadTaiKhoanAsync();
            }
            catch (Exception ex)
            {
                HandleError("Lỗi khi mở form chỉnh sửa", ex);
            }
        }

        private async Task HandleLockUnlockUser(UserData user)
        {
            try
            {
                string action = user.IsActive ? "khóa" : "mở khóa";
                string message = user.IsActive
                    ? $"Khóa tài khoản '{user.FullName}'?\n\nTài khoản sẽ không thể đăng nhập."
                    : $"Mở khóa tài khoản '{user.FullName}'?\n\nTài khoản sẽ đăng nhập được.";

                if (ShowConfirm(message, $"Xác nhận {action}"))
                {
                    // TODO: Call API
                    // await _userApi.UpdateUserStatusAsync(user.Id, !user.IsActive);

                    ShowInfo($"Đã {action} tài khoản thành công!");
                    await LoadTaiKhoanAsync();
                }
            }
            catch (Exception ex)
            {
                HandleError("Lỗi cập nhật trạng thái", ex);
            }
        }

        private async Task HandleDeleteUser(UserData user)
        {
            try
            {
                string message = $"⚠️ CẢNH BÁO ⚠️\n\n" +
                    $"Xóa tài khoản '{user.FullName}'?\n\n" +
                    $"Email: {user.Email}\n" +
                    $"Vai trò: {user.Role}\n\n" +
                    $"KHÔNG THỂ HOÀN TÁC!";

                if (!ShowConfirm(message, "Xác nhận xóa", MessageBoxIcon.Warning))
                    return;

                if (!ShowConfirm("Bạn CHẮC CHẮN muốn xóa?", "Xác nhận lần cuối", MessageBoxIcon.Stop))
                    return;

                // TODO: Call API
                // await _userApi.DeleteUserAsync(user.Id);

                ShowInfo("Đã xóa tài khoản thành công!");
                await LoadTaiKhoanAsync();
            }
            catch (Exception ex)
            {
                HandleError("Lỗi khi xóa tài khoản", ex);
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
                {
                    await LoadTaiKhoanAsync();
                }
            }
            catch (Exception ex)
            {
                HandleError("Lỗi khi mở form thêm giáo viên", ex);
            }
        }

        private async void btnThemHocSinh_Click(object sender, EventArgs e)
        {
            try
            {
                using var form = new FormThemHocSinh();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadTaiKhoanAsync();
                }
            }
            catch (Exception ex)
            {
                HandleError("Lỗi khi mở form thêm giáo viên", ex);
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadTaiKhoanAsync();
        }
        #endregion

        #region Helper Methods
        private bool ShowConfirm(string message, string title = "Xác nhận",
            MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, icon) == DialogResult.Yes;
        }

        private void ShowInfo(string message, string title = "Thông báo")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HandleError(string message, Exception ex)
        {
            Debug.WriteLine($"{message}: {ex}");
            MessageBox.Show($"{message}: {ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
    }
}