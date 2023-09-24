using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Utilities.ExtentionMethods
{
    public static class TempDataEM
    {
        public static void Push<T>(this ITempDataDictionary tempData, string Key, T Value) where T : class
        {
            tempData[Key] = JsonConvert.SerializeObject(Value, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        }
        public static T Get<T>(this ITempDataDictionary tempData, string Key) where T : class
        {
            object obj;
            tempData.TryGetValue(Key, out obj);
            return obj==null?null:JsonConvert.DeserializeObject<T>((string)obj);
        }
    }
}
