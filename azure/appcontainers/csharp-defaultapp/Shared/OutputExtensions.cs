using System.Collections.Generic;
using System.Linq;
using Pulumi;

namespace csharp.Shared;

public static class OutputExtensions
{
    public static Output<Dictionary<string, T>> CreateDictOutput<T>(
        IDictionary<string, Output<T>> inputs)
    {
        var keys = inputs.Keys.ToArray();
        var values = inputs.Values.ToArray();
        return Output.All(values).Apply(objs =>
        {
            var result = new Dictionary<string, T>(keys.Length);
            for (var i = 0; i < keys.Length; i++)
            {
                result[keys[i]] = objs[i];
            }

            return result;
        });
    }
}