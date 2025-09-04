namespace Quizzer.Core.Entities;

/// <summary>
/// A possible answer for a question. Exactly one option is marked correct
/// for the current single-correct model.
/// </summary>
public class AnswerOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    /// <summary>
    /// Owning question (EF Core navigation).
    /// </summary>
    public Question? Question { get; set; }

    /// <summary>
    /// The text displayed for this option.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// True if this option is the correct answer for the question.
    /// </summary>
    public bool IsCorrect { get; set; }
}
