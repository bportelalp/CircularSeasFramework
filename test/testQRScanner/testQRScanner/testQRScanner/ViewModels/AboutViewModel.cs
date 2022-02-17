using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using testQRScanner.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace testQRScanner.ViewModels
{
    public class AboutViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            ScanQR = new Command(async () => btnScan_Clicked());
        }

        public ICommand OpenWebCommand { get; }
        public ICommand ScanQR { get; }
        private string _texto;
        public string texto { get
            {
                return _texto;
            }
            set
            {
                _texto = value;
                NotifyPropertyChanged();
            }
        }

        private async void btnScan_Clicked()
        {
            try
            {
                var scanner = DependencyService.Get<IQrScanningService>();
                var result = await scanner.ScanAsync();
                if (result != null)
                {
                    texto = result;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}