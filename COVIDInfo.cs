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
    [Activity(Label = "COVIDInfo")]
    public class COVIDInfo : Activity
    {
        Button back;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.COVIDInformation);

            back = FindViewById<Button>(Resource.Id.back);
            back.Click += delegate
            {
                string loggedonUser = Intent.GetStringExtra("User");
                Intent intent = new Intent(this, typeof(Dashboard));
                intent.PutExtra("User", loggedonUser);
                StartActivity(intent);
            };
        }
    }
}