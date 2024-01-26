using System.Collections.Generic;

namespace DefaultNamespace
{
    public static class FlagManager
    {
        private static HashSet<string> Flags = new HashSet<string>();

        private static void Init()
        {
            Flags.Clear();
        }
        
        public static void Set(string flag)
        {
            Flags.Add(flag);
        }

        public static bool Check(string flag)
        {
            return Flags.Contains(flag);
        }
    }
}