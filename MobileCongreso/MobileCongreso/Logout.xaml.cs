using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCongreso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Logout : ContentPage
    {
        public Logout()
        {
            InitializeComponent();
            personImage.Source = ImageSource.FromResource("MobileCongreso.Images.person.png");

            txtNombre.Text = Application.Current.Properties["ParticipanteNombre"].ToString();
            txtLocalizacion.Text = Application.Current.Properties["Procedencia"].ToString();
        }
        async void OnLogout(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Cerrar sesión", "¿Desea cerrar sesión?", "Si", "No");
            if (answer)
            {
                await Navigation.PushModalAsync(new Login());
                //var test = this.Navigation.ModalStack[0];
                //await test.Navigation.PopToRootAsync();
                //Navigation.RemovePage(this.Navigation.ModalStack[0]);
            }
        }
    }
}