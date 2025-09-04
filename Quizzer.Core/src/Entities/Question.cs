using System.Collections.Generic;


namespace Quizzer.Core.Entities;

/// <summary>
/// A prompt within a quiz set with a set of answer options.
/// Currently assumes a single correct option per question.
/// </summary>
public class Question
{
    public int Id { get; set; }
    public int QuizSetId { get; set; }
    /// <summary>
    /// Parent quiz set (EF Core navigation).
    /// </summary>
    public QuizSet? QuizSet { get; set; }

    /// <summary>
    /// The question text presented to the user.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    // Assume single-correct in Step 1 (we can support multi-correct later)
    /// <summary>
    /// The options the user can choose from.
    /// </summary>
    public List<AnswerOption> Options { get; set; } = new();
}
