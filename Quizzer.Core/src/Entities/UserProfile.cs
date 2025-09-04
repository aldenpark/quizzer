using System;
using System.Collections.Generic;

namespace Quizzer.Core.Entities;

/// <summary>
/// Represents a user of the quiz system. Username is unique.
/// </summary>
public class UserProfile
{
    public int Id { get; set; }
    /// <summary>
    /// Unique username used to identify the user.
    /// </summary>
    public string Username { get; set; } = string.Empty; // unique

    /// <summary>
    /// Optional display name shown in summaries.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// UTC timestamp when the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Attempts made by this user (EF Core navigation).
    /// </summary>
    public List<QuizAttempt> Attempts { get; set; } = new();
}
