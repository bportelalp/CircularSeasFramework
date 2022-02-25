using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CircularSeasManager.Services;
using System.Threading.Tasks;
using ZXing.Mobile;

[assembly: Dependency(typeof(CircularSeasManager.Droid.NativeServices.QrService))]
namespace CircularSeasManager.Droid.NativeServices
{
    public  class QrService : IQrService
    {
        public async Task<string> ScanAsync()
        {
            var optionsDefault = new MobileBarcodeScanningOptions();
            var optionsCustom = new MobileBarcodeScanningOptions();

            var scanner = new MobileBarcodeScanner()
            {
                TopText = "Scan the QR Code of your spool",
                BottomText = "Please Wait",
            };

            var scanResult = await scanner.Scan(optionsCustom);
            return scanResult?.Text;
        }
    }
}