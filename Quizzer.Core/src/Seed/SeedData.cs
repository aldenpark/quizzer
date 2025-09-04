using Quizzer.Core.Data;
using Quizzer.Core.Entities;

namespace Quizzer.Core.Seed;

/// <summary>
/// Provides initial data for development/demo use. Creates the database
/// if missing and inserts a couple of quiz sets with questions/options
/// when no data is present.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Ensures the database exists and contains seed data.
    /// </summary>
    public static void EnsureSeeded(QuizDbContext db)
    {
        db.Database.EnsureCreated();

        if (db.QuizSets.Any()) return; // already seeded

        var csharp = new QuizSet
        {
            Slug = "csharp-basics",
            Title = "C# Basics",
            Description = "Intro to C# syntax and concepts",
            Questions = new()
            {
                new Question
                {
                    Text = "Which keyword declares a constant in C#?",
                    Options = new()
                    {
                        new AnswerOption { Text = "const", IsCorrect = true },
                        new AnswerOption { Text = "static", IsCorrect = false },
                        new AnswerOption { Text = "readonly", IsCorrect = false },
                        new AnswerOption { Text = "var", IsCorrect = false },
                    }
                },
                new Question
                {
                    Text = "What type is the result of 1 / 2 in C# when both operands are int?",
                    Options = new()
                    {
                        new AnswerOption { Text = "int (0)", IsCorrect = true },
                        new AnswerOption { Text = "double (0.5)", IsCorrect = false },
                        new AnswerOption { Text = "decimal (0.5)", IsCorrect = false },
                        new AnswerOption { Text = "float (0.5)", IsCorrect = false },
                    }
                },
                new Question
                {
                    Text = "Which of the following defines a string interpolation?",
                    Options = new()
                    {
                        new AnswerOption { Text = "$\"Hello {name}\"", IsCorrect = true },
                        new AnswerOption { Text = "string.Format(\"Hello {0}\", name)", IsCorrect = false },
                        new AnswerOption { Text = "\"Hello \" + name", IsCorrect = false },
                        new AnswerOption { Text = "@\"Hello {name}\"", IsCorrect = false },
                    }
                }
            }
        };

        var bash = new QuizSet
        {
            Slug = "linux-bash",
            Title = "Linux Bash",
            Description = "Basic Bash commands",
            Questions = new()
            {
                new Question
                {
                    Text = "Which command lists files, including hidden ones?",
                    Options = new()
                    {
                        new AnswerOption { Text = "ls -la", IsCorrect = true },
                        new AnswerOption { Text = "list --all", IsCorrect = false },
                        new AnswerOption { Text = "dir /a", IsCorrect = false },
                        new AnswerOption { Text = "show -h", IsCorrect = false },
                    }
                },
                new Question
                {
                    Text = "What's the command to print current working directory?",
                    Options = new()
                    {
                        new AnswerOption { Text = "pwd", IsCorrect = true },
                        new AnswerOption { Text = "cwd", IsCorrect = false },
                        new AnswerOption { Text = "whereami", IsCorrect = false },
                        new AnswerOption { Text = "path", IsCorrect = false },
                    }
                }
            }
        };

        // Insert created quiz sets and persist.
        db.QuizSets.AddRange(csharp, bash);
        db.SaveChanges();
    }
}
