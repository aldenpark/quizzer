namespace Quizzer.Core.Entities;

/// <summary>
/// Records the answer selected by a user for a specific question within
/// a given attempt, and whether it was correct.
/// </summary>
public class AttemptAnswer
{
    public int Id { get; set; }

    public int QuizAttemptId { get; set; }
    /// <summary>
    /// Owning attempt (EF Core navigation).
    /// </summary>
    public QuizAttempt? QuizAttempt { get; set; }

    public int QuestionId { get; set; }
    /// <summary>
    /// The question answered.
    /// </summary>
    public Question? Question { get; set; }

    public int SelectedOptionId { get; set; }
    /// <summary>
    /// The option the user chose.
    /// </summary>
    public AnswerOption? SelectedOption { get; set; }

    /// <summary>
    /// True if the selected option was correct at the time of answering.
    /// </summary>
    public bool WasCorrect { get; set; }
}
