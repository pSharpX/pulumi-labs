using System;
using Pulumi;

namespace defaultapp;

public static class OutputValidationExtensions
{
    public static Output<T> Ensure<T>(this Output<T> output, Func<T, bool> predicate, string message)
    {
        return output.Apply(v => !predicate(v) ? throw new ResourceException(message, resource: null) : v);
    }
}