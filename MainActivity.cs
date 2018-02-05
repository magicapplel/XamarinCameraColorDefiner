using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Widget;

namespace XamarinCamera
{
    [Activity(Label = "XamarinCamera", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private int count = 1;
        private ImageView imageView;
        private TextView colorNameTextView;
        private Tuple<string, Color>[] definedColors;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            definedColors = new[]
            {
                new Tuple<string, Color>("Black", Color.Rgb(0, 0, 0)),
                new Tuple<string, Color>("White", Color.Rgb(255, 255, 255))
            };

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var btnCamera = FindViewById<Button>(Resource.Id.btnCamera);
            imageView = FindViewById<ImageView>(Resource.Id.imageView);
            colorNameTextView = FindViewById<TextView>(Resource.Id.textViewDefinedColor);

            btnCamera.Click += BtnCamera_Click;
        }

        private void setupStart()
        {

        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            var bitmap = (Bitmap) data.Extras.Get("data");
            var meanColor = getMeanColor(bitmap);
            var colorName = defineColorName(meanColor);
            colorNameTextView.Text = colorName;

            imageView.SetImageBitmap(bitmap);
        }

        private string defineColorName(Color color)
        {
            int distance = int.MaxValue;
            var currentDefinedColor = default(Tuple<string, Color>);
            foreach (var definedColor in definedColors)
            {
                int currentDistance = getDistance(color, definedColor.Item2);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    currentDefinedColor = definedColor;
                }
            }

            return currentDefinedColor.Item1;
        }

        private int getDistance(Color color1, Color color2)
        {
            return (int) Math.Sqrt(Math.Pow(color1.R - color2.R, 2) + Math.Pow(color1.G - color2.G, 2)
                                                                    + Math.Pow(color1.B - color2.B, 2));
        }

        private Color getMeanColor(Bitmap bitmap)
        {
            int R, G, B;
            R = G = B = 0;
            var height = bitmap.Height;
            var width = bitmap.Width;

            var startX = height / 4;
            var endX = height / 4 * 3;
            var startY = height / 4;
            var endY = height / 4 * 3;

            var size = width * height;

            //bitmap.GetPixels(pixels, 0, width, 0, 0, width, height);
            //foreach (var pixel in pixels)
            //{
            //    R += Color.GetRedComponent(pixel);
            //    G += Color.GetGreenComponent(pixel);
            //    B += Color.GetBlueComponent(pixel);
            //}

            for (int i = startY; i < endY; i++)
            {
                for (int j = startX; j < endX; j++)
                {
                    int pixel = bitmap.GetPixel(i, j);
                    R += Color.GetRedComponent(pixel);
                    G += Color.GetGreenComponent(pixel);
                    B += Color.GetBlueComponent(pixel);
                }
            }

            R /= size;
            G /= size;
            B /= size;

            return Color.Rgb(R, G, B);
        }

        private void BtnCamera_Click(object sender, EventArgs e)
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, 0);
        }
    }
}