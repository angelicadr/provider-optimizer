using System;
using System.Collections.Generic;
using System.Linq;

public class DefectiveService
{
    private List<string> providers = new List<string>();

    public void AddProvider(string p)
    {
        providers.Add(p);
    }

    public string AssignProvider(string request)
    {
        // bad synchronous blocking, no validation, race conditions, wrong logic
        var provider = providers.First();
        providers.Remove(provider); // concurrency issue
        return provider;
    }
}
