using Android.App;
using Android.Util;
using AndroidX.Core.App;
using Health.Platforms.Android.Callbacks;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JObject = Java.Lang.Object;

namespace Health.Platforms.Android.Permissions
{
    internal class PermissionHandler
    {
        public static async Task<List<string>> Request(Java.Lang.Object permissions, CancellationToken cancellationToken = default)
        {
            try
            {

                if (Platform.CurrentActivity is not MainActivity activity)
                    return null;

                var whenCompletedSource = new TaskCompletionSource<JObject?>();
                _ = Task.Delay(MainActivity.MaxPermissionRequestDuration, cancellationToken)
                    .ContinueWith(_ => whenCompletedSource.TrySetResult(null), TaskScheduler.Default);


                new AlertDialog.Builder(activity)
                    .SetTitle("Health connect permission isn't granted")
                    .SetMessage("Do you want to allow permissions?")
                    .SetNegativeButton("Decline", (_, _) => whenCompletedSource.TrySetResult(null))
                    .SetPositiveButton("Allow", (_, _) => RequestPermission())
                    .Show();


                JObject? result = await whenCompletedSource.Task.ConfigureAwait(false);
                //return (ISet)result;
                return KotlinCallback.ConvertISetToList((ISet)result);

                void RequestPermission()
                => activity.RequestPermission(permissions, whenCompletedSource);
            }
            catch (Exception ex)
            {
                return null;
                // Handle exceptions as needed, e.g., logging or user notification
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
