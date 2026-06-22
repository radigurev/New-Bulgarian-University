using System.Security.Claims;
using ForumGuard.Application.Workspaces;
using ForumGuard.Domain.Authorization;
using ForumGuard.Tests.Application.Fakes;

namespace ForumGuard.Tests.Application.Workspaces;

/// <summary>
/// Verifies the role-keyed workspace resolution precedence (Administrator &gt; Moderator &gt; User), the anonymous
/// baseline, and the action/policy consistency of each workspace per SDD-FORUM-011 §2.3, §2.4.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-011")]
public sealed class RoleWorkspaceResolverTests
{
    private RoleWorkspaceResolver _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new RoleWorkspaceResolver(
        [
            new AdministratorWorkspace(),
            new ModeratorWorkspace(),
            new UserWorkspace()
        ]);
    }

    [Test]
    public void Resolve_AdministratorPrincipal_ReturnsAdministratorWorkspace()
    {
        // Arrange
        ClaimsPrincipal principal = PrincipalFactory.WithRoles(ForumRoles.Administrator);

        // Act
        IRoleWorkspace workspace = _sut.Resolve(principal);

        // Assert
        Assert.That(workspace, Is.InstanceOf<AdministratorWorkspace>());
    }

    [Test]
    public void Resolve_ModeratorPrincipal_ReturnsModeratorWorkspace()
    {
        // Arrange
        ClaimsPrincipal principal = PrincipalFactory.WithRoles(ForumRoles.Moderator);

        // Act
        IRoleWorkspace workspace = _sut.Resolve(principal);

        // Assert
        Assert.That(workspace, Is.InstanceOf<ModeratorWorkspace>());
    }

    [Test]
    public void Resolve_UserPrincipal_ReturnsUserWorkspace()
    {
        // Arrange
        ClaimsPrincipal principal = PrincipalFactory.WithRoles(ForumRoles.User);

        // Act
        IRoleWorkspace workspace = _sut.Resolve(principal);

        // Assert
        Assert.That(workspace, Is.InstanceOf<UserWorkspace>());
    }

    [Test]
    public void Resolve_ModeratorAndAdministratorPrincipal_ReturnsAdministratorWorkspace()
    {
        // Arrange
        ClaimsPrincipal principal = PrincipalFactory.WithRoles(
            ForumRoles.Moderator,
            ForumRoles.Administrator);

        // Act
        IRoleWorkspace workspace = _sut.Resolve(principal);

        // Assert
        Assert.That(workspace, Is.InstanceOf<AdministratorWorkspace>());
    }

    [Test]
    public void Resolve_NullPrincipal_ReturnsUserWorkspace()
    {
        // Arrange
        // Act
        IRoleWorkspace workspace = _sut.Resolve(null);

        // Assert
        Assert.That(workspace, Is.InstanceOf<UserWorkspace>());
    }

    [Test]
    public void Resolve_AnonymousPrincipal_ReturnsUserWorkspace()
    {
        // Arrange
        ClaimsPrincipal principal = PrincipalFactory.Anonymous();

        // Act
        IRoleWorkspace workspace = _sut.Resolve(principal);

        // Assert
        Assert.That(workspace, Is.InstanceOf<UserWorkspace>());
    }

    [Test]
    public void Resolve_NullPrincipal_DoesNotThrow()
    {
        // Arrange
        // Act
        TestDelegate act = () => _sut.Resolve(null);

        // Assert
        Assert.That(act, Throws.Nothing);
    }

    [Test]
    public void AvailableActions_AdministratorWorkspace_ExcludesCommentModeration()
    {
        // Arrange
        AdministratorWorkspace workspace = new();

        // Act
        IReadOnlySet<WorkspaceAction> actions = workspace.AvailableActions;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actions, Does.Not.Contain(WorkspaceAction.ViewModerationQueue));
            Assert.That(actions, Does.Not.Contain(WorkspaceAction.ApproveComment));
            Assert.That(actions, Does.Not.Contain(WorkspaceAction.RejectComment));
        });
    }

    [Test]
    public void AvailableActions_ModeratorWorkspace_ExcludesAccountAndModeratorManagement()
    {
        // Arrange
        ModeratorWorkspace workspace = new();

        // Act
        IReadOnlySet<WorkspaceAction> actions = workspace.AvailableActions;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actions, Does.Not.Contain(WorkspaceAction.ManageModerators));
            Assert.That(actions, Does.Not.Contain(WorkspaceAction.ManageAccounts));
        });
    }

    [Test]
    public void AvailableActions_UserWorkspace_ContainsOnlyAuthoringActions()
    {
        // Arrange
        UserWorkspace workspace = new();

        // Act
        IReadOnlySet<WorkspaceAction> actions = workspace.AvailableActions;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actions, Is.EquivalentTo(new[] { WorkspaceAction.CreateThread, WorkspaceAction.WriteComment }));
        });
    }

    [Test]
    public void AvailableActions_EachWorkspace_IsSubsetOfRolePermittedActions()
    {
        // Arrange
        HashSet<WorkspaceAction> userPermitted = [WorkspaceAction.CreateThread, WorkspaceAction.WriteComment];
        HashSet<WorkspaceAction> moderatorPermitted = [WorkspaceAction.ViewModerationQueue, WorkspaceAction.ApproveComment, WorkspaceAction.RejectComment];
        HashSet<WorkspaceAction> administratorPermitted = [WorkspaceAction.ManageModerators, WorkspaceAction.ManageAccounts];

        // Act
        bool userSubset = new UserWorkspace().AvailableActions.IsSubsetOf(userPermitted);
        bool moderatorSubset = new ModeratorWorkspace().AvailableActions.IsSubsetOf(moderatorPermitted);
        bool administratorSubset = new AdministratorWorkspace().AvailableActions.IsSubsetOf(administratorPermitted);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(userSubset, Is.True);
            Assert.That(moderatorSubset, Is.True);
            Assert.That(administratorSubset, Is.True);
        });
    }

    [Test]
    public void RoleConstants_AreExactlyExpectedValues_MatchIdentitySeed()
    {
        // Arrange
        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ForumRoles.User, Is.EqualTo("User"));
            Assert.That(ForumRoles.Moderator, Is.EqualTo("Moderator"));
            Assert.That(ForumRoles.Administrator, Is.EqualTo("Administrator"));
        });
    }

    [Test]
    public void Workspace_EachConcreteWorkspace_HasNonEmptyActionsAndCorrectRoleName()
    {
        // Arrange
        UserWorkspace user = new();
        ModeratorWorkspace moderator = new();
        AdministratorWorkspace administrator = new();

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(user.AvailableActions, Is.Not.Empty);
            Assert.That(moderator.AvailableActions, Is.Not.Empty);
            Assert.That(administrator.AvailableActions, Is.Not.Empty);
            Assert.That(user.RoleName, Is.EqualTo(ForumRoles.User));
            Assert.That(moderator.RoleName, Is.EqualTo(ForumRoles.Moderator));
            Assert.That(administrator.RoleName, Is.EqualTo(ForumRoles.Administrator));
        });
    }
}
