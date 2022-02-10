using System;

namespace Engine.Extensions;

public static class MiscMethods
{
    public static string NewGuid() => Guid.NewGuid().ToString();
}
