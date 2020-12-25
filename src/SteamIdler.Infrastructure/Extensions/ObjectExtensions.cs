using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace System
{
    public static class ObjectExtensions
    {
        public static dynamic ToDynamic(this object obj)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            var type = obj.GetType();
            var properties = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor property in properties)
            {
                var value = property.GetValue(obj);
                expando.Add(property.Name, value);
            }

            return expando as ExpandoObject;
        }
    }
}
