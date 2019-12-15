namespace NetCoreSample.Extensions
{
    public static class ObjectExtensions
    {
        public static string GetPropertyValue<TObject>(this TObject obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null)?.ToString();
        }
    }
}
