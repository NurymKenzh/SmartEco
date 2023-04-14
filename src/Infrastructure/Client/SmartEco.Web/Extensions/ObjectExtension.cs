using Force.DeepCloner;

namespace SmartEco.Web.Extensions
{
    public static class ObjectExtension
    {

        public static T CloneSetValue<T>(this T source, string propertyName, object value)
            => source.DeepClone().SetValue(propertyName, value);

        private static T SetValue<T>(this T obj, string propertyName, object value)
        {
            if (obj is not null)
                obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, value, null);
            return obj;
        }
    }
}
