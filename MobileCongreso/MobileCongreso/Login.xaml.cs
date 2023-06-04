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
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
            txtContraseña.Text = "SLADe2710*";
            txtCorreo.Text = "raymiusario@gmail.com";

            resourceImage.Source = ImageSource.FromResource("MobileCongreso.Images.slade.png");
        }
        public async void btnLogin_Clicked(object sender, EventArgs e)
        {
            var databaseUrl = "postgresql://postgres:lEIF3iOi0Fj9JhyKXh9j@containers-us-west-104.railway.app:7992/railway";
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var Role = "";
            var UserId = "";
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
            var ParticipanteId = "";
            using (NpgsqlConnection conn = new NpgsqlConnection(test2))
            {
                conn.Open();
                string query = "SELECT \"Id\",\"Nombre\",\"Procedencia\"  FROM \"Participante\" WHERE \"Email\" ='" + txtCorreo.Text + "' and \"Contraseña\" ='" + txtContraseña.Text + "';";

                //query = "Update \"Registro\" SET \"Asistencia\" = " + true + " WHERE \"Id\" = '" + Guid.Parse(result.Text) + "';";
                var nombre = "";
                var procedencia = "";
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    ParticipanteId = dr[0].ToString();
                    nombre = dr[1].ToString();
                    procedencia = dr[2].ToString();
                    conn.Close();
                    conn.Open();
                    query = "SELECT \"Id\" FROM \"AspNetUsers\" WHERE \"Email\" ='" + txtCorreo.Text + "';";
                    cmd = new NpgsqlCommand(query, conn);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        UserId = dr[0].ToString();
                    }
                    conn.Close();
                    conn.Open();
                    query = "SELECT \"RoleId\" FROM \"AspNetUserRoles\" WHERE \"UserId\" ='" + UserId + "';";
                    cmd = new NpgsqlCommand(query, conn);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        Role = dr[0].ToString();
                    }
                    conn.Close();
                    conn.Open();
                    query = "SELECT \"Name\" FROM \"AspNetRoles\" WHERE \"Id\" ='" + Role + "';";
                    cmd = new NpgsqlCommand(query, conn);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        Role = dr[0].ToString();
                    }
                    conn.Close();
                    Application.Current.Properties["Email"] = txtCorreo.Text;
                    Application.Current.Properties["ParticipanteId"] = ParticipanteId;
                    Application.Current.Properties["ParticipanteNombre"] = nombre;
                    Application.Current.Properties["Procedencia"] = procedencia;
                    if (Role == "Administrator")
                    {
                        await Navigation.PushModalAsync(new IndexAdministrador());
                    }
                    else
                    {
                        await Navigation.PushModalAsync(new MainPage());
                    }
                }
                else
                {

                    conn.Close();
                    conn.Open();
                    query = "SELECT \"Id\" FROM \"AspNetUsers\" WHERE \"Email\" ='" + txtCorreo.Text + "';";
                    cmd = new NpgsqlCommand(query, conn);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        Application.Current.Properties["Email"] = txtCorreo.Text;
                        Application.Current.Properties["ParticipanteNombre"] = txtCorreo.Text;
                        Application.Current.Properties["Procedencia"] = "México";
                        await Navigation.PushModalAsync(new MainPageAdministrador());
                    }
                    else
                    {
                        await DisplayAlert("Error", "El correo o contraseña proporcionado no es correcto", "OK");
                    }
                }
            }

        }
    }
}