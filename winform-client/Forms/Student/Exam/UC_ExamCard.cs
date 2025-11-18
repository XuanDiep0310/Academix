using Academix.WinApp.Models.Teacher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academix.WinApp.Forms.Student.MyResult
{
    public partial class UC_ExamCard : UserControl
    {
        private ExamDto? _exam;

        public UC_ExamCard()
        {
            InitializeComponent();
        }

        public void Bind(ExamDto exam)
        {
            _exam = exam;

            lblTenBaiKiemTra.Text = exam.Title;
            lblLopHoc.Text = exam.ClassName;
            lblMonHoc.Text = exam.ClassName;
            lblThoiLuong.Text = $"{exam.Duration} phút";
            lblThoiGianBatDau.Text = exam.StartTime.ToString("HH:mm dd/MM/yyyy");
            lblThoiGianKetThuc.Text = exam.EndTime.ToString("HH:mm dd/MM/yyyy");

            var now = DateTime.Now;
            var isOpen = exam.IsPublished
                && now >= exam.StartTime
                && now <= exam.EndTime;

            // Cập nhật giao diện theo trạng thái
            if (isOpen)
            {
                // Bài đang mở - màu xanh, nổi bật
                lblTrangThai.Text = "Đang mở";
                lblTrangThai.BackColor = Color.SkyBlue;
                lblTrangThai.ForeColor = Color.DarkBlue;
                lblTrangThaiDangMo.Text = "Bài kiểm tra đang mở. Bạn có thể làm bài ngay bây giờ!";
                lblTrangThaiDangMo.ForeColor = Color.LimeGreen;
                BackColor = Color.White;
                btnBatDauLamBai.Visible = true;
                btnBatDauLamBai.Enabled = true;
            }
            else
            {
                // Bài đã đóng - màu xám, mờ
                lblTrangThai.Text = "Đã đóng";
                lblTrangThai.BackColor = Color.LightGray;
                lblTrangThai.ForeColor = Color.DarkGray;

                string reason = "Bài kiểm tra đã kết thúc.";
                if (!exam.IsPublished)
                    reason = "Bài kiểm tra chưa được mở.";
                else if (now < exam.StartTime)
                    reason = "Bài kiểm tra chưa đến thời gian bắt đầu.";
                else if (now > exam.EndTime)
                    reason = "Bài kiểm tra đã hết thời gian.";

                lblTrangThaiDangMo.Text = reason;
                lblTrangThaiDangMo.ForeColor = Color.Gray;
                BackColor = Color.FromArgb(245, 245, 245); // Nền xám nhạt
                btnBatDauLamBai.Visible = false;
                btnBatDauLamBai.Enabled = false;
            }
        }
    }
}
