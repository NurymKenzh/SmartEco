using Force.DeepCloner;

namespace SmartEco.Web.Extensions
{
    public static class ObjectExtension
    {
        public static T SetValue<T>(this T obj, string propertyName, object value)
        {
            obj.GetType().GetProperty(propertyName).SetValue(obj, value, null);
            return obj;
        }

        public static T CloneSetValue<T>(this T source, string propertyName, object value)
            => source.DeepClone().SetValue(propertyName, value);
    }
}
