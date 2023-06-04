using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace MobileCongreso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConferenciaSeleccionada : ContentPage
    {
        public ConferenciaSeleccionada(DataModel.Evento evento)
        {
            InitializeComponent();
            Evento = evento;
            txtConferencia.Text = evento.Nombre;
        }
        public DataModel.Evento Evento { get; set; }
        public async void btnScannerQR_Clicked(object sender, EventArgs e)
        {

            txtResultado.Text = "";
            try
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                var result = await scanner.Scan();
                if (result != null)
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
                    string nombre = "";
                    string participante = "";
                    string evento = "";
                    bool asistencia = false;
                    using (NpgsqlConnection conn = new NpgsqlConnection(test2))
                    {
                        conn.Open();
                        string query = "SELECT \"Id\" ,\"ParticipanteId\", \"Asistencia\" FROM \"Registro\" WHERE \"ParticipanteId\" ='" + Guid.Parse(result.Text) + "' and \"EventoId\" ='" + Evento.Id + "';";

                        //query = "Update \"Registro\" SET \"Asistencia\" = " + true + " WHERE \"Id\" = '" + Guid.Parse(result.Text) + "';";

                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        NpgsqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            var registroId = dr[0].ToString();
                            asistencia = bool.Parse(dr[2].ToString());
                            participante = dr[1].ToString();
                            conn.Close();
                            if (asistencia == true)
                            {
                                txtResultado.Text = "Entrada Validada Anteriormente";
                                conn.Open();
                                query = "SELECT \"Nombre\" FROM \"Participante\" WHERE \"Id\" ='" + Guid.Parse(participante) + "';";
                                cmd = new NpgsqlCommand(query, conn);
                                dr = cmd.ExecuteReader();

                                if (dr.Read())
                                {
                                    nombre = dr[0].ToString();
                                    txtParticipante.Text = nombre;
                                }
                                conn.Close();

                                await DisplayAlert("Validación", "Entrada Validada Anteriormente", "OK");
                            }
                            else
                            {
                                conn.Open();
                                query = "Update \"Registro\" SET \"Asistencia\" = " + true + " WHERE \"Id\" = '" + Guid.Parse(registroId) + "';";

                                cmd = new NpgsqlCommand(query, conn);
                                dr = cmd.ExecuteReader();
                                dr.Read();
                                conn.Close();
                                conn.Open();
                                query = "SELECT \"Nombre\" FROM \"Participante\" WHERE \"Id\" ='" + Guid.Parse(participante) + "';";
                                cmd = new NpgsqlCommand(query, conn);
                                dr = cmd.ExecuteReader();

                                if (dr.Read())
                                {
                                    nombre = dr[0].ToString();
                                    txtParticipante.Text = nombre;
                                }
                                txtResultado.Text = "Entrada Validada Exitosamente";
                                conn.Close();
                            }
                        }
                        else
                        {
                            txtParticipante.Text = "";
                            await DisplayAlert("Error", "No existe un registro a esta conferencia.", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtParticipante.Text = "";
                await DisplayAlert("Error", "El código QR no es válido", "OK");
            }
        }

    }
}
