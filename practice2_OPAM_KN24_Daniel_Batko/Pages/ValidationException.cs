using System;
using System.Collections.Generic;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

internal class ValidationException : Exception
{
    public Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();

    public ValidationException(Dictionary<string, List<string>> errors)
    {
        Errors = errors;
    }
}
