using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListDebuggerVisualizer
{
    public static class JObjectExtension
    {
        /// <summary>
        /// Gets the value of the property
        /// </summary>
        public static object GetValue(this JToken property)
        {
            Type type = property.GetValueType();
            return property.ToObject(type);
        }

        /// <summary>
        /// Gets the value type
        /// </summary>
        public static Type GetValueType(this JToken property)
        {
            JTokenType tokenType = property.Type;

            if (property.Type == JTokenType.Property)
            {
                return GetTypeFromToken(property.First().Type);
            }

            return GetTypeFromToken(tokenType);
        }

        /// <summary>
        /// Gets the type from the token
        /// </summary>
        public static Type GetTypeFromToken(JTokenType tokenType)
        {
            switch (tokenType)
            {
                case JTokenType.Integer:
                    return typeof(int);
                case JTokenType.Float:
                    return typeof(double);
                case JTokenType.Boolean:
                    return typeof(bool);
                case JTokenType.Date:
                    return typeof(DateTime);
                default:
                    return typeof(string);
            }
        }
    }
}
