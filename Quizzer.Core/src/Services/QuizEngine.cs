using Quizzer.Core.Data;
using Quizzer.Core.Entities;

namespace Quizzer.Core.Services;

/// <summary>
/// Encapsulates quiz operations such as listing sets, starting attempts,
/// generating randomized questions, grading answers, and recording results.
/// </summary>
public class QuizEngine
{
    private readonly QuizDbContext _db;
    private readonly Random _rng;

    /// <summary>
    /// Creates a new engine. A seed can be provided to make shuffling deterministic
    /// in tests or reproducible runs.
    /// </summary>
    public QuizEngine(QuizDbContext db, int? seed = null)
    {
        _db = db;
        _rng = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    /// <summary>
    /// Finds an existing user by username or creates one if missing.
    /// </summary>
    public UserProfile GetOrCreateUser(string username, string? displayName = null)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            user = new UserProfile { Username = username, DisplayName = displayName };
            _db.Users.Add(user);
            _db.SaveChanges();
        }
        return user;
    }

    /// <summary>
    /// Loads a quiz set by slug, projecting to plain entities with options loaded.
    /// </summary>
    public QuizSet? GetQuizSetBySlug(string slug) =>
        _db.QuizSets
           .Where(s => s.Slug == slug)
           .Select(s => new QuizSet
           {
               Id = s.Id,
               Slug = s.Slug,
               Title = s.Title,
               Description = s.Description,
               Questions = s.Questions
                    .Select(q => new Question
                    {
                        Id = q.Id,
                        QuizSetId = q.QuizSetId,
                        Text = q.Text,
                        Options = q.Options.ToList()
                    })
                    .ToList()
           })
           .FirstOrDefault();

    /// <summary>
    /// Returns all quiz sets.
    /// </summary>
    public IEnumerable<QuizSet> ListQuizSets() => _db.QuizSets.ToList();

    /// <summary>
    /// Starts a new attempt for the given user and quiz. Optionally caps the number of
    /// questions via <paramref name="maxQuestions"/>.
    /// </summary>
    public QuizAttempt StartAttempt(int userId, int quizSetId, int? maxQuestions = null)
    {
        var totalQs = _db.Questions.Count(q => q.QuizSetId == quizSetId);
        if (totalQs == 0) throw new InvalidOperationException("Quiz has no questions.");

        var attempt = new QuizAttempt
        {
            UserProfileId = userId,
            QuizSetId = quizSetId,
            TotalQuestions = maxQuestions.HasValue ? Math.Min(maxQuestions.Value, totalQs) : totalQs,
            Correct = 0
        };
        _db.Attempts.Add(attempt);
        _db.SaveChanges();
        return attempt;
    }

    /// <summary>
    /// Retrieves questions for a quiz, shuffles their order, optionally limits the count,
    /// then shuffles options within each question.
    /// </summary>
    public List<Question> GetRandomizedQuestions(int quizSetId, int? limit = null)
    {
        var questions = _db.Questions
            .Where(q => q.QuizSetId == quizSetId)
            .Select(q => new Question
            {
                Id = q.Id,
                QuizSetId = q.QuizSetId,
                Text = q.Text,
                Options = q.Options.ToList()
            })
            .ToList();

        Shuffle(questions);
        if (limit.HasValue) questions = questions.Take(limit.Value).ToList();

        // Shuffle options for each question
        foreach (var q in questions)
            Shuffle(q.Options);

        return questions;
    }

    /// <summary>
    /// Grades a selected option, persists the per-question answer,
    /// and increments the attempt's correct tally when appropriate.
    /// </summary>
    public AttemptAnswer GradeAndRecord(QuizAttempt attempt, Question q, AnswerOption selected)
    {
        bool correct = selected.IsCorrect;
        var aa = new AttemptAnswer
        {
            QuizAttemptId = attempt.Id,
            QuestionId = q.Id,
            SelectedOptionId = selected.Id,
            WasCorrect = correct
        };
        _db.AttemptAnswers.Add(aa);
        if (correct) { attempt.Correct += 1; }
        _db.SaveChanges();
        return aa;
    }

    /// <summary>
    /// Marks an attempt complete and records the completion timestamp.
    /// </summary>
    public void CompleteAttempt(QuizAttempt attempt)
    {
        attempt.CompletedAt = DateTime.UtcNow;
        _db.SaveChanges();
    }

    /// <summary>
    /// In-place Fisher–Yates shuffle for lists using the engine's RNG.
    /// </summary>
    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
