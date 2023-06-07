using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Data;
using Android.Content;
using System.Data.SqlClient;
using Google.Android.Material.Snackbar;

namespace CTAMobile
{
    [Activity(Label = "Contact Tracing Application", Theme = "@style/AppTheme", MainLauncher = true)]
    public class LoginPage : AppCompatActivity
    {
        EditText email, password;
        Button login, signup;
        TextView error;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.LoginPage);

            email = FindViewById<EditText>(Resource.Id.email);
            password = FindViewById<EditText>(Resource.Id.password);
            login = FindViewById<Button>(Resource.Id.login);
            signup = FindViewById<Button>(Resource.Id.signup);
            error = FindViewById<TextView>(Resource.Id.error);

            login.Click += this.Login;
            signup.Click += delegate { StartActivity(typeof(RegistrationPage)); };
        }

        public void Login(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("Persist Security Info=False;User ID=admin;Password=123123;Initial Catalog=contact_tracing_system;Server=25.79.149.104");
            try
            {
                conn.Open();
                SqlCommand cus_cmd = new SqlCommand("select * from Customers where email_address = '" + email.Text + "' and password = '" + password.Text + "'", conn);
                SqlDataReader cus_rdr = cus_cmd.ExecuteReader();

                if (cus_rdr.HasRows)
                {
                    cus_rdr.Read();
                    string emailAdd = cus_rdr["email_address"].ToString();
                    string pass = cus_rdr["password"].ToString();

                    if (password.Text != pass || email.Text != emailAdd)
                    {                      
                        error.Text = "Invalid login details. Please try again.";
                        
                    }
                    else
                    {
                        Intent intent = new Intent(this, typeof(Dashboard));
                        intent.PutExtra("User", email.Text.ToString());
                        StartActivity(intent);
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex} ");
                error.Text = "Something went wrong.";
            }
        }
     
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}