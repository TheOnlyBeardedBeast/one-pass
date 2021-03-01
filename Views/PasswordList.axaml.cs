using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LiteDB;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TextCopy;
using ThePass.Models;

namespace ThePass.Views
{
    public class PasswordList : UserControl, IDisposable
    {
        public RangeObservableCollection<PasswordItem> Passwords { get; set; } = new RangeObservableCollection<PasswordItem>();

        public PasswordList()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
        }

        

        private void LoadData()
        {
            string password = (this.Parent as MainWindow)?._Password;

            using (var db = new LiteDatabase($"filename=\"app.db\";password=\"{password}\""))
            {
                var col = db.GetCollection<PasswordItem>("Passwords");

                this.Passwords.AddRange(col.FindAll());
            }

            
        }

        private void OnCopyUserNameButtonClick(object sender, RoutedEventArgs e)
        {
            var username = ((sender as Button)?.DataContext as PasswordItem)?.UserName;

            if (!string.IsNullOrWhiteSpace(username))
            {
                ClipboardService.SetText(username);
            }

            (this.Parent as MainWindow)?.RestartLockTimer();
        }

        private void OnCopyPasswordButtonClick(object sender, RoutedEventArgs e)
        {
            var password = ((sender as Button)?.DataContext as PasswordItem)?.Password;

            if (!string.IsNullOrWhiteSpace(password))
            {
                ClipboardService.SetText(password);
            }

            (this.Parent as MainWindow)?.RestartLockTimer();

        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            (this.Parent as MainWindow)?.NavigateTo(typeof(AddPassword));
        }

        private void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            (this.Parent as MainWindow)?.NavigateTo(typeof(AddPassword), ((sender as Button)?.DataContext as PasswordItem));
        }

        private void OnLockClick(object sender, RoutedEventArgs e)
        {
            (this.Parent as MainWindow)!.Password = null;
        }

        private void OnRemoveButtonClick(object sender, RoutedEventArgs e)
        {
            string password = (this.Parent as MainWindow)?._Password;
            var id = ((sender as Button)?.DataContext as PasswordItem)?.Id;

            if (id != null)
            {
                using (var db = new LiteDatabase($"filename=\"app.db\";password=\"{password}\""))
                {
                    var col = db.GetCollection<PasswordItem>("Passwords");

                    col.DeleteMany(p => p.Id == id);
                    this.Passwords.RemoveAll(p => p.Id == id);
                }

                
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadData();
        }
    }
}
