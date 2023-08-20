namespace AtomicCore
{
    /// <summary>
    /// struct结构体原型拓展
    /// </summary>
    public static partial class StructExtensions
    {
        /// <summary>
        /// 判断当前的泛型类型结构体是否是默认值或null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }
    }
}
