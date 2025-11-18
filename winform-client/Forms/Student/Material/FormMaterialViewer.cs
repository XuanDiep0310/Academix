using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student.Material
{
    public class FormMaterialViewer : Form
    {
        private readonly string _localFilePath;
        private readonly string _title;
        private readonly WebBrowser _browser;

        public FormMaterialViewer(string localFilePath, string title)
        {
            _localFilePath = localFilePath;
            _title = title;

            Text = $"Xem tài liệu - {title}";
            StartPosition = FormStartPosition.CenterParent;
            Width = 900;
            Height = 600;

            _browser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true
            };

            Controls.Add(_browser);

            Load += FormMaterialViewer_Load;
            FormClosing += FormMaterialViewer_FormClosing;
        }

        private void FormMaterialViewer_Load(object? sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(_localFilePath))
                {
                    MessageBox.Show(
                        "Không tìm thấy tệp đã tải xuống để hiển thị.",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    Close();
                    return;
                }

                _browser.Navigate(_localFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể hiển thị tài liệu.\nChi tiết: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Close();
            }
        }

        private void FormMaterialViewer_FormClosing(object? sender, FormClosingEventArgs e)
        {
            try
            {
                if (File.Exists(_localFilePath))
                {
                    File.Delete(_localFilePath);
                }
            }
            catch
            {
                // nếu xóa thất bại thì bỏ qua, file temp sẽ được OS dọn sau
            }
        }
    }
}


