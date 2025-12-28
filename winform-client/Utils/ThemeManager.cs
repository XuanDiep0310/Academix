using System.Drawing;

namespace Academix.WinApp.Utils
{
    public static class ThemeManager
    {
        // Màu chính - LightSkyBlue với các biến thể
        public static Color PrimaryColor => Color.FromArgb(135, 206, 250); // LightSkyBlue
        public static Color PrimaryDarkColor => Color.FromArgb(70, 130, 180); // SteelBlue
        public static Color PrimaryLightColor => Color.LightSkyBlue;
        
        // Màu phụ
        public static Color SecondaryColor => Color.FromArgb(99, 102, 241); // Indigo
        public static Color AccentColor => Color.FromArgb(59, 130, 246); // Blue
        
        // Màu nền
        public static Color BackgroundColor => Color.White;
        public static Color SidebarColor => Color.FromArgb(135, 206, 250);
        public static Color PanelColor => Color.White;
        
        // Màu text
        public static Color TextPrimary => Color.FromArgb(30, 30, 30);
        public static Color TextSecondary => Color.FromArgb(100, 100, 100);
        public static Color TextWhite => Color.White;
        
        // Màu button states
        public static Color ButtonDefault => PrimaryColor;
        public static Color ButtonHover => Color.FromArgb(100, 181, 246);
        public static Color ButtonActive => Color.White;
        public static Color ButtonTextDefault => TextWhite;
        public static Color ButtonTextActive => PrimaryDarkColor;
        
        // Màu border
        public static Color BorderColor => Color.FromArgb(200, 200, 200);
        public static Color BorderFocusColor => PrimaryDarkColor;
        
        // Shadow
        public static Color ShadowColor => Color.FromArgb(0, 0, 0, 30);
        
        // Success/Error colors
        public static Color SuccessColor => Color.FromArgb(34, 197, 94);
        public static Color ErrorColor => Color.FromArgb(239, 68, 68);
        public static Color WarningColor => Color.FromArgb(251, 191, 36);
        
        // Font
        public static Font DefaultFont => new Font("Segoe UI", 10F);
        public static Font TitleFont => new Font("Segoe UI Semibold", 12F);
        public static Font ButtonFont => new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold);
        public static Font LabelFont => new Font("Segoe UI", 10.2F);
    }
}

