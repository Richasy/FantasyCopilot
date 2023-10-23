// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App;

/// <summary>
/// Type with an identity property.
/// </summary>
public class IdentityModelBase
{
    /// <summary>
    /// Identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description.
    /// </summary>
    public string Description { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is IdentityModelBase @base && Id == @base.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
