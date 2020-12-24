using System;
using System.Collections.Generic;
using System.Linq;
using Atc.Data.Models;

namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class LogItemHelper
    {
        public static LogKeyValueItem Create(LogCategoryType logCategoryType, string ruleName, string description)
        {
            return new LogKeyValueItem(logCategoryType, ruleName, ExtractAreaFromRuleName(ruleName), description);
        }

        private static string ExtractAreaFromRuleName(string ruleName)
        {
            var result = "#";
            var fieldInfos = typeof(ValidationRuleNameConstants).GetFields();
            var list = new List<Tuple<string, string>>();
            foreach (var fieldInfo in fieldInfos)
            {
                var o = fieldInfo.GetValue(null);
                string fieldValue = string.Empty;
                if (o != null)
                {
                    fieldValue = o.ToString();
                }

                list.Add(new Tuple<string, string>(fieldInfo.Name, fieldValue));
            }

            var tuple = list.FirstOrDefault(x => x.Item2 == ruleName);
            if (tuple == null || string.IsNullOrEmpty(tuple.Item2))
            {
                return result;
            }

            result = tuple.Item1;
            if (char.IsDigit(result[^2]))
            {
                result = result.Substring(0, result.Length - 2);
            }

            return result;
        }
    }
}