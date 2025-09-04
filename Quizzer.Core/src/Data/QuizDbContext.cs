using Microsoft.EntityFrameworkCore;
using Quizzer.Core.Entities;

namespace Quizzer.Core.Data;

/// <summary>
/// EF Core database context for the quiz application. Configures SQLite by default
/// and defines entity sets plus relationships and indexes.
/// </summary>
public class QuizDbContext : DbContext
{
    // Entity sets
    public DbSet<QuizSet> QuizSets => Set<QuizSet>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<UserProfile> Users => Set<UserProfile>();
    public DbSet<QuizAttempt> Attempts => Set<QuizAttempt>();
    public DbSet<AttemptAnswer> AttemptAnswers => Set<AttemptAnswer>();

    public string DbPath { get; }

    public QuizDbContext()
    {
        // Put the DB in the current working directory by default.
        var folder = Directory.GetCurrentDirectory();
        DbPath = Path.Combine(folder, "quiz.db");
    }

    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options)
    {
        DbPath = string.Empty;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            // Default to a local SQLite database when options not provided.
            options.UseSqlite($"Data Source={DbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        // Users: unique username
        model.Entity<UserProfile>()
            .HasIndex(u => u.Username)
            .IsUnique();


        // QuizSet: unique slug for stable identification
        model.Entity<QuizSet>()
            .HasIndex(q => q.Slug)
            .IsUnique();


        // Question -> QuizSet (many-to-one) with cascade on delete to remove questions with their set
        model.Entity<Question>()
            .HasOne(q => q.QuizSet)
            .WithMany(s => s.Questions)
            .HasForeignKey(q => q.QuizSetId)
            .OnDelete(DeleteBehavior.Cascade);


        // AnswerOption -> Question (many-to-one) cascade on delete
        model.Entity<AnswerOption>()
            .HasOne(o => o.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);


        // QuizAttempt -> UserProfile (many-to-one)
        model.Entity<QuizAttempt>()
            .HasOne(a => a.UserProfile)
            .WithMany(u => u.Attempts)
            .HasForeignKey(a => a.UserProfileId);


        // QuizAttempt -> QuizSet (many-to-one)
        model.Entity<QuizAttempt>()
            .HasOne(a => a.QuizSet)
            .WithMany()
            .HasForeignKey(a => a.QuizSetId);


        // AttemptAnswer -> QuizAttempt (many-to-one)
        model.Entity<AttemptAnswer>()
            .HasOne(aa => aa.QuizAttempt)
            .WithMany(a => a.Answers)
            .HasForeignKey(aa => aa.QuizAttemptId);


        // AttemptAnswer -> Question (many-to-one)
        model.Entity<AttemptAnswer>()
            .HasOne(aa => aa.Question)
            .WithMany()
            .HasForeignKey(aa => aa.QuestionId);


        // AttemptAnswer -> SelectedOption (many-to-one)
        model.Entity<AttemptAnswer>()
            .HasOne(aa => aa.SelectedOption)
            .WithMany()
            .HasForeignKey(aa => aa.SelectedOptionId);
    }
}
