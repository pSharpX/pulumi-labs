using System;
using Pulumi;

namespace defaultapp;

public static class InputValidationExtensions
{
    public static Input<T> Ensure<T>(
        this Input<T> input,
        Func<T, bool> predicate,
        string message)
    {
        // Convert to Output<T>, validate, then return as Input<T>
        return input.ToOutput().Apply(v => !predicate(v) ? throw new ResourceException(message, resource: null) : v);
    }
}