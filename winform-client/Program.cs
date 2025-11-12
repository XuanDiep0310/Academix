namespace Academix.WinApp;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        Application.Run(new Forms.FormSignIn());
        //Application.Run(new Forms.Admin.FormMainAdmin());
        //Application.Run(new Forms.Student.FormMainStudent());
        //Application.Run(new Forms.Teacher.FormMainTeacher());
    }
}