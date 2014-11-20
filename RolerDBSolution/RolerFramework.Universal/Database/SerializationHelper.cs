using System.IO;
using System.Runtime.Serialization.Json;

namespace RolerFramework.Database
{
    internal class SerializationHelper
    {
        /// <summary>
        /// 将Json字符串反序列化成对象
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static T JsonDeserialize<T>(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return default(T);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (Stream stream = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(jsonString)))
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// 将某个对象序列化成Json字符串
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static string JsonSerializer(object target)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(target.GetType());
            using (Stream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, target);
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
