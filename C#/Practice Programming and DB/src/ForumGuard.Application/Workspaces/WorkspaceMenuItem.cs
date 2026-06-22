namespace ForumGuard.Application.Workspaces;

/// <summary>
/// Represents a single ordered entry in a role workspace's navigation menu (SDD-FORUM-011).
/// <para>See <see cref="IRoleWorkspace"/> and <see cref="WorkspaceAction"/>.</para>
/// </summary>
/// <param name="Label">The display label shown to the user.</param>
/// <param name="Route">The relative route the menu item navigates to.</param>
/// <param name="Action">The action this menu item exposes.</param>
public sealed record WorkspaceMenuItem(string Label, string Route, WorkspaceAction Action);
