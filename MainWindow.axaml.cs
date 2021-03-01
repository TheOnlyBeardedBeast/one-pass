using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TextCopy;
using ThePass.Views;
using System.Threading.Tasks;
using System.Threading;
using Avalonia.Threading;

namespace ThePass
{
    public class MainWindow : FluentWindow
    {
        public string _Password;
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        public string Password
        {
            get => _Password;
            set { 
                _Password = value;
                if (String.IsNullOrWhiteSpace(_Password))
                {
                    this.NavigateTo(typeof(LockScreen));
                    
                } else
                {
                    this.NavigateTo(typeof(PasswordList));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();

#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.Content = new LockScreen();
        }

        public void NavigateTo(Type view)
        {
            var prevContent = this.Content;

            if(view != null)
            {
                this.Content = Activator.CreateInstance(view);
            }

            if (view != null && view != typeof(LockScreen))
            {
                this.tokenSource.Cancel();
                this.ClearPassword();
            } else 
            {
                this.tokenSource.Cancel();
            }

            (prevContent as IDisposable)?.Dispose();
        }

        public void NavigateTo(Type view,params object?[]? args)
        {
            var prevContent = this.Content;

            if (view != null)
            {
                this.Content = Activator.CreateInstance(view, args);
            }

            if (view != null && view != typeof(LockScreen))
            {
                this.RestartLockTimer();
            }
            else
            {
                this.StopTimer();
            }

            (prevContent as IDisposable)?.Dispose();
        }

        public void RestartLockTimer()
        {
            this.tokenSource.Cancel();
            this.ClearPassword();
        }

        public void StopTimer()
        {
            this.tokenSource.Cancel();
        }

        private async Task ClearPassword()
        {
            Debug.WriteLine("clearer");
            this.tokenSource = new CancellationTokenSource();
            try
            {
                await Task.Delay(60000, tokenSource.Token);
                this.Password = "";
            }
            catch (Exception)
            {
                Debug.WriteLine("cancelled");
            }
            
        }

    }

        
}