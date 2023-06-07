using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Google.Android.Material.Snackbar;

namespace CTAMobile
{
    [Activity(Label = "RegistrationPage")]
    public class RegistrationPage : Activity
    {
        EditText firstName, lastName, contactNum, emailAdd, password, address1, brgy, province, city, region;
        Button register, backtologin;
        List<string> existingAccs = new List<string>();
        string currentUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RegistrationPage);

            firstName = FindViewById<EditText>(Resource.Id.firstName);
            lastName = FindViewById<EditText>(Resource.Id.lastName);
            contactNum = FindViewById<EditText>(Resource.Id.contactNum);
            emailAdd = FindViewById<EditText>(Resource.Id.emailAdd);
            password = FindViewById<EditText>(Resource.Id.password);
            address1 = FindViewById<EditText>(Resource.Id.address1);
            brgy = FindViewById<EditText>(Resource.Id.brgy);
            province = FindViewById<EditText>(Resource.Id.province);
            city = FindViewById<EditText>(Resource.Id.city);
            region = FindViewById<EditText>(Resource.Id.region);

            register = FindViewById<Button>(Resource.Id.register);
            register.Click += this.Register;

            backtologin = FindViewById<Button>(Resource.Id.backtologin);
            backtologin.Click += delegate { StartActivity(typeof(LoginPage)); };

        }

        public void Register(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("Persist Security Info=False;User ID=admin;Password=123123;Initial Catalog=contact_tracing_system;Server=25.79.149.104");

            // validation if user account already exists           
            SqlCommand existing = new SqlCommand("select * from Customers", conn);
            conn.Open();
            SqlDataReader rdr = existing.ExecuteReader();
           
            if (rdr.HasRows)
            {
                rdr.Read();
                currentUser = rdr["email_address"].ToString();
                existingAccs.Add(currentUser);
                conn.Close();

                if (existingAccs.Contains(emailAdd.Text.ToString()) == true)
                {
                    Snackbar invalid = (Snackbar)Snackbar.Make(register, "Account already exists.", Snackbar.LengthIndefinite).SetDuration(1000);
                    invalid.Show();
                }
                else
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("insert into Customers(name_first, name_last, password, contact_num,email_address,home_address,region,province,city,brgy) values(@fname,@lname,@pass,@cnum,@email,@add1,@region,@prov,@city,@brgy)", conn);

                    cmd.Parameters.AddWithValue("@fname", firstName.Text);
                    cmd.Parameters.AddWithValue("@lname", lastName.Text);
                    cmd.Parameters.AddWithValue("@cnum", contactNum.Text);
                    cmd.Parameters.AddWithValue("@email", emailAdd.Text);
                    cmd.Parameters.AddWithValue("@pass", password.Text);
                    cmd.Parameters.AddWithValue("@add1", address1.Text);
                    cmd.Parameters.AddWithValue("@region", region.Text);
                    cmd.Parameters.AddWithValue("@brgy", brgy.Text);
                    cmd.Parameters.AddWithValue("@prov", province.Text);
                    cmd.Parameters.AddWithValue("@city", city.Text);

                    cmd.ExecuteNonQuery();
                    conn.Close();

                    Intent intent = new Intent(this, typeof(LoginPage));
                    StartActivity(intent);
                }
            }
            
            // registration
            /*conn.Open();
            SqlCommand cmd = new SqlCommand("insert into Customers(name_first, name_last, password, contact_num,email_address,home_address,region,province,city,brgy) values(@fname,@lname,@pass,@cnum,@email,@add1,@region,@prov,@city,@brgy)", conn);

            cmd.Parameters.AddWithValue("@fname", firstName.Text);
            cmd.Parameters.AddWithValue("@lname", lastName.Text);
            cmd.Parameters.AddWithValue("@cnum", contactNum.Text);
            cmd.Parameters.AddWithValue("@email", emailAdd.Text);
            cmd.Parameters.AddWithValue("@pass", password.Text);
            cmd.Parameters.AddWithValue("@add1", address1.Text);
            cmd.Parameters.AddWithValue("@region", region.Text);
            cmd.Parameters.AddWithValue("@brgy", brgy.Text);
            cmd.Parameters.AddWithValue("@prov", province.Text);
            cmd.Parameters.AddWithValue("@city", city.Text);

            cmd.ExecuteNonQuery();
            conn.Close();

            Intent intent = new Intent(this, typeof(LoginPage));
            StartActivity(intent);*/
        }
    }
}