// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Libs.CustomConnector;

internal static class Utils
{
    public static void VerifyNotNull(object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException("Parameter is null");
        }
    }
}
