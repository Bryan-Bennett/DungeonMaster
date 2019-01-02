using System.IO;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static string GetAssemblyPath(this Assembly assembly)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            return Uri.UnescapeDataString(uri.Path);
            
        }

        public static string GetAssemblyDirectory(this Assembly assembly)
        {
            return Path.GetDirectoryName(assembly.GetAssemblyPath());
        }
    }
}
