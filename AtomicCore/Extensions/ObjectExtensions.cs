//using System;
//using System.Linq.Expressions;
//using System.Reflection;

//namespace AtomicCore.Extensions
//{
//    /// <summary>
//    /// Object拓展方法
//    /// </summary>
//    public static class ObjectExtensions
//    {
//        #region Instance Entity Property Lambda

//        /// <summary>
//        /// 根据属性名获取属性值
//        /// </summary>
//        /// <typeparam name="T">对象类型</typeparam>
//        /// <param name="t">对象</param>
//        /// <param name="name">属性名</param>
//        /// <returns>属性的值</returns>
//        public static object GetPropertyValue<T>(this T t, string name)
//        {
//            Type type = t.GetType();
//            PropertyInfo pi = type.GetProperty(name);
//            if (null == pi)
//                throw new Exception($"该类型没有名为{name}的属性");

//            var param_obj = Expression.Parameter(typeof(T));
//            var param_val = Expression.Parameter(typeof(object));

//            // 原有的，非string类型会报转换object错误
//            // 转成真实类型，防止Dynamic类型转换成object
//            //var body_obj = Expression.Convert(param_obj, type);

//            //var body = Expression.Property(body_obj, p);
//            //var getValue = Expression.Lambda<Func<T, object>>(body, param_obj).Compile();

//            //转成真实类型，防止Dynamic类型转换成object
//            Expression<Func<T, object>> result = Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(param_obj, pi), typeof(object)), param_obj);
//            var getValue = result.Compile();

//            return getValue(t);
//        }

//        /// <summary>
//        /// 根据属性名称设置属性的值
//        /// </summary>
//        /// <typeparam name="T">对象类型</typeparam>
//        /// <param name="t">对象</param>
//        /// <param name="name">属性名</param>
//        /// <param name="value">属性的值</param>
//        public static void SetPropertyValue<T>(this T t, string name, object value)
//        {
//            Type type = t.GetType();
//            PropertyInfo pi = type.GetProperty(name);
//            if (pi == null)
//                throw new Exception($"该类型没有名为{name}的属性");

//            var param_obj = Expression.Parameter(type);
//            var param_val = Expression.Parameter(typeof(object));
//            var body_obj = Expression.Convert(param_obj, type);
//            var body_val = Expression.Convert(param_val, pi.PropertyType);

//            //获取设置属性的值的方法
//            var setMethod = pi.GetSetMethod(true);

//            //如果只是只读,则setMethod==null
//            if (setMethod != null)
//            {
//                var body = Expression.Call(param_obj, pi.GetSetMethod(), body_val);
//                var setValue = Expression.Lambda<Action<T, object>>(body, param_obj, param_val).Compile();
//                setValue(t, value);
//            }
//        }

//        /// <summary>
//        /// 获取类的属性名称
//        /// </summary>
//        /// <typeparam name="TSource"></typeparam>
//        /// <typeparam name="TProperty"></typeparam>
//        /// <param name="source"></param>
//        /// <param name="propery"></param>
//        /// <returns></returns>
//        public static string GetPropName<TSource, TProperty>(this TSource source,
//            Expression<Func<TSource, TProperty>> propery) where TSource : class
//        {
//            if (propery.Body is MemberExpression member)
//                return member.Member.Name;
//            if (propery.Body is UnaryExpression unary)
//                if (unary.Operand is MemberExpression operand)
//                    return operand.Member.Name;

//            throw new Exception($"'propery' is not MemberExpression Lambda");
//        }

//        /// <summary>
//        /// 获取类的属性反射类型
//        /// </summary>
//        /// <typeparam name="TSource"></typeparam>
//        /// <typeparam name="TProperty"></typeparam>
//        /// <param name="propertyLambda"></param>
//        /// <returns></returns>
//        /// <exception cref="ArgumentException"></exception>
//        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
//        Expression<Func<TSource, TProperty>> propertyLambda)
//        {
//            Type type = typeof(TSource);

//            MemberExpression member = propertyLambda.Body as MemberExpression;
//            if (member == null)
//                throw new ArgumentException(string.Format(
//                    "Expression '{0}' refers to a method, not a property.",
//                    propertyLambda.ToString()));

//            PropertyInfo propInfo = member.Member as PropertyInfo;
//            if (propInfo == null)
//                throw new ArgumentException(string.Format(
//                    "Expression '{0}' refers to a field, not a property.",
//                    propertyLambda.ToString()));

//            if (type != propInfo.ReflectedType &&
//                !type.IsSubclassOf(propInfo.ReflectedType))
//                throw new ArgumentException(string.Format(
//                    "Expresion '{0}' refers to a property that is not from type {1}.",
//                    propertyLambda.ToString(),
//                    type));

//            return propInfo;
//        }

//        /// <summary>
//        /// 获取类的属性信息
//        /// </summary>
//        /// <typeparam name="TSource"></typeparam>
//        /// <typeparam name="TProperty"></typeparam>
//        /// <param name="source"></param>
//        /// <param name="propertyLambda"></param>
//        /// <returns></returns>
//        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this TSource source,
//            Expression<Func<TSource, TProperty>> propertyLambda) where TSource : class
//        {
//            return GetPropertyInfo(propertyLambda);
//        }

//        /// <summary>
//        /// 获取类的属性名称 
//        /// </summary>
//        /// <typeparam name="TSource"></typeparam>
//        /// <typeparam name="TProperty"></typeparam>
//        /// <param name="source"></param>
//        /// <param name="propertyLambda"></param>
//        /// <returns></returns>
//        public static string NameOfProperty<TSource, TProperty>(this TSource source,
//            Expression<Func<TSource, TProperty>> propertyLambda) where TSource : class
//        {
//            PropertyInfo prodInfo = GetPropertyInfo(propertyLambda);
//            return prodInfo.Name;
//        }

//        #endregion
//    }
//}
