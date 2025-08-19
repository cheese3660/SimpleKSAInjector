using SimpleKSAInjector;

namespace SimpleMod;

public class SimpleInjectedMod
{

    [ModPreEntryPoint]
    public static void PreStart()
    {
        
    }

    [ModPostEntryPoint]
    public static void PostStart()
    {
        File.WriteAllText("injected_test.txt", "Hello, I was injected!");
    }
}