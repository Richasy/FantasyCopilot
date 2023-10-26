// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Libs.CustomConnector;

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
