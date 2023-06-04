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
    public partial class RegistroConferencia : ContentPage
    {
        Button Button;
        Button CancelarButton;
        Label Label;
        DataModel.Registro RegistroExistente;
        public RegistroConferencia(DataModel.Evento evento)
        {
            InitializeComponent();
            var test=Navigation.NavigationStack.Count();
            Evento = evento;
            NombreEvento.Text = Evento.Nombre;
            HoraTerminoEvento.Text = Evento.HoraTermino.ToString();
            HoraInicioEvento.Text = Evento.HoraInicio.ToString();
            Button = new Button
            {
                Text = "Registrarse",
            };
            CancelarButton = new Button
            {
                Text = "Eliminar Registro",
            };
            Button.Clicked += btnScannerQR_Clicked;
            CancelarButton.Clicked += eliminarRegistro_Clicked;
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
            bool ExisteCruce = false;
            using (NpgsqlConnection conn = new NpgsqlConnection(test2))
            {
                conn.Open();

                query = "SELECT \"Id\" FROM \"Registro\" WHERE \"EventoId\"= '" + Evento.Id + "' and \"ParticipanteId\" = '" + Application.Current.Properties["ParticipanteId"].ToString() + "';";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                List<DataModel.Evento> allList = new List<DataModel.Evento>();
                List<DataModel.Evento> myList = new List<DataModel.Evento>();
                List<DataModel.Registro> myListRegistro = new List<DataModel.Registro>();
                var tieneValor = false;
                if (dr.Read())
                {
                    Label = new Label
                    {
                        Text = "Ya está Registrado"
                    };
                    RegistroExistente = new DataModel.Registro { Id = dr[0].ToString() };
                    tieneValor = true;
                    Registro.Children.Add(Label);
                    Registro.Children.Add(CancelarButton);
                }
                else
                {
                    conn.Close();
                    conn.Open();
                    query = "SELECT \"EventoId\" FROM \"Registro\" WHERE \"ParticipanteId\" = '" + Application.Current.Properties["ParticipanteId"].ToString() + "';";
                    cmd = new NpgsqlCommand(query, conn);
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var nuevoRegistro = new DataModel.Registro { EventoId = dr[0].ToString() };
                        myListRegistro.Add(nuevoRegistro);
                    }
                    conn.Close();
                    conn.Open();
                    query = "SELECT \"Id\", \"Nombre\", \"HoraInicio\", \"HoraTermino\" FROM \"Evento\";";
                    cmd = new NpgsqlCommand(query, conn);
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var nuevoEvento = new DataModel.Evento { Id = dr[0].ToString(), Nombre = dr[1].ToString(), HoraInicio = DateTime.Parse(dr[2].ToString()).ToUniversalTime(), HoraTermino = DateTime.Parse(dr[3].ToString()).ToUniversalTime() };
                        allList.Add(nuevoEvento);
                    }
                    foreach (var item in allList)
                    {
                        if (myListRegistro.Any(p => p.EventoId == item.Id))
                        {
                            if (item.Id != Evento.Id)
                            {
                                myList.Add(item);
                            }
                        }
                    }
                    foreach (var item in myList)
                    {
                        if (Evento.HoraInicio >= item.HoraInicio && Evento.HoraInicio < item.HoraTermino)
                        {
                            ExisteCruce = true;
                            Label = new Label
                            {
                                Text = "Ya Existe un Registro en el mismo tiempo"
                            };
                            Registro.Children.Add(Label);
                            tieneValor = true;
                            break;
                        }
                    }
                }
                if (!tieneValor)
                {
                    Registro.Children.Add(Button);
                }

                conn.Close();
            }

        }
        public DataModel.Evento Evento { get; set; }
        public async void btnScannerQR_Clicked(object sender, EventArgs e)
        {
            var registro = new DataModel.Registro
            {
                ParticipanteId = Application.Current.Properties["ParticipanteId"].ToString(),
                Asistencia = false,
                EventoId = Evento.Id,
                HoraDeRegistro = DateTime.UtcNow
            };
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
                query = "INSERT INTO \"Registro\" (\"Id\",\"ParticipanteId\",\"Asistencia\",\"EventoId\") " +
                                           "VALUES ('" + Guid.NewGuid() + "','" + Application.Current.Properties["ParticipanteId"].ToString() + "','" + false + "','" + Evento.Id + "');";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                try
                {
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    conn.Close();
                    var MyAppsFirstPage = new MainPage();
                    await DisplayAlert("Evento Registrado", "Registro exitoso a la conferencia", "OK");
                    Application.Current.MainPage = new NavigationPage(MyAppsFirstPage);
                }
                catch (Exception ex)
                {

                }
            }
        }
        public async void eliminarRegistro_Clicked(object sender, EventArgs e)
        {
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
                try
                {
                    query = "DELETE FROM \"Registro\" WHERE \"Id\"='" + RegistroExistente.Id + "'";
                }
                catch (Exception ex)
                {

                }
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                NpgsqlDataReader dr = cmd.ExecuteReader(); 
                conn.Close();
                var MyAppsFirstPage = new MainPage();
                await DisplayAlert("Registro eliminado Registrado", "Registro eliminado a la conferencia", "OK");
                Application.Current.MainPage = new NavigationPage(MyAppsFirstPage);
            }

        }
    }
}