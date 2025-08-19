// See https://aka.ms/new-console-template for more information

using System.Reflection;
using SimpleKSAInjector;

const string KsaPath = "./ksa.dll"; // TODO: When KSA releases
var assemblies = new List<Assembly>();
foreach (var file in Directory.EnumerateFiles("./mods", "*.dll", SearchOption.AllDirectories))
{
    try
    {
        var asm = Assembly.LoadFile(Path.GetFullPath(file));
        assemblies.Add(asm);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error loading: {file}");
    }
}

foreach (var mod in assemblies)
{
    foreach (var method in mod.GetTypes().SelectMany(x => x.GetMethods())
                 .Where(x => x.IsStatic && x.GetCustomAttribute<ModPreEntryPointAttribute>() is not null && x.GetParameters().Length == 0))
    {
        method.Invoke(null, []);
    }
}

var ksa = Assembly.LoadFile(Path.GetFullPath(KsaPath));

foreach (var mod in assemblies)
{
    foreach (var method in mod.GetTypes().SelectMany(x => x.GetMethods())
                 .Where(x => x.IsStatic && x.GetCustomAttribute<ModPostEntryPointAttribute>() is not null && x.GetParameters().Length == 0))
    {
        method.Invoke(null, []);
    }
}
ksa.EntryPoint!.Invoke(null,[args]);