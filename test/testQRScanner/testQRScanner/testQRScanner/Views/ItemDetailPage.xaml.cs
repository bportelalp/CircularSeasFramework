using System.ComponentModel;
using testQRScanner.ViewModels;
using Xamarin.Forms;

namespace testQRScanner.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}