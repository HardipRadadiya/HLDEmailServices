using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarmingBlast.Repository
{
    public class BaseBLL
    {
        public static T SafeValue<T>(object value, bool IsAllowNull = false)
        {
            if (value == DBNull.Value || value == null)
            {
                if (typeof(T) == typeof(string))
                {
                    if (IsAllowNull)
                    {
                        return default(T);
                    }
                    else
                    {
                        return (T)Convert.ChangeType(string.Empty, typeof(T));
                    }
                }
                else
                {
                    return default(T);
                }
            }
            else if (value == null)
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), value.ToString());
            }
            if (value.GetType() == typeof(StringBuilder))
            {
                value = value.ToString();
            }
            if (value.GetType() == typeof(Guid))
            {
                return (T)Convert.ChangeType(Convert.ToString(value), typeof(T));
            }
            //else if (value.GetType() == typeof(StringBuilder) && typeof(T) == typeof(string))
            //{
            //    return (T)Convert.ChangeType(value.ToString(), typeof(T));
            //}
            if (value.GetType() == typeof(byte[]))
            {
                return (T)Convert.ChangeType(Convert.ToString(value), typeof(T));
            }
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
            {
                Type t = Nullable.GetUnderlyingType(typeof(T));
                return (T)Convert.ChangeType(value, t);

            }
            else
            {

                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
    }
}
