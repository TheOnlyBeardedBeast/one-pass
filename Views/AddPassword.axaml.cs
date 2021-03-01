using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LiteDB;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ThePass.Views
{
    public class AddPassword : UserControl, IDisposable, INotifyPropertyChanged
    {
        private bool _HasError = false;
        private PasswordItem EditItem { get; set; }
        public bool HasError
        {
            get => _HasError;
            set { _HasError = value; NotifyPropertyChanged("HasError"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AddPassword()
        {
            InitializeComponent();
        }

        public AddPassword(PasswordItem editItem)
        {
            this.EditItem = editItem;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.DataContext = this;
            if (this.EditItem != null)
            {
                this.FindControl<TextBox>("Service").Text = this.EditItem.Service;
                this.FindControl<TextBox>("UserName").Text = this.EditItem.UserName;
            }
        }

        public void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            var item = this.GetInputs();

            if(item == null)
            {
                HasError = true;
                return;
            }


            string password = (this.Parent as MainWindow)?._Password;

            using (var db = new LiteDatabase($"filename=\"app.db\";password=\"{password}\""))
            {
                var col = db.GetCollection<PasswordItem>("Passwords");

                if (this.EditItem!=null)
                {
                    this.EditItem.Service = this.FindControl<TextBox>("Service").Text;
                    this.EditItem.UserName = this.FindControl<TextBox>("UserName").Text;
                    this.EditItem.Password = this.FindControl<TextBox>("Password").Text;

                    col.Update(this.EditItem);
                } else
                {
                    col.Insert(item);
                }
                
            }

            (this.Parent as MainWindow)?.NavigateTo(typeof(PasswordList));

        }

        private PasswordItem? GetInputs()
        {
            string service = this.FindControl<TextBox>("Service").Text;
            string userName = this.FindControl<TextBox>("UserName").Text;
            string password = this.FindControl<TextBox>("Password").Text;

            if(String.IsNullOrWhiteSpace(service) || String.IsNullOrWhiteSpace(userName) || String.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            return new PasswordItem { Service = service, UserName = userName, Password = password };
        }

        public void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            (this.Parent as MainWindow)?.NavigateTo(typeof(PasswordList));
        }

        public void OnErrorButtonClick(object sender, RoutedEventArgs e)
        {
            this.HasError = false;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }
    }
}
