using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LiteDB;
using System;
using System.Diagnostics;

namespace ThePass.Views
{
    public class LockScreen : UserControl, IDisposable
    {
        public LockScreen()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void OnUnlockClick(object sender, RoutedEventArgs e)
        {
            string password = this.FindControl<TextBox>("Password")?.Text;

            if (String.IsNullOrWhiteSpace(password))
            {
                // Handle error
                return;
            }

            try
            {
                using (var db = new LiteDatabase($"filename=\"app.db\";password=\"{password}\""))
                {
                    var col = db.GetCollection<PasswordItem>("Passwords");
                }

                if((this.Parent as MainWindow) != null)
                {
                    (this.Parent as MainWindow)!.Password = password;
                } 
            }
            catch (Exception error)
            {
                Debug.WriteLine(error);
            }

           
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }
    }
}
