using System;

namespace Engine.Application
{
    public static class MiscMethods
    {
        public static string NewGuid() => Guid.NewGuid().ToString();
    }
}
