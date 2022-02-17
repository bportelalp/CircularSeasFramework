using System;
using System.Collections.Generic;
using System.ComponentModel;
using testQRScanner.Models;
using testQRScanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testQRScanner.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}