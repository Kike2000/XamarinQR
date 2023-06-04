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
    public partial class CodigoQR : ContentPage
    {
        ZXingBarcodeImageView qr;
        public CodigoQR()
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
            List<DataModel.Evento> ListaEventos = new List<DataModel.Evento>();
            List<DataModel.Evento> miListaEventos = new List<DataModel.Evento>();
            List<DataModel.Registro> misRegistros = new List<DataModel.Registro>();
            using (NpgsqlConnection conn = new NpgsqlConnection(test2))
            {
                conn.Open();
                query = "SELECT \"EventoId\" FROM \"Registro\" WHERE \"ParticipanteId\" = '" + Application.Current.Properties["ParticipanteId"].ToString() + "';";

                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    var registro = new DataModel.Registro { EventoId = dr[0].ToString() };
                    misRegistros.Add(registro);
                }
                conn.Close();
                conn.Open();
                query = "SELECT \"Id\",\"Nombre\",\"HoraInicio\",\"HoraTermino\" FROM \"Evento\";";
                cmd = new NpgsqlCommand(query, conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var evento = new DataModel.Evento { Id = dr[0].ToString(), Nombre = dr[1].ToString(), HoraInicio = DateTime.Parse(dr[2].ToString()), HoraTermino = DateTime.Parse(dr[3].ToString()) };
                    ListaEventos.Add(evento);
                }
                foreach (var item in misRegistros)
                {
                    var evento = ListaEventos.FirstOrDefault(p => p.Id == item.EventoId);
                    miListaEventos.Add(evento);
                }
                misEventos.ItemsSource = miListaEventos.OrderBy(p => p.HoraInicio);
                conn.Close();
            }

        }
        async void misEventos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var evento = e.SelectedItem as DataModel.Evento;
            await Navigation.PushModalAsync(new RegistroConferencia(evento));
        }
    }
}