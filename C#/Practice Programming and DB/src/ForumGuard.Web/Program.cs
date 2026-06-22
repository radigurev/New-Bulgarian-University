using ForumGuard.Application;
using ForumGuard.Infrastructure;
using ForumGuard.Infrastructure.Analysis;
using ForumGuard.Web.Authorization;
using ForumGuard.Web.Composition;
using Microsoft.AspNetCore.Authorization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
// Active analyzer: the trained NAS-BERT model (SDD-FORUM-020), as required by the assignment —
// every comment is analyzed by the model pre-trained on the seed corpus. The keyword profanity
// pre-filter remains the first link of the moderation chain (SDD-FORUM-021) as a fast, reliable
// guard ahead of the model. This requires Microsoft.ML.TorchSharp + the native libtorch-cpu backend
// (referenced by this project) and the trained Models/sentiment-model.zip copied to output.
builder.Services.AddNasBertCommentAnalyzer(builder.Configuration);
builder.Services.AddModerationOptions(builder.Configuration);
builder.Services.AddForumIdentity();
builder.Services.AddForumAuthorization();
builder.Services.AddWebServices();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Threads");
    options.Conventions.AuthorizePage("/Dashboard");
    options.Conventions.AuthorizeFolder("/Moderation", ForumPolicies.CanModerateComments);
    options.Conventions.AuthorizePage("/Admin/Users", ForumPolicies.CanManageAccounts);
    options.Conventions.AuthorizePage("/Admin/Moderators", ForumPolicies.CanManageModerators);
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Threads/Details");
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await StartupInitializer.InitializeDevelopmentAsync(app);
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

/// <summary>
/// Exposes the implicit Program entry point so an integration test host can reference it.
/// </summary>
public partial class Program
{
}
