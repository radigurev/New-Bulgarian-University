using ForumGuard.Application.Workspaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForumGuard.Web.Pages;

/// <summary>
/// Renders the signed-in user's role landing page (SDD-FORUM-011 §2.3).
/// <para>Resolves the active <see cref="IRoleWorkspace"/> via <see cref="IRoleWorkspaceResolver"/> and
/// exposes its menu and available actions to the view.</para>
/// </summary>
[Authorize]
public sealed class DashboardModel : PageModel
{
    private readonly IRoleWorkspaceResolver _resolver;

    /// <summary>
    /// Initializes the page with the workspace resolver.
    /// </summary>
    /// <param name="resolver">The resolver that selects the role's workspace Strategy.</param>
    public DashboardModel(IRoleWorkspaceResolver resolver)
    {
        ArgumentNullException.ThrowIfNull(resolver);
        _resolver = resolver;
    }

    /// <summary>
    /// Gets the resolved workspace for the current principal.
    /// </summary>
    public IRoleWorkspace Workspace { get; private set; } = null!;

    /// <summary>
    /// Resolves the workspace for the signed-in user.
    /// </summary>
    public void OnGet()
    {
        Workspace = _resolver.Resolve(User);
    }
}
