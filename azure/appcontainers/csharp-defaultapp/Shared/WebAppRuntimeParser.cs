using System;
using System.Collections.Generic;
using System.Linq;

namespace defaultapp.Shared;

/**
 *  SUPPORTED RUNTIMES
    *
    * 'PHP|8.4' | 'PHP|8.3' | 'PHP|8.2' | 'PHP|8.1'
    * 'JAVA|17-java17' | 'JAVA|21-java21' | 'JAVA|11-java11' | 'JAVA|8-jre8'
    * 'JBOSSEAP|8-java21' | 'JBOSSEAP|8-java17' | 'JBOSSEAP|8-java11' | 'JBOSSEAP|7-java17' | 'JBOSSEAP|7-java11' | 'JBOSSEAP|7-java8'
    * 'TOMCAT|11.0-java21' | 'TOMCAT|11.0-java17' | 'TOMCAT|11.0-java11' | 'TOMCAT|10.1-java21' | 'TOMCAT|10.1-java17' | 'TOMCAT|10.1-java11' | 'TOMCAT|9.0-java21' | 'TOMCAT|9.0-java17' | 'TOMCAT|9.0-java11' | 'TOMCAT|9.0-jre8'
    * 'NODE|22-lts' | 'NODE|20-lts'
    * 'PYTHON|3.13' | 'PYTHON|3.12' | 'PYTHON|3.11' | 'PYTHON|3.10' | 'PYTHON|3.9'
    * 'DOTNETCORE|10.0' | 'DOTNETCORE|9.0' | 'DOTNETCORE|8.0'
 */
public class WebAppRuntimeParser
{
    private readonly Dictionary<string, string> _runtimes;
    public List<WebAppRuntime> Runtimes { get; set; }

    private readonly Dictionary<string, string[]> _validRuntimes = new()
    {
        ["PHP"] = ["8.4", "8.2", "8.1"],
        ["JAVA"] = ["21-java21", "17-java17", "11-java11", "8-jre8"],
        ["JBOSSEAP"] = ["8-java21", "8-java17", "8-java11", "7-java17", "7-java11", "7-java8"],
        ["TOMCAT"] = ["11.0-java21", "11.0-java17", "11.0-java11", "10.1-java21", "10.1-java17", "10.1-java11", "9.0-java11", "9.0-java21", "9.0-java17", "9.0-jre8"],
        ["NODE"] = ["22-lts", "20-lts"],
        ["PYTHON"] = ["3.13", "3.12", "3.11", "3.10", "3.9"],
        ["DOTNETCORE"] = ["10.0", "9.0", "8.0"],
    };
    
    public WebAppRuntimeParser(string runtimes)
    {
        if (string.IsNullOrWhiteSpace(runtimes)) throw new ArgumentNullException(nameof(runtimes));
        _runtimes = runtimes.Trim().Split(',')
            .Where(runtime => !string.IsNullOrWhiteSpace(runtime))
            .Select(runtime => runtime.Trim().Split("|").Where(x=> !string.IsNullOrWhiteSpace(x)).ToList())
            .Where(items => items.Count == 2)
            .ToDictionary(item => item[0], item => item[1]);
        
        if (_runtimes.Count == 0) throw new ArgumentException($"Invalid runtime '{runtimes}'");

        foreach (var runtime in _runtimes)
        {
            ValidateRuntime(runtime.Key, runtime.Value);
        }
        
        Runtimes = _runtimes.Select(x => new WebAppRuntime(x.Key, x.Value)).ToList();
    }

    private void ValidateRuntime(string runtime, string version)
    {
        if (GetValidRuntimes().ContainsKey(runtime))
        {
            GetValidRuntimes().TryGetValue(runtime, out var validVersions);
            if (!validVersions.Contains(version))
                throw new ArgumentException($"Unknown runtime version '{runtime}' '{version}'");
        }
        throw new ArgumentException($"Unknown runtime '{runtime}'");
    }

    protected virtual Dictionary<string, string[]> GetValidRuntimes()
    {
        return _validRuntimes;
    }
}

public class WebAppWindowsRuntimeParser(string runtimes) : WebAppRuntimeParser(runtimes)
{
    private readonly Dictionary<string, string[]> _validRuntimes = new()
    {
        ["PHP"] = ["5.6"],
        ["JAVA"] = ["1.8", "11", "17", "21"],
        ["JAVACONTAINER"] = ["TOMCAT", "TOMCAT"],
        ["JAVACONTAINERVERSION"] = ["9.0", "10.0", "10.1", "11.0", "SE"],
        ["NODE"] = ["~22", "~20", "~16"],
        ["PYTHON"] = ["2.7"],
        ["DOTNETCORE"] = ["v3.5", "v4.8", "v8.0", "v9.0", "v10.0"],
    };

    protected override Dictionary<string, string[]> GetValidRuntimes()
    {
        return _validRuntimes;
    }
}

public record WebAppRuntime(string Name, string Version)
{
    public override string ToString() => $"{Name}|{Version}";
}