using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Google.Android.Material.Snackbar;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using ZXing;
using ZXing.Common;

namespace CTAMobile
{
    [Activity(Label = "ViewQR")]
    public class ViewQR : Activity
    {
        Button show;
        ImageView qrCode;
        string userLog, declaredHealth;
        private static int size = 660;

        TextView error;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewQR);

            qrCode = FindViewById<ImageView>(Resource.Id.qrCode);
            error = FindViewById<TextView>(Resource.Id.error);
            show = FindViewById<Button>(Resource.Id.viewQR);
            show.Click += this.showQR;
        }

        public void showQR(object sender, EventArgs e)
        {
            // retrieve user id from database table "customers"
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

            
                // validate if user id exists in database table "health reports"
                conn.Open();
                SqlCommand currUser = new SqlCommand("select * from Health_Reports where UserID = @user", conn);
                currUser.Parameters.AddWithValue("@user", userLog);

                SqlDataReader rdrCurrUser = currUser.ExecuteReader();
                if (rdrCurrUser != null)
                {
                    rdrCurrUser.Read();
                    declaredHealth = rdrCurrUser["UserID"].ToString(); // store userID into variable
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                error.Text = "No QR Code available. Please fill in Health Declaration Form.";
                Console.WriteLine($"Exception {ex} ");
            }

            // generate qr code 
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
                    bitmapMatrix = new MultiFormatWriter().encode(declaredHealth, BarcodeFormat.QR_CODE, size, size);

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

                    qrCode.SetImageBitmap(bitmap);
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