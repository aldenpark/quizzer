using System;
using System.Collections.Generic;

namespace Quizzer.Core.Entities;

/// <summary>
/// A user's run through a quiz set. Tracks timing, question count,
/// correctness tally, and per-question answers.
/// </summary>
public class QuizAttempt
{
    public int Id { get; set; }

    public int UserProfileId { get; set; }
    /// <summary>
    /// The user who took the attempt.
    /// </summary>
    public UserProfile? UserProfile { get; set; }

    public int QuizSetId { get; set; }
    /// <summary>
    /// The quiz set attempted.
    /// </summary>
    public QuizSet? QuizSet { get; set; }

    /// <summary>
    /// UTC timestamp when the attempt started. Defaults to now.
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// UTC timestamp when the attempt completed (null if in-progress).
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// How many questions were presented for this attempt.
    /// </summary>
    public int TotalQuestions { get; set; }
    /// <summary>
    /// How many questions were answered correctly so far.
    /// </summary>
    public int Correct { get; set; }

    /// <summary>
    /// Answers recorded for this attempt (EF Core navigation).
    /// </summary>
    public List<AttemptAnswer> Answers { get; set; } = new();
}
