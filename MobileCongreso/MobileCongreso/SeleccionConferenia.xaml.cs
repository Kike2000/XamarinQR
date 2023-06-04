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
    public partial class SeleccionConferenia : ContentPage
    {
        public SeleccionConferenia()
        {
            InitializeComponent();
            var databaseUrl = "postgresql://postgres:lEIF3iOi0Fj9JhyKXh9j@containers-us-west-104.railway.app:7992/railway";
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            var test2 = builder.ToString();
            string query = "";
            using (NpgsqlConnection conn = new NpgsqlConnection(test2))
            {
                conn.Open();
                query = "SELECT \"Id\",\"Nombre\",\"HoraInicio\",\"HoraTermino\" FROM \"Evento\";";

                //query = "Update \"Registro\" SET \"Asistencia\" = " + true + " WHERE \"Id\" = '" + Guid.Parse(result.Text) + "';";

                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                List<DataModel.Evento> myList = new List<DataModel.Evento>();

                while (dr.Read())
                {
                    var evento = new DataModel.Evento { Id = dr[0].ToString(), Nombre = dr[1].ToString(), HoraInicio = DateTime.Parse(dr[2].ToString()).ToUniversalTime(), HoraTermino = DateTime.Parse(dr[3].ToString()).ToUniversalTime() };
                    myList.Add(evento);
                }
                myListView.ItemsSource = myList.OrderBy(p => p.HoraInicio);
                conn.Close();
            }
        }
        public async void btnScannerQR_Clicked(object sender, EventArgs e)
        {

        }

        async void myListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var evento = e.SelectedItem as DataModel.Evento;
            await Navigation.PushModalAsync(new RegistroConferencia(evento));
        }
    }
}