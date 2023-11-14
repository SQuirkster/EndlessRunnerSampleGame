using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Netflix
{
    /**
     * Base abstract class for all InGame CL Events. Wraps all the CL calls to send data to CL.
     */
    public abstract class InGameEvent
    {
        public string name;

        protected InGameEvent(string name)
        {
            this.name = name;
        }

        public virtual string ToJson()
        {
            var classType = GetType();
            var mInfos = classType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                .Where(m => m.IsSpecialName && m.Name.StartsWith("get_"));
            var builder = new StringBuilder("{");
            var keyValues = new List<string>();
            foreach (MethodInfo method in mInfos)
            {
                var returnedValue = method.Invoke(this, new object[]{});
                if (returnedValue != null)
                {
                    var keyName = method.Name.Substring(4);
                    var jsonValue = "";
                    if (returnedValue is IList returnedArr)
                    {
                        var arrayValues = new List<string>();
                        foreach (var arrayValue in returnedArr)
                        {
                            arrayValues.Add(GetJsonKey(arrayValue));    
                        }
                        jsonValue = $"[{String.Join(",", arrayValues)}]";
                    }
                    else if (returnedValue is Dictionary<string, string> dict)
                    {
                        var dictBuilder = new StringBuilder("{");
                        var convertedArrValues = new List<string>();
                        foreach (var keyValue in dict)
                        {
                            convertedArrValues.Add($"\"{keyValue.Key}\":\"{EscapeJsonString(keyValue.Value)}\"");
                        }
                        dictBuilder.Append(string.Join(",", convertedArrValues) + "}");
                        jsonValue = dictBuilder.ToString();
                    }
                    else
                    {
                        var attributes = TypeDescriptor.GetProperties(this)[keyName].Attributes;
                        var defaultAttribute = (DefaultValueAttribute)attributes[typeof(DefaultValueAttribute)];
                        jsonValue = GetJsonKey(returnedValue, defaultAttribute);   
                    }

                    if (jsonValue != null)
                    {
                        keyValues.Add($"\"{keyName}\":{jsonValue}");     
                    }
                }
            }

            builder.Append(String.Join(",", keyValues) + "}");

            return builder.ToString();
        }

        private string EscapeJsonString(string jsonValue)
        {
            return jsonValue.Replace("\"", "\\\"");
        }

        private string GetJsonKey(object value, DefaultValueAttribute defaultValueAttribute = null)
        {
            if (defaultValueAttribute != null && value.Equals(defaultValueAttribute.Value))
            {
                return null;
            } 
            if (value is bool boolValue)
            {
                return boolValue.ToString().ToLower();
            } 

            if (value is string strValue)
            {
                return $"\"{EscapeJsonString(strValue)}\"";
            }
            
            if (value is Enum enumValue)
            {
                return $"\"{enumValue.ToString()}\"";
            }

            return value.ToString();
        }
    }
}