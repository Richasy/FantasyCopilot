// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.App.Authorize;

/// <summary>
/// Apps that are already authorized.
/// </summary>
public sealed class AuthorizedApp
{
    /// <summary>
    /// Package ID of the app.
    /// </summary>
    public string PackageId { get; set; }

    /// <summary>
    /// Package name of the app.
    /// </summary>
    public string PackageName { get; set; }

    /// <summary>
    /// Time of the request.
    /// </summary>
    public DateTimeOffset RequestTime { get; set; }

    /// <summary>
    /// Requested permissions.
    /// </summary>
    public string[] Scopes { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AuthorizedApp app && PackageId == app.PackageId;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(PackageId);
}
