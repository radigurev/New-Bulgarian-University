using ForumGuard.Application.Options;
using Microsoft.Extensions.Options;

namespace ForumGuard.Tests.Application.Options;

/// <summary>
/// Verifies the fail-fast field-level and cross-field validation of <see cref="ModerationOptions"/> and its defaults
/// per SDD-FORUM-023 §2.2, §2.3, §2.5, and §3.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-023")]
public sealed class ModerationOptionsValidatorTests
{
    private ModerationOptionsValidator _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new ModerationOptionsValidator();
    }

    private static ModerationOptions ValidOptions() => new()
    {
        ToxicityThreshold = 0.5f,
        ModelPath = "C:/models/model.zip",
        MaxCommentLength = 4000,
        ProfanityListPath = "C:/config/profanity.txt"
    };

    [Test]
    public void Validate_AllFieldsValid_ReturnsSuccess()
    {
        // Arrange
        ModerationOptions options = ValidOptions();

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.That(result.Succeeded, Is.True);
    }

    [Test]
    public void Validate_ToxicityThresholdBelowZero_ReturnsFailure()
    {
        // Arrange
        ModerationOptions options = ValidOptions();
        options.ToxicityThreshold = -0.01f;

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Failed, Is.True);
            Assert.That(result.FailureMessage, Does.Contain(ModerationConfigKeys.ToxicityThreshold));
        });
    }

    [Test]
    public void Validate_ToxicityThresholdAboveOne_ReturnsFailure()
    {
        // Arrange
        ModerationOptions options = ValidOptions();
        options.ToxicityThreshold = 1.5f;

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.That(result.Failed, Is.True);
    }

    [Test]
    public void Validate_ToxicityThresholdExactlyZero_ReturnsSuccess()
    {
        // Arrange
        ModerationOptions options = ValidOptions();
        options.ToxicityThreshold = 0.0f;

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.That(result.Succeeded, Is.True);
    }

    [Test]
    public void Validate_ToxicityThresholdExactlyOne_ReturnsSuccess()
    {
        // Arrange
        ModerationOptions options = ValidOptions();
        options.ToxicityThreshold = 1.0f;

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.That(result.Succeeded, Is.True);
    }

    [TestCase("")]
    [TestCase("   ")]
    public void Validate_ModelPathEmptyOrWhitespace_ReturnsFailure(string modelPath)
    {
        // Arrange
        ModerationOptions options = ValidOptions();
        options.ModelPath = modelPath;

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Failed, Is.True);
            Assert.That(result.FailureMessage, Does.Contain(ModerationConfigKeys.ModelPath));
        });
    }

    [TestCase("")]
    [TestCase("   ")]
    public void Validate_ProfanityListPathEmptyOrWhitespace_ReturnsFailure(string profanityListPath)
    {
        // Arrange
        ModerationOptions options = ValidOptions();
        options.ProfanityListPath = profanityListPath;

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Failed, Is.True);
            Assert.That(result.FailureMessage, Does.Contain(ModerationConfigKeys.ProfanityListPath));
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Validate_MaxCommentLengthZeroOrNegative_ReturnsFailure(int maxCommentLength)
    {
        // Arrange
        ModerationOptions options = ValidOptions();
        options.MaxCommentLength = maxCommentLength;

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Failed, Is.True);
            Assert.That(result.FailureMessage, Does.Contain(ModerationConfigKeys.MaxCommentLength));
        });
    }

    [Test]
    public void Validate_MaxCommentLengthExceeds4000_ReturnsFailure()
    {
        // Arrange
        ModerationOptions options = ValidOptions();
        options.MaxCommentLength = 4001;

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Failed, Is.True);
            Assert.That(result.FailureMessage, Does.Contain("4000"));
        });
    }

    [Test]
    public void Validate_MaxCommentLengthAt4000_ReturnsSuccess()
    {
        // Arrange
        ModerationOptions options = ValidOptions();
        options.MaxCommentLength = 4000;

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.That(result.Succeeded, Is.True);
    }

    [Test]
    public void Validate_MultipleInvalidFields_AggregatesAllFailures()
    {
        // Arrange
        ModerationOptions options = new()
        {
            ToxicityThreshold = 2.0f,
            ModelPath = string.Empty,
            MaxCommentLength = 0,
            ProfanityListPath = string.Empty
        };

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Failed, Is.True);
            Assert.That(result.Failures!.Count(), Is.EqualTo(4));
        });
    }

    [Test]
    public void Validate_FailureMessage_NamesOffendingFieldAndConstraint()
    {
        // Arrange
        ModerationOptions options = ValidOptions();
        options.ToxicityThreshold = -5.0f;

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.That(result.FailureMessage, Does.Contain("[0.0, 1.0]"));
    }

    [Test]
    public void Validate_ToxicityThresholdNaN_ReturnsFailure()
    {
        // Arrange
        ModerationOptions options = ValidOptions();
        options.ToxicityThreshold = float.NaN;

        // Act
        ValidateOptionsResult result = _sut.Validate(null, options);

        // Assert
        Assert.That(result.Failed, Is.True);
    }

    [Test]
    public void Validate_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        // Act
        TestDelegate act = () => _sut.Validate(null, null!);

        // Assert
        Assert.That(act, Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void ModerationOptions_DefaultToxicityThreshold_IsZeroPointFive()
    {
        // Arrange
        // Act
        ModerationOptions options = new();

        // Assert
        Assert.That(options.ToxicityThreshold, Is.EqualTo(0.5f));
    }

    [Test]
    public void ModerationOptions_DefaultMaxCommentLength_Is4000()
    {
        // Arrange
        // Act
        ModerationOptions options = new();

        // Assert
        Assert.That(options.MaxCommentLength, Is.EqualTo(4000));
    }
}
