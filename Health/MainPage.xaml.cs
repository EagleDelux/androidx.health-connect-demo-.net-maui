using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Aggregate;
using AndroidX.Health.Connect.Client.Request;
using AndroidX.Health.Connect.Client.Records;
using Health.Platforms.Android.Callbacks;
using AndroidX.Health.Connect.Client.Time;
using AndroidX.Health.Connect.Client.Records.Metadata;
using Java.Time;
using AndroidX.Activity.Result.Contract;
using AndroidX.Activity.Result;
using AndroidX.Health.Connect.Client.Permission;
using Android.Runtime;
using Kotlin.Jvm.Internal;
using Java.Util;
using Health.Platforms.Android.Permissions;
using AndroidX.Health.Connect.Client.Units;


namespace Health
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        static ActivityResultContract activityResultContract = PermissionController.CreateRequestPermissionResultContract();
        private ActivityResultLauncher launcher = ((AndroidX.Activity.ComponentActivity)Platform.CurrentActivity).RegisterForActivityResult(activityResultContract, new ActivityResultCallback());
        public MainPage()
        {
            InitializeComponent();

        }

        private class ActivityResultCallback : Java.Lang.Object, IActivityResultCallback
        {
            public void OnActivityResult(Java.Lang.Object? result) // Make the parameter nullable
            {
                if (result is ISet PermissionGranted)
                {
                    ;
                }

            }
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {

            var zonedDateTime = ZonedDateTime.Now();
            string startTimeString = zonedDateTime.MinusDays(0).MinusHours(zonedDateTime.Hour).MinusMinutes(zonedDateTime.Minute).MinusSeconds(zonedDateTime.Second).ToString().Substring(0, 19) + "Z";
            Instant startTime = Instant.Parse(startTimeString);
 

            Instant endTime = Instant.Parse(ZonedDateTime.Now().ToString().Substring(0, 19) + "Z");
            ZoneOffset startZoneOffset = ZoneOffset.OfHours(2);
            ZoneOffset endZoneOffset = ZoneOffset.OfHours(1);
            Metadata metadata = new Metadata();
            StepsRecord stepsRecord = new StepsRecord(startTime,startZoneOffset, endTime, endZoneOffset, 1, metadata);
            DistanceRecord distanceRecord = new DistanceRecord(startTime, startZoneOffset, endTime, endZoneOffset, Length.InvokeMeters(11), metadata);

            //var PermissionsToGrant = new HashSet();
            List<string> PermissionsToGrant = new List<string>();
            PermissionsToGrant.Add(HealthPermission.GetReadPermission(Reflection.GetOrCreateKotlinClass(stepsRecord.Class)));
            PermissionsToGrant.Add(HealthPermission.GetReadPermission(Reflection.GetOrCreateKotlinClass(distanceRecord.Class)));

            


            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {

                //int availabilityStatus = HealthConnectClient.GetSdkStatus(Platform.CurrentActivity);
                

                ICollection<AggregateMetric> metrics = new List<AggregateMetric> { StepsRecord.CountTotal, DistanceRecord.DistanceTotal };

                ICollection<DataOrigin> dataOrginFilter = new List<DataOrigin>();

                AggregateGroupByDurationRequest request = new AggregateGroupByDurationRequest(metrics, TimeRangeFilter.After(startTime), Duration.OfDays(1), dataOrginFilter);

                
                try
                {
                    var healthConnectClient = new KotlinCallback(HealthConnectClient.GetOrCreate(Android.App.Application.Context));

                    List<string> GrantedPermissions = await healthConnectClient.GetGrantedPermissions();

                    List<string> MissingPermissions = PermissionsToGrant.Except(GrantedPermissions).ToList();
                    if (MissingPermissions.Count > 0)
                    {
                        GrantedPermissions = await PermissionHandler.Request(new HashSet(PermissionsToGrant));
                    }

                    bool allPermissionsGranted = PermissionsToGrant.All(permission => GrantedPermissions.Contains(permission));
                    if(allPermissionsGranted)
                    {
                        var Result = await healthConnectClient.AggregateGroupByDuration(request);
                        int? StepCountTotal = null;
                        Java.Lang.Object? DistanceTotal;

                        foreach (AggregationResultGroupedByDuration item in Result)
                        {

                            if (item.Result.Contains(StepsRecord.CountTotal))
                            {
                                StepCountTotal = (int)item.Result.Get(StepsRecord.CountTotal).JavaCast<Java.Lang.Number>();
                            }

                            if (item.Result.Contains(DistanceRecord.DistanceTotal))
                            {
                                DistanceTotal = item.Result.Get(DistanceRecord.DistanceTotal);
                            }
                        }
                        CounterBtn.Text = StepCountTotal.ToString();
                        SemanticScreenReader.Announce(CounterBtn.Text);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            
        }

    }

}
