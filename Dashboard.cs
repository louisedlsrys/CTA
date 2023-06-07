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

namespace CTAMobile
{
    [Activity(Label = "Dashboard")]
    public class Dashboard : Activity
    {
        Button health, covidinfo, qr, logout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);

            health = FindViewById<Button>(Resource.Id.healthform);
            covidinfo = FindViewById<Button>(Resource.Id.covidinfo);
            qr = FindViewById<Button>(Resource.Id.viewqr);
            logout = FindViewById<Button>(Resource.Id.logout);

            health.Click += this.Health;
            covidinfo.Click += this.CovidInfo;
            qr.Click += this.viewQR;
            logout.Click += delegate { StartActivity(typeof(LoginPage)); };
        }

        public void Health(object sender, EventArgs e)
        {
            string loggedonUser = Intent.GetStringExtra("User");
            Intent intent = new Intent(this, typeof(HealthSurvey));
            intent.PutExtra("User", loggedonUser);
            StartActivity(intent);
        }

        public void CovidInfo(object sender, EventArgs e)
        {
            string loggedonUser = Intent.GetStringExtra("User");
            Intent intent = new Intent(this, typeof(COVIDInfo));
            intent.PutExtra("User", loggedonUser);
            StartActivity(intent);
        }

        public void viewQR(object sender, EventArgs e)
        {
            string loggedonUser = Intent.GetStringExtra("User");
            Intent intent = new Intent(this, typeof(ViewQR));
            intent.PutExtra("User", loggedonUser);
            StartActivity(intent);
        }
    }
}