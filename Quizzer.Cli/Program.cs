// Quizzer CLI entry point.
// This program runs a simple interactive quiz over the console.
// It initializes the database, seeds sample data, prompts the user,
// lets them choose a quiz set, administers questions, grades answers,
// records attempts, and shows basic history and averages.
using Quizzer.Core.Data;
using Quizzer.Core.Seed;
using Quizzer.Core.Services;
using Quizzer.Core.Entities;
using Microsoft.EntityFrameworkCore;

// Ensure Unicode characters (like ✅, ❌, ⏭️) render correctly.
Console.OutputEncoding = System.Text.Encoding.UTF8;

// Constants for UI behavior and limits
const char FIRST_OPTION_LETTER = 'A';
const int MAX_RECENT_ATTEMPTS = 10;
const string QUIT_COMMAND = "q";
const string SKIP_COMMAND = "s";

// Create a database context and ensure initial seed data exists.
using var db = new QuizDbContext();
SeedData.EnsureSeeded(db);

// Quiz engine encapsulates core quiz operations.
var engine = new QuizEngine(db);

// Step 1: Identify the user (create the profile if it doesn't exist yet).
WriteLine("\n=== Quizzer CLI (Step 1) ===\n");
Write("Enter your username: ");
var username = ReadNonEmpty();
var user = engine.GetOrCreateUser(username);

// Main loop: allow the user to take multiple quizzes until they quit.
while (true)
{
    // List available quiz sets with numeric indices for selection.
    Console.WriteLine("\nAvailable quiz sets:");
    var sets = engine.ListQuizSets().ToList();  // This loads all quiz sets into memory at once
    for (int i = 0; i < sets.Count; i++)
        Console.WriteLine($"  [{i + 1}] {sets[i].Title}  (slug: {sets[i].Slug})");

    // Let the user pick a quiz (or quit).
    Console.Write($"\nChoose a quiz by number (or '{QUIT_COMMAND}' to quit): ");
    var choice = Console.ReadLine()?.Trim();
    if (string.Equals(choice, QUIT_COMMAND, StringComparison.OrdinalIgnoreCase)) break;
    if (!int.TryParse(choice, out var idx) || idx < 1 || idx > sets.Count)
    {
        Console.WriteLine("Invalid selection.");
        continue;
    }

    var selected = sets[idx - 1];

    // Optional: limit the number of questions for this attempt.
    int? maxQ = null;
    Console.Write("Limit number of questions? (blank for all): ");
    var limitStr = Console.ReadLine()?.Trim();
    if (int.TryParse(limitStr, out var limit) && limit > 0) maxQ = limit;

    // Start an attempt and get a randomized subset of questions.
    var attempt = engine.StartAttempt(user.Id, selected.Id, maxQ);
    var questions = engine.GetRandomizedQuestions(selected.Id, attempt.TotalQuestions);

    Console.WriteLine($"\nStarting: {selected.Title}  (Questions: {questions.Count})\n");

    int qNum = 1;
    foreach (var q in questions)
    {
        // Print question text and enumerate options as A, B, C, ...
        Console.WriteLine($"Q{qNum}. {q.Text}");
        char letter = FIRST_OPTION_LETTER;  // start option labels at 'A'
        var letterToOption = new Dictionary<char, AnswerOption>();
        foreach (var opt in q.Options)
        {
            Console.WriteLine($"   {letter}) {opt.Text}");
            letterToOption[letter] = opt;
            letter++;
        }

        // Read the user's selection, allowing skip via 's'.
        AnswerOption? selectedOpt = null;
        while (selectedOpt == null)
        {
            Console.Write($"Your answer ({FIRST_OPTION_LETTER},{(char)(FIRST_OPTION_LETTER + 1)},{(char)(FIRST_OPTION_LETTER + 2)},...) or '{SKIP_COMMAND}' to skip: ");
            var ans = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(ans)) continue;
            if (ans.Equals(SKIP_COMMAND, StringComparison.OrdinalIgnoreCase)) break;
            var ch = char.ToUpperInvariant(ans[0]);
            if (letterToOption.TryGetValue(ch, out var opt)) selectedOpt = opt;
        }

        // Grade and record the answer if provided; otherwise, mark as skipped.
        if (selectedOpt != null)
        {
            var aa = engine.GradeAndRecord(attempt, q, selectedOpt);
            Console.WriteLine(aa.WasCorrect ? "✅ Correct!" : "❌ Incorrect.");
        }
        else
        {
            Console.WriteLine("⏭️  Skipped.");
        }

        Console.WriteLine();
        qNum++;
    }

    // Mark the attempt complete and summarize the score.
    engine.CompleteAttempt(attempt);

    Console.WriteLine($"Finished! Score: {attempt.Correct}/{attempt.TotalQuestions}  ({Percent(attempt.Correct, attempt.TotalQuestions):F1}%)\n");

    // Show recent history for this user + quiz
    var recent = db.Attempts  // This could be inefficient for large datasets
        .Include(a => a.QuizSet)
        .Where(a => a.UserProfileId == user.Id && a.QuizSetId == selected.Id)
        .OrderByDescending(a => a.StartedAt)
        .Take(MAX_RECENT_ATTEMPTS)
        .ToList();

    if (recent.Count > 0)
    {
        // Display last attempts with their scores and timestamps, plus average.
        Console.WriteLine("Last attempts on this quiz:");
        foreach (var a in recent)
        {
            var pct = Percent(a.Correct, a.TotalQuestions);
            Console.WriteLine($" - {a.StartedAt:u}  {a.Correct}/{a.TotalQuestions} ({pct:F1}%)");
        }
        var avg = recent.Average(a => Percent(a.Correct, a.TotalQuestions));
        Console.WriteLine($"Average over last {recent.Count}: {avg:F1}%\n");
    }
}

Console.WriteLine("Goodbye!\n");

// --- helpers ---
/// Reads a non-empty line from the console, prompting until provided.
static string ReadNonEmpty()
{
    while (true)
    {
        var s = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(s)) return s;
        Console.Write("Please enter a value: ");
    }
}

/// Computes percentage as double, guarding against divide-by-zero.
static double Percent(int correct, int total) => total > 0 ? (100.0 * correct / total) : 0.0;

// Convenience wrappers to keep top-level statements tidy.
static void Write(string s) => Console.Write(s);
static void WriteLine(string s) => Console.WriteLine(s);
