using System;

namespace GlucacxeScadaSystem.Helpers;

public  class LogNewLine
{
    public string LogAdd(string content)
    {
        return $"{content}{Environment.NewLine}";
    }
}