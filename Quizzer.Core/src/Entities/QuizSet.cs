using System.Collections.Generic;

namespace Quizzer.Core.Entities;

/// <summary>
/// Represents a collection of related questions (a quiz).
/// Users select a <see cref="QuizSet"/> to attempt in the CLI.
/// </summary>
public class QuizSet
{
    public int Id { get; set; }
    /// <summary>
    /// Stable unique identifier for the set (e.g., "csharp-basics").
    /// </summary>
    public string Slug { get; set; } = string.Empty; // e.g., "csharp-basics"

    /// <summary>
    /// Human-friendly title shown in menus.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description shown in summaries.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Questions that belong to this set (EF Core navigation).
    /// </summary>
    public List<Question> Questions { get; set; } = new();
}
