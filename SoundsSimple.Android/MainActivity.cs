using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace SoundsSimple.Droid
{
   [Activity(Label = "SoundsSimple", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
   public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
   {
      protected override void OnCreate(Bundle savedInstanceState)
      {
         TabLayoutResource = Resource.Layout.Tabbar;
         ToolbarResource = Resource.Layout.Toolbar;

         base.OnCreate(savedInstanceState);

         Xamarin.Essentials.Platform.Init(this, savedInstanceState);
         global::Xamarin.Forms.Forms.Init(this, savedInstanceState);


         AppCenter.Start("ca472a75-fe5e-4ef3-a685-b5ff4df09f41",
                            typeof(Analytics), typeof(Crashes));


         //if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
         //{
         //   // Provide an additional rationale to the user if the permission was not granted
         //   // and the user would benefit from additional context for the use of the permission.
         //   // For example if the user has previously denied the permission.
         //   Log.Info(TAG, "Displaying camera permission rationale to provide additional context.");

         //   var requiredPermissions = new String[] { Manifest.Permission.AccessFineLocation };
         //   Snackbar.Make(layout,
         //                  Resource.String.permission_location_rationale,
         //                  Snackbar.LengthIndefinite)
         //           .SetAction(Resource.String.ok,
         //                      new Action<View>(delegate (View obj)
         //                      {
         //                         ActivityCompat.RequestPermissions(this, requiredPermissions, REQUEST_LOCATION);
         //                      }
         //           )
         //   ).Show();
         //}
         //else
         //{
         //   ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, REQUEST_LOCATION);
         //}

         LoadApplication(new App());
      }



      public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
      {
         Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

         base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
      }

      //public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
      //{
      //   if (requestCode == REQUEST_LOCATION)
      //   {
      //      // Received permission result for camera permission.
      //      Log.Info(TAG, "Received response for Location permission request.");

      //      // Check if the only required permission has been granted
      //      if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
      //      {
      //         // Location permission has been granted, okay to retrieve the location of the device.
      //         Log.Info(TAG, "Location permission has now been granted.");
      //         Snackbar.Make(layout, Resource.String.permission_available_camera, Snackbar.LengthShort).Show();
      //      }
      //      else
      //      {
      //         Log.Info(TAG, "Location permission was NOT granted.");
      //         Snackbar.Make(layout, Resource.String.permissions_not_granted, Snackbar.LengthShort).Show();
      //      }
      //   }
      //   else
      //   {
      //      base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
      //   }
      //}

   }
}