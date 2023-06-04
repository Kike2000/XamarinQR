using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace MobileCongreso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MisEvento : ContentPage
    {
        ZXingBarcodeImageView qr;
        public MisEvento()
        {
            InitializeComponent();
            qr = new ZXingBarcodeImageView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            qr.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
            qr.BarcodeOptions.Width = 500;
            qr.BarcodeOptions.Height = 500;
            qr.BarcodeValue = Application.Current.Properties["ParticipanteId"].ToString();

            stkQR.Children.Add(qr);
        }
    }
}