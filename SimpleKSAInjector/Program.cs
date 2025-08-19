// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using SimpleKSAInjector;

const string KsaPath = "./ksa.dll"; // TODO: When KSA releases
var assemblies = new List<Assembly>();
Console.WriteLine("Searching for mod assemblies...");
var domain = AppDomain.CurrentDomain;
foreach (var file in Directory.EnumerateFiles("./mods", "*.dll", SearchOption.AllDirectories))
{
    try
    {
        var asm = Assembly.LoadFile(Path.GetFullPath(file));
        assemblies.Add(asm);
        Console.WriteLine($"Loaded {asm.GetName().Name}");
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error loading: {file}, {e}");
    }
}

domain.AssemblyResolve += MyResolveEventHandler;


foreach (var mod in assemblies)
{
    foreach (var method in mod.GetTypes().SelectMany(x => x.GetMethods())
                 .Where(x => x.IsStatic && x.GetCustomAttribute<ModPreEntryPointAttribute>() is not null && x.GetParameters().Length == 0))
    {
        method.Invoke(null, []);
    }
}

// Now we want to load all of KSA's dependencies


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
return;

Assembly? MyResolveEventHandler(object? sender, ResolveEventArgs args)
{
    if (AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName == args.Name) is { } assembly)
    {
        return assembly;
    }
    var assemblyName = new AssemblyName(args.Name).Name + ".dll";
    var culture = new AssemblyName(args.Name).CultureName;
    try
    {
        foreach (var file in Directory.EnumerateFiles($"./{culture}", assemblyName, SearchOption.AllDirectories))
        {
            return Assembly.LoadFile(file);
        }
    }
    catch (Exception e)
    {
        // ignore
    }
    
    foreach (var file in Directory.EnumerateFiles(".", assemblyName, SearchOption.AllDirectories))
    {
        return Assembly.LoadFile(Path.GetFullPath(file));
    }


    foreach (var file in Directory.EnumerateFiles($"./mods", assemblyName, SearchOption.AllDirectories))
    {
        return Assembly.LoadFile(file);
    }

    return null;
}