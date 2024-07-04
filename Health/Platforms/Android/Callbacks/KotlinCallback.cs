using Android.Runtime;
using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Health.MainPage;

namespace Health.Platforms.Android.Callbacks
{
    internal class KotlinCallback
    {
        private IHealthConnectClient healthConnectClient;
        public KotlinCallback(IHealthConnectClient client)
        {
            healthConnectClient = client;
        }

        public enum MyCoroutineSingletons
        {
            COROUTINE_SUSPENDED,
            UNDECIDED,
            RESUMED
        }
        public async Task<List<AggregationResultGroupedByDuration>> AggregateGroupByDuration(global::AndroidX.Health.Connect.Client.Request.AggregateGroupByDurationRequest request)
        {
            var tcs = new TaskCompletionSource<Java.Lang.Object>();
            Java.Lang.Object result = healthConnectClient.AggregateGroupByDuration(request,new Continuation(tcs, default));

            if (result is Java.Lang.Enum CoroutineSingletons)
            {
                MyCoroutineSingletons checkedEnum = (MyCoroutineSingletons)Enum.Parse(typeof(MyCoroutineSingletons), CoroutineSingletons.ToString());
                if (checkedEnum == MyCoroutineSingletons.COROUTINE_SUSPENDED)
                {
                    result = await tcs.Task;
                }
            }
            
            if (result is JavaList javaList)
            {
                List<AggregationResultGroupedByDuration> dotNetList = new List<AggregationResultGroupedByDuration>();
                for (int i = 0; i < javaList.Size(); i++)
                {
                    if (javaList.Get(i) is AggregationResultGroupedByDuration item)
                    {
                        dotNetList.Add(item);
                    }
                }
                return dotNetList;
            }
            return null;
        }


        public async Task<List<string>> GetGrantedPermissions()
        {
            var tcs = new TaskCompletionSource<Java.Lang.Object>();
            Java.Lang.Object result = healthConnectClient.PermissionController.GetGrantedPermissions(new Continuation(tcs, default));

            if (result is Java.Lang.Enum CoroutineSingletons)
            {
                MyCoroutineSingletons checkedEnum = (MyCoroutineSingletons)Enum.Parse(typeof(MyCoroutineSingletons), CoroutineSingletons.ToString());
                if (checkedEnum == MyCoroutineSingletons.COROUTINE_SUSPENDED)
                {
                    result = await tcs.Task;
                }
            }
            
            if (result is Kotlin.Collections.AbstractMutableSet abstractMutableSet)
            {
                Java.Util.ISet javaSet = abstractMutableSet.JavaCast<Java.Util.ISet>();
                List<string> listOfStrings = new List<string>();
                var iterator = javaSet.Iterator();

                while (iterator.HasNext)
                {
                    Java.Lang.Object element = iterator.Next();
                    listOfStrings.Add((string)element.JavaCast<Java.Lang.String>());
                }
                return listOfStrings;
            }
            return null;
        }
    }
}
