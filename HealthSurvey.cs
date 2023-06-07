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
using System.IO;
using Android.Graphics;
using ZXing;
using ZXing.Common;
using Android.Content.PM;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Android.Graphics.Drawables;

namespace CTAMobile
{
    [Activity(Label = "HealthSurvey")]
    public class HealthSurvey : Activity
    {           
        string userLog;
        CheckBox feverCB, coughCB, sorethroatCB, shortbreathCB, bodyacheCB, redeyesCB, lossSTCB, nauseaCB, fatigueCB;
        int f, c, st, sb, ba, re, lst, n, fa;
        Button submitCB, donesurvey;
        ImageView image;

        //QR CODE
        private static int size = 660;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.HealthSurvey);

            // checkboxes
            feverCB = FindViewById<CheckBox>(Resource.Id.feverCB);
            feverCB.Click += delegate
            {
                if (feverCB.Checked)
                    f = 1;
                else
                    f = 0;
            };

            coughCB = FindViewById<CheckBox>(Resource.Id.coughCB);
            coughCB.Click += delegate
            {
                if (coughCB.Checked)
                    c = 1;
                else
                    c = 0;
            };

            sorethroatCB = FindViewById<CheckBox>(Resource.Id.sorethroatCB);
            sorethroatCB.Click += delegate
            {
                if (sorethroatCB.Checked)
                    st = 1;
                else
                    st = 0;
            };

            shortbreathCB = FindViewById<CheckBox>(Resource.Id.shortbreathCB);
            shortbreathCB.Click += delegate
            {
                if (shortbreathCB.Checked)
                    sb = 1;
                else
                    sb = 0;
            };

            bodyacheCB = FindViewById<CheckBox>(Resource.Id.bodyacheCB);
            bodyacheCB.Click += delegate
            {
                if (bodyacheCB.Checked)
                    ba = 1;
                else
                    ba = 0;
            };

            redeyesCB = FindViewById<CheckBox>(Resource.Id.redeyeCB);
            redeyesCB.Click += delegate
            {
                if (redeyesCB.Checked)
                    re = 1;
                else
                    re = 0;
            };

            lossSTCB = FindViewById<CheckBox>(Resource.Id.lossSTCB);
            lossSTCB.Click += delegate
            {
                if (lossSTCB.Checked)
                    lst = 1;
                else
                    lst = 0;
            };

            nauseaCB = FindViewById<CheckBox>(Resource.Id.nauseaCB);
            nauseaCB.Click += delegate
            {
                if (nauseaCB.Checked)
                    n = 1;
                else
                    n = 0;
            };

            fatigueCB = FindViewById<CheckBox>(Resource.Id.fatigueCB);
            fatigueCB.Click += delegate
            {
                if (fatigueCB.Checked)
                    fa = 1;
                else
                    fa = 0;
            };

            // buttons
            submitCB = FindViewById<Button>(Resource.Id.submitCB);
            submitCB.Click += this.submitCheckbox;

            donesurvey = FindViewById<Button>(Resource.Id.surveydone);
            donesurvey.Click += delegate
            {
                string loggedonUser = Intent.GetStringExtra("User");
                Intent intent = new Intent(this, typeof(Dashboard));
                intent.PutExtra("User", loggedonUser);
                StartActivity(intent);
            };

            // qr code           
            image = FindViewById<ImageView>(Resource.Id.barcodeImage);
        }
        
        public void submitCheckbox(object sneder, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("Persist Security Info=False;User ID=admin;Password=123123;Initial Catalog=contact_tracing_system;Server=25.79.149.104");

            try
            {
                conn.Open();
                string loggedonUser = Intent.GetStringExtra("User"); // pass logged on user's email from login page

                SqlCommand loggedOn = new SqlCommand("select * from Customers where email_address ='" + loggedonUser.ToString() + "';", conn); // retrieve userID from database
                SqlDataReader rdr = loggedOn.ExecuteReader();
                if (rdr != null)
                {
                    rdr.Read();
                    userLog = rdr["UserID"].ToString(); // store userID into variable
                }
                conn.Close();

                // insert into database
                conn.Open();
                SqlCommand cmd = new SqlCommand("insert into Health_Reports(UserID, fever, cough, sore_throat, short_breath,body_ache,red_eyes,loss_smell_taste,nausea_vomiting_diarrhoea,fatigue_weakness) values(@uid,@fever,@cough,@sorethroat,@shortbreath,@bodyache,@redeyes,@lossST,@nausea,@fw)", conn);

                cmd.Parameters.AddWithValue("@uid", userLog);
                cmd.Parameters.AddWithValue("@fever", f);
                cmd.Parameters.AddWithValue("@cough", c);
                cmd.Parameters.AddWithValue("@sorethroat", st);
                cmd.Parameters.AddWithValue("@shortbreath", sb);
                cmd.Parameters.AddWithValue("@bodyache", ba);
                cmd.Parameters.AddWithValue("@redeyes", re);
                cmd.Parameters.AddWithValue("@lossST", lst);
                cmd.Parameters.AddWithValue("@nausea", n);
                cmd.Parameters.AddWithValue("@fw", fa);

                cmd.ExecuteNonQuery();
                conn.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex} ");
            }

            string[] PERMISSIONS =
                {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE"
                };

            var permission = ContextCompat.CheckSelfPermission(this, "android.permission.WRITE_EXTERNAL_STORAGE");
            var permissionread = ContextCompat.CheckSelfPermission(this, "android.permission.READ_EXTERNAL_STORAGE");

            if (permission != Permission.Granted && permissionread != Permission.Granted)
                ActivityCompat.RequestPermissions(this, PERMISSIONS, 1);

            try
            {
                if (permission == Permission.Granted && permissionread == Permission.Granted)
                {
                    BitMatrix bitmapMatrix = null;
                    bitmapMatrix = new MultiFormatWriter().encode(userLog, BarcodeFormat.QR_CODE, size, size);

                    var width = bitmapMatrix.Width;
                    var height = bitmapMatrix.Height;
                    int[] pixelsImage = new int[width * height];

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            if (bitmapMatrix[j, i])
                                pixelsImage[i * width + j] = (int)Convert.ToInt64(0xff000000);
                            else
                                pixelsImage[i * width + j] = (int)Convert.ToInt64(0xffffffff);
                        }
                    }

                    Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                    bitmap.SetPixels(pixelsImage, 0, width, 0, 0, width, height);

                    var sdpath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                    var path = System.IO.Path.Combine(sdpath, "QRcode.jpg");
                    var stream = new FileStream(path, FileMode.Create);
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                    stream.Close();

                    image.SetImageBitmap(bitmap);
                }
                else
                {
                    Console.WriteLine("No Permission");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex} ");
            }
        }              
    }
}