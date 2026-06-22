# ForumGuard — Cross-Reference Map (Spec ↔ Planned Source)

> Project: **ForumGuard** — Forum with automatic moderation of rude comments (CSCB634 practice project)
> Status: **GREENFIELD** — no source code exists yet. Every path below is **PLANNED / TARGET**;
> nothing under `src/` or `tests/` has been created. Specs are **authoritative** (they define what
> WILL be built).
> Last updated: 2026-06-21

This map is bidirectional traceability between the authoritative SDD specs and the planned
implementation artifacts. Keep it in sync whenever a spec's *Key Files* section or the planned
solution structure (`CLAUDE.md` §2) changes.

## Canonical Spec ID Map (authoritative)

| Spec ID | Category | Title |
|---|---|---|
| SDD-FORUM-001 | core | Comment Submission & Auto-Publishing |
| SDD-FORUM-002 | core | Moderation Queue & Decisions |
| SDD-FORUM-003 | core | Registration, Login & Account Activation |
| SDD-FORUM-004 | core | Moderator Role Management |
| SDD-FORUM-010 | domain | Comment Entity & Lifecycle |
| SDD-FORUM-011 | domain | Roles & Authorization Model |
| SDD-FORUM-020 | infrastructure | Comment Analysis (NAS-BERT) |
| SDD-FORUM-021 | infrastructure | Moderation Pipeline (Chain of Responsibility) |
| SDD-FORUM-022 | infrastructure | Data Access (EF Core + MSSQL) |
| SDD-FORUM-023 | infrastructure | Configuration & Options (ModerationOptions) |
| SDD-FORUM-024 | infrastructure | ML Model Training & Packaging (ModelTrainer) |

---

## Section 1 — Spec ID → Planned Source Files

For each spec, the planned `src/` (and `tests/`) artifacts that implement it. **Layer** is one of
Domain / Application / Infrastructure / Web / ModelTrainer / Tests. **Role** describes the artifact's
responsibility. All paths are **planned**.

### SDD-FORUM-001 — Comment Submission & Auto-Publishing (core)

| Planned file | Layer | Role |
|---|---|---|
| `src/ForumGuard.Web/Pages/Threads/Details.cshtml(.cs)` | Web | Razor Page hosting the submit form; `OnPostAsync` submit handler. |
| `src/ForumGuard.Application/Comments/ICommentSubmissionService.cs` | Application | Submission use-case contract (`SubmitCommentAsync`). |
| `src/ForumGuard.Application/Comments/CommentSubmissionService.cs` | Application | Orchestrates validate → create `PendingAnalysis` → run pipeline → set status → persist. |
| `src/ForumGuard.Application/Comments/SubmitCommentRequest.cs` | Application | Input DTO (`ThreadId`, `Body`, `AuthorId`). |
| `src/ForumGuard.Application/Comments/SubmitCommentResult.cs` | Application | Result DTO (`Published` / `FlaggedForReview` + validation/error state). |
| `src/ForumGuard.Domain/Entities/Comment.cs` | Domain | `Comment` aggregate (field/lifecycle authority is SDD-FORUM-010). |
| `src/ForumGuard.Application/Moderation/ICommentModerationPipeline.cs` | Application | Pipeline entry point invoked by submission (defined by SDD-FORUM-021). |
| `tests/ForumGuard.Tests/Comments/CommentSubmissionServiceTests.cs` | Tests | `[Unit]` submission rules. |
| `tests/ForumGuard.Tests/Web/CommentSubmissionFlowTests.cs` | Tests | `[Integration]` Razor + EF Core boundary. |

### SDD-FORUM-002 — Moderation Queue & Decisions (core)

| Planned file | Layer | Role |
|---|---|---|
| `src/ForumGuard.Application/Moderation/IModerationService.cs` | Application | Use-case contract: list queue, approve, reject. |
| `src/ForumGuard.Application/Moderation/ModerationService.cs` | Application | State transition + `ModerationDecision` write via Unit of Work. |
| `src/ForumGuard.Application/Moderation/Dtos/QueuedCommentDto.cs` | Application | Read DTO for queue listing (no EF entity exposed). |
| `src/ForumGuard.Application/Moderation/Dtos/ModerationDecisionRequest.cs` | Application | Input DTO (`CommentId`, `ModerationOutcome`, `Reason?`). |
| `src/ForumGuard.Domain/Specifications/FlaggedForReviewCommentsSpecification.cs` | Domain | Specification selecting `Status == FlaggedForReview` (see SDD-FORUM-022 `FlaggedPendingReviewSpecification`). |
| `src/ForumGuard.Web/Pages/Moderation/Queue.cshtml(.cs)` | Web | Queue listing page (guarded by `CanModerateComments`). |
| `src/ForumGuard.Web/Pages/Moderation/Review.cshtml(.cs)` | Web | `OnPostApprove` / `OnPostReject` handlers. |
| `tests/ForumGuard.Tests/Moderation/ModerationServiceTests.cs` | Tests | `[Unit]` use-case tests. |
| `tests/ForumGuard.Tests/Moderation/ModerationQueuePageTests.cs` | Tests | `[Integration]` Razor + authorization + EF Core boundary. |

### SDD-FORUM-003 — Registration, Login & Account Activation (core)

| Planned file | Layer | Role |
|---|---|---|
| `src/ForumGuard.Domain/Entities/ApplicationUser.cs` | Domain | `ApplicationUser : IdentityUser<Guid>` (`DisplayName`, `IsActive`, `CreatedAtUtc`). |
| `src/ForumGuard.Domain/Constants/Roles.cs` | Domain | Role string constants (alias of SDD-FORUM-011 `ForumRoles`). |
| `src/ForumGuard.Application/Accounts/IAccountService.cs` | Application | Use case: register, set-active, query active. |
| `src/ForumGuard.Application/Accounts/AccountService.cs` | Application | Registration / activation / self-deactivation guard; `Result<T>`. |
| `src/ForumGuard.Application/Common/Result.cs` | Application | `Result` / `Result<T>` (shared across core specs). |
| `src/ForumGuard.Web/Areas/Identity/Pages/Account/Register.cshtml(.cs)` | Web | Registration page bound to `UserManager`. |
| `src/ForumGuard.Web/Areas/Identity/Pages/Account/Login.cshtml(.cs)` | Web | Sign-in page enforcing `IsActive` precondition. |
| `src/ForumGuard.Web/Pages/Admin/Accounts/Index.cshtml(.cs)` | Web | Admin activate/deactivate page (guarded by `CanManageAccounts`). |
| `src/ForumGuard.Web/Security/ActiveUserMiddleware.cs` | Web | Per-request `IsActive` re-evaluation (mid-session deactivation). |
| `src/ForumGuard.Web/Program.cs` | Web | Identity registration; default-role seeding; cookie validation hook. |
| `tests/ForumGuard.Tests/Accounts/AccountServiceTests.cs` | Tests | `[Unit]` registration/activation rules. |
| `tests/ForumGuard.Tests/Web/AccountFlowsTests.cs` | Tests | `[Integration]` Razor request boundary. |

### SDD-FORUM-004 — Moderator Role Management (core)

| Planned file | Layer | Role |
|---|---|---|
| `src/ForumGuard.Application/Services/IModeratorRoleService.cs` | Application | `GrantModeratorAsync` / `RevokeModeratorAsync`. |
| `src/ForumGuard.Application/Services/ModeratorRoleService.cs` | Application | Identity role APIs + Unit of Work; returns `Result`. |
| `src/ForumGuard.Application/Dtos/ModeratorRoleChangeResult.cs` | Application | Outcome DTO (`Granted` / `Revoked` / `NoOp`). |
| `src/ForumGuard.Web/Pages/Admin/Moderators/Index.cshtml(.cs)` | Web | Admin grant/revoke page (guarded by `CanManageModerators`). |
| `src/ForumGuard.Web/Authorization/Requirements/CanManageModeratorsRequirement.cs` | Web | Requirement consumed here, **defined** in SDD-FORUM-011. |
| `src/ForumGuard.Infrastructure/Identity/` | Infrastructure | `UserManager<ApplicationUser>` / `RoleManager` registration. |
| `tests/ForumGuard.Tests/Accounts/ModeratorRoleServiceTests.cs` | Tests | `[Unit]` grant/revoke rules. |
| `tests/ForumGuard.Tests/Web/ModeratorRoleManagementTests.cs` | Tests | `[Integration]` admin page + Identity store. |

### SDD-FORUM-010 — Comment Entity & Lifecycle (domain)

| Planned file | Layer | Role |
|---|---|---|
| `src/ForumGuard.Domain/Entities/Comment.cs` | Domain | `Comment` entity + `IsPubliclyVisible` predicate. |
| `src/ForumGuard.Domain/Enums/CommentStatus.cs` | Domain | `CommentStatus` enum. |
| `src/ForumGuard.Domain/Enums/ToxicityLabel.cs` | Domain | `ToxicityLabel` enum (also referenced by SDD-FORUM-020). |
| `src/ForumGuard.Domain/Enums/ModerationOutcome.cs` | Domain | `ModerationOutcome` enum. |
| `src/ForumGuard.Domain/Lifecycle/CommentStatusTransitions.cs` | Domain | Pure transition guard (`IsAllowed`). |
| `tests/ForumGuard.Tests/Domain/CommentLifecycleTests.cs` | Tests | `[Unit]` lifecycle/transition/visibility tests. |

### SDD-FORUM-011 — Roles & Authorization Model (domain)

| Planned file | Layer | Role |
|---|---|---|
| `src/ForumGuard.Domain/Authorization/ForumRoles.cs` | Domain | Role name constants `"User"`/`"Moderator"`/`"Administrator"`. |
| `src/ForumGuard.Domain/Authorization/ForumPolicies.cs` | Domain | Policy name constants. |
| `src/ForumGuard.Web/Authorization/Requirements/CanModerateCommentsRequirement.cs` | Web | `CanModerateComments` requirement. |
| `src/ForumGuard.Web/Authorization/Handlers/CanModerateCommentsHandler.cs` | Web | `CanModerateComments` handler (Administrator excluded). |
| `src/ForumGuard.Web/Authorization/Requirements/CanManageModeratorsRequirement.cs` | Web | `CanManageModerators` requirement (consumed by SDD-FORUM-004). |
| `src/ForumGuard.Web/Authorization/Handlers/CanManageModeratorsHandler.cs` | Web | `CanManageModerators` handler. |
| `src/ForumGuard.Web/Authorization/Requirements/CanManageAccountsRequirement.cs` | Web | `CanManageAccounts` requirement (consumed by SDD-FORUM-003). |
| `src/ForumGuard.Web/Authorization/Handlers/CanManageAccountsHandler.cs` | Web | `CanManageAccounts` handler. |
| `src/ForumGuard.Application/Workspaces/IRoleWorkspace.cs` | Application | `IRoleWorkspace` Strategy contract. |
| `src/ForumGuard.Application/Workspaces/UserWorkspace.cs` | Application | `UserWorkspace` Strategy. |
| `src/ForumGuard.Application/Workspaces/ModeratorWorkspace.cs` | Application | `ModeratorWorkspace` Strategy. |
| `src/ForumGuard.Application/Workspaces/AdministratorWorkspace.cs` | Application | `AdministratorWorkspace` Strategy. |
| `src/ForumGuard.Application/Workspaces/RoleWorkspaceResolver.cs` | Application | `IRoleWorkspaceResolver` + resolver (Admin > Moderator > User). |
| `src/ForumGuard.Web/Program.cs` | Web | Policy + handler + workspace DI registration. |
| `tests/ForumGuard.Tests/Authorization/AuthorizationHandlerTests.cs` | Tests | `[Unit]` handler decisions. |
| `tests/ForumGuard.Tests/Authorization/RoleWorkspaceResolverTests.cs` | Tests | `[Unit]` resolver + workspace-action tests. |
| `tests/ForumGuard.Tests/Authorization/PolicyPipelineTests.cs` | Tests | `[Integration]` handlers through `IAuthorizationService`. |

### SDD-FORUM-020 — Comment Analysis (NAS-BERT) (infrastructure)

| Planned file | Layer | Role |
|---|---|---|
| `src/ForumGuard.Domain/Interfaces/ICommentAnalyzer.cs` | Domain | Strategy contract `AnalyzeAsync`. |
| `src/ForumGuard.Domain/Analysis/AnalysisResult.cs` | Domain | Value object `{ ToxicityLabel Label, float Score }`. |
| `src/ForumGuard.Domain/Enums/ToxicityLabel.cs` | Domain | `Clean` / `Toxic` (defined by SDD-FORUM-010). |
| `src/ForumGuard.Infrastructure/Analysis/NasBertCommentAnalyzer.cs` | Infrastructure | Adapter over `PredictionEnginePool` (Object-Pool). |
| `src/ForumGuard.Infrastructure/Analysis/KeywordCommentAnalyzer.cs` | Infrastructure | Keyword Strategy / pre-filter / test double. |
| `src/ForumGuard.Infrastructure/Analysis/ModelInput.cs` | Infrastructure | `{ string Text }`. |
| `src/ForumGuard.Infrastructure/Analysis/ModelOutput.cs` | Infrastructure | `{ string PredictedLabel, float[] Score }`. |
| `src/ForumGuard.Infrastructure/DependencyInjection.cs` | Infrastructure | `AddPredictionEnginePool(...).FromFile(...)` + analyzer registration. |
| `src/ForumGuard.Application/Options/ModerationOptions.cs` | Application | Options class (owned by SDD-FORUM-023). |
| `tests/ForumGuard.Tests/Analysis/KeywordCommentAnalyzerTests.cs` | Tests | `[Unit]` keyword Strategy. |
| `tests/ForumGuard.Tests/Analysis/NasBertCommentAnalyzerTests.cs` | Tests | `[Unit]`+`[Integration]` adapter / pool / real model. |

### SDD-FORUM-021 — Moderation Pipeline (Chain of Responsibility) (infrastructure)

| Planned file | Layer | Role |
|---|---|---|
| `src/ForumGuard.Application/Moderation/ICommentModerationHandler.cs` | Application | CoR handler Strategy contract. |
| `src/ForumGuard.Application/Moderation/CommentModerationContext.cs` | Application | Input context (`Body`, optional `Comment.Id`). |
| `src/ForumGuard.Application/Moderation/ModerationVerdict.cs` | Application | Result `{ Label, Score, IsFlagged, FlaggingHandlerName }`. |
| `src/ForumGuard.Application/Moderation/CommentModerationPipeline.cs` | Application | Chain assembler + traversal + fail-safe wrapper. |
| `src/ForumGuard.Application/Moderation/Handlers/ProfanityPreFilterHandler.cs` | Application | Link 1 — uses `KeywordCommentAnalyzer`. |
| `src/ForumGuard.Application/Moderation/Handlers/MlToxicityHandler.cs` | Application | Link 2 — uses NAS-BERT `ICommentAnalyzer`. |
| `src/ForumGuard.Domain/Interfaces/ICommentAnalyzer.cs` | Domain | Strategy consumed by handlers (defined by SDD-FORUM-020). |
| `src/ForumGuard.Web/Program.cs` | Web | DI registration + configured chain order. |
| `tests/ForumGuard.Tests/Moderation/CommentModerationPipelineTests.cs` | Tests | `[Unit]` chain/short-circuit/fail-safe. |
| `tests/ForumGuard.Tests/Moderation/ModerationPipelineDiTests.cs` | Tests | `[Integration]` container-resolved chain + config order. |

### SDD-FORUM-022 — Data Access (EF Core + MSSQL) (infrastructure)

| Planned file | Layer | Role |
|---|---|---|
| `src/ForumGuard.Domain/Interfaces/IRepository.cs` | Domain | Generic repository contract. |
| `src/ForumGuard.Domain/Interfaces/ICommentRepository.cs` | Domain | Comment-specific queries. |
| `src/ForumGuard.Domain/Interfaces/IModerationDecisionRepository.cs` | Domain | Moderation-decision queries. |
| `src/ForumGuard.Domain/Interfaces/IForumThreadRepository.cs` | Domain | Thread-specific queries. |
| `src/ForumGuard.Domain/Interfaces/IUnitOfWork.cs` | Domain | Commit boundary (`SaveChangesAsync`). |
| `src/ForumGuard.Domain/Interfaces/ISpecification.cs` | Domain | Specification contract. |
| `src/ForumGuard.Domain/Specifications/FlaggedPendingReviewSpecification.cs` | Domain | Moderator-queue spec (consumed by SDD-FORUM-002). |
| `src/ForumGuard.Domain/Specifications/PublishedCommentsByThreadSpecification.cs` | Domain | Public-visibility spec (per SDD-FORUM-010 invariant). |
| `src/ForumGuard.Domain/Specifications/ActiveUsersSpecification.cs` | Domain | Active-accounts spec. |
| `src/ForumGuard.Infrastructure/Persistence/ForumGuardDbContext.cs` | Infrastructure | EF Core `IdentityDbContext`. |
| `src/ForumGuard.Infrastructure/Persistence/Configurations/ApplicationUserConfiguration.cs` | Infrastructure | Fluent API for `ApplicationUser` extension columns. |
| `src/ForumGuard.Infrastructure/Persistence/Configurations/ForumThreadConfiguration.cs` | Infrastructure | Fluent API for `ForumThread`. |
| `src/ForumGuard.Infrastructure/Persistence/Configurations/CommentConfiguration.cs` | Infrastructure | Fluent API for `Comment` (incl. rowversion). |
| `src/ForumGuard.Infrastructure/Persistence/Configurations/ModerationDecisionConfiguration.cs` | Infrastructure | Fluent API for `ModerationDecision`. |
| `src/ForumGuard.Infrastructure/Persistence/Repositories/Repository.cs` | Infrastructure | `IRepository<T>` impl. |
| `src/ForumGuard.Infrastructure/Persistence/Repositories/CommentRepository.cs` | Infrastructure | `ICommentRepository` impl. |
| `src/ForumGuard.Infrastructure/Persistence/Repositories/ModerationDecisionRepository.cs` | Infrastructure | `IModerationDecisionRepository` impl. |
| `src/ForumGuard.Infrastructure/Persistence/Repositories/ForumThreadRepository.cs` | Infrastructure | `IForumThreadRepository` impl. |
| `src/ForumGuard.Infrastructure/Persistence/UnitOfWork.cs` | Infrastructure | `IUnitOfWork` impl. |
| `src/ForumGuard.Infrastructure/Persistence/SpecificationEvaluator.cs` | Infrastructure | Translates `ISpecification<T>` → `IQueryable<T>`. |
| `src/ForumGuard.Infrastructure/Persistence/Migrations/v1.0.0_InitialSchema.sql` | Infrastructure | Initial schema migration. |
| `tests/ForumGuard.Tests/Persistence/SpecificationTests.cs` | Tests | `[Unit]` specification criteria. |
| `tests/ForumGuard.Tests/Persistence/ForumGuardDbContextTests.cs` | Tests | `[Integration]` schema/round-trip/concurrency against MSSQL. |

### SDD-FORUM-023 — Configuration & Options (ModerationOptions) (infrastructure)

| Planned file | Layer | Role |
|---|---|---|
| `src/ForumGuard.Application/Options/ModerationOptions.cs` | Application | Options class (`ToxicityThreshold`, `ModelPath`, `MaxCommentLength`, `ProfanityListPath`). |
| `src/ForumGuard.Application/Options/ModerationOptionsValidator.cs` | Application | `IValidateOptions<ModerationOptions>` (threshold range, non-empty paths, positive length). |
| `src/ForumGuard.Application/Options/ModerationConfigKeys.cs` | Application | Section/key name constants for `Moderation:*`. |
| `src/ForumGuard.Web/Program.cs` | Web | `AddOptions<ModerationOptions>().Bind(...).ValidateDataAnnotations().ValidateOnStart()` from `Moderation:*`. |
| `tests/ForumGuard.Tests/Options/ModerationOptionsTests.cs` | Tests | `[Unit]` validator (each field, boundaries). |
| `tests/ForumGuard.Tests/Options/ModerationOptionsBindingTests.cs` | Tests | `[Integration]` binding + `ValidateOnStart`. |

### SDD-FORUM-024 — ML Model Training & Packaging (ModelTrainer) (infrastructure)

| Planned file | Layer | Role |
|---|---|---|
| `src/ForumGuard.ModelTrainer/Program.cs` | ModelTrainer | Console entry point: load + validate dataset, train, report metrics, export `model.zip`; non-zero exit on abort. |
| `src/ForumGuard.ModelTrainer/TrainingPipeline.cs` | ModelTrainer | `MapValueToKey → MulticlassClassification.Trainers.TextClassification(Label/Text) → MapKeyToValue`; trains + returns `ITransformer` + metrics. |
| `src/ForumGuard.ModelTrainer/SeedDatasetLoader.cs` | ModelTrainer | Loads + validates `seed-comments.csv` (header, UTF-8, both classes, min rows). |
| `src/ForumGuard.ModelTrainer/TrainingDataRow.cs` | ModelTrainer | CSV row schema `{ string Text, string Label }`. |
| `src/ForumGuard.ModelTrainer/TrainingMetrics.cs` | ModelTrainer | `MicroAccuracy` / `MacroAccuracy` / confusion-matrix capture. |
| `src/ForumGuard.ModelTrainer/seed-comments.csv` | ModelTrainer | Hand-written, extendable training seed (`Text`,`Label`); UTF-8, both classes. |
| `data/seed-comments.csv` | ModelTrainer | Optional repo-root alternate corpus location (trainer accepts a dataset path arg). |
| `tests/ForumGuard.Tests/ModelTrainer/SeedDatasetLoaderTests.cs` | Tests | `[Unit]` dataset loading/validation. |
| `tests/ForumGuard.Tests/ModelTrainer/TrainingPipelineTests.cs` | Tests | `[Unit]` pipeline construction + `[Integration]` train-then-load round-trip. |

---

## Section 2 — Planned Component / File → Governing Spec ID(s)

Each planned artifact and which spec(s) govern it. **Authoritative** = the spec that owns the
contract/definition; **Consumes** = a spec that depends on the artifact without defining it.

### Domain entities & enums

| Planned component | Authoritative spec | Also referenced by |
|---|---|---|
| `Comment` entity (`Comment.cs`) | SDD-FORUM-010 | SDD-FORUM-001, 002, 020, 021, 022 |
| `ForumThread` entity | SDD-FORUM-010 (canonical model) | SDD-FORUM-001, 022 |
| `ApplicationUser` entity | SDD-FORUM-003 (fields/registration) | SDD-FORUM-004, 010, 011, 022 |
| `ModerationDecision` entity | SDD-FORUM-002 (audit write) | SDD-FORUM-004, 022 |
| `CommentStatus` enum | SDD-FORUM-010 | SDD-FORUM-001, 002, 021, 022 |
| `ToxicityLabel` enum | SDD-FORUM-010 | SDD-FORUM-020, 021 |
| `ModerationOutcome` enum | SDD-FORUM-010 | SDD-FORUM-002, 022 |
| `CommentStatusTransitions` (transition guard) | SDD-FORUM-010 | SDD-FORUM-001, 002 |

### DbContext, repositories, specifications, Unit of Work

| Planned component | Authoritative spec | Also referenced by |
|---|---|---|
| `ForumGuardDbContext` | SDD-FORUM-022 | SDD-FORUM-003 (Identity schema) |
| `IRepository<T>` / `Repository<T>` | SDD-FORUM-022 | SDD-FORUM-001, 002, 004 |
| `ICommentRepository` / `CommentRepository` | SDD-FORUM-022 | SDD-FORUM-001, 002 |
| `IModerationDecisionRepository` / impl | SDD-FORUM-022 | SDD-FORUM-002 |
| `IForumThreadRepository` / impl | SDD-FORUM-022 | SDD-FORUM-001 |
| `IUnitOfWork` / `UnitOfWork` | SDD-FORUM-022 | SDD-FORUM-001, 002, 004 |
| `ISpecification<T>` / `SpecificationEvaluator` | SDD-FORUM-022 | SDD-FORUM-002 |
| `FlaggedPendingReviewSpecification` | SDD-FORUM-022 | SDD-FORUM-002 |
| `PublishedCommentsByThreadSpecification` | SDD-FORUM-022 | SDD-FORUM-001, 010 (visibility) |
| `ActiveUsersSpecification` | SDD-FORUM-022 | SDD-FORUM-003 |
| Entity `IEntityTypeConfiguration<T>` classes | SDD-FORUM-022 | SDD-FORUM-010 (field contract) |
| `v1.0.0_InitialSchema.sql` migration | SDD-FORUM-022 | SDD-FORUM-010, 003 |

### Comment analysis (ICommentAnalyzer + analyzers + model I/O)

| Planned component | Authoritative spec | Also referenced by |
|---|---|---|
| `ICommentAnalyzer` (Strategy contract) | SDD-FORUM-020 | SDD-FORUM-021 (consumes) |
| `AnalysisResult` value object | SDD-FORUM-020 | SDD-FORUM-010, 021 |
| `NasBertCommentAnalyzer` (Adapter) | SDD-FORUM-020 | SDD-FORUM-021 (`MlToxicityHandler`) |
| `KeywordCommentAnalyzer` (Strategy) | SDD-FORUM-020 | SDD-FORUM-021 (`ProfanityPreFilterHandler`) |
| `ModelInput` / `ModelOutput` | SDD-FORUM-020 | SDD-FORUM-024 (training output schema) |
| `PredictionEnginePool` registration (`DependencyInjection.cs`) | SDD-FORUM-020 | SDD-FORUM-021 |

### Moderation pipeline (Chain of Responsibility)

| Planned component | Authoritative spec | Also referenced by |
|---|---|---|
| `ICommentModerationHandler` | SDD-FORUM-021 | SDD-FORUM-001 |
| `CommentModerationContext` | SDD-FORUM-021 | SDD-FORUM-001 |
| `ModerationVerdict` | SDD-FORUM-021 | SDD-FORUM-001 (maps to `CommentStatus`) |
| `CommentModerationPipeline` | SDD-FORUM-021 | SDD-FORUM-001 (caller) |
| `ProfanityPreFilterHandler` | SDD-FORUM-021 | SDD-FORUM-020 (uses `KeywordCommentAnalyzer`) |
| `MlToxicityHandler` | SDD-FORUM-021 | SDD-FORUM-020 (uses `NasBertCommentAnalyzer`) |
| `ICommentModerationPipeline` (Application entry) | SDD-FORUM-021 | SDD-FORUM-001 |

### Authorization requirements / handlers / policies

| Planned component | Authoritative spec | Also referenced by |
|---|---|---|
| `ForumRoles` / `ForumPolicies` constants | SDD-FORUM-011 | SDD-FORUM-003, 004 |
| `CanModerateCommentsRequirement` / `...Handler` | SDD-FORUM-011 | SDD-FORUM-002 (applied at queue) |
| `CanManageModeratorsRequirement` / `...Handler` | SDD-FORUM-011 | SDD-FORUM-004 (applied at role-mgmt page) |
| `CanManageAccountsRequirement` / `...Handler` | SDD-FORUM-011 | SDD-FORUM-003 (applied at admin accounts page) |

### IRoleWorkspace strategies

| Planned component | Authoritative spec | Also referenced by |
|---|---|---|
| `IRoleWorkspace` (Strategy interface) | SDD-FORUM-011 | — |
| `UserWorkspace` | SDD-FORUM-011 | SDD-FORUM-001 (authoring actions) |
| `ModeratorWorkspace` | SDD-FORUM-011 | SDD-FORUM-002 (moderation actions) |
| `AdministratorWorkspace` | SDD-FORUM-011 | SDD-FORUM-003, 004 (admin actions) |
| `IRoleWorkspaceResolver` / `RoleWorkspaceResolver` | SDD-FORUM-011 | — |

### Application services & DTOs (use cases)

| Planned component | Authoritative spec | Also referenced by |
|---|---|---|
| `ICommentSubmissionService` / `CommentSubmissionService` | SDD-FORUM-001 | SDD-FORUM-021 (invokes pipeline) |
| `SubmitCommentRequest` / `SubmitCommentResult` | SDD-FORUM-001 | — |
| `IModerationService` / `ModerationService` | SDD-FORUM-002 | SDD-FORUM-022 (Unit of Work) |
| `QueuedCommentDto` / `ModerationDecisionRequest` | SDD-FORUM-002 | — |
| `IAccountService` / `AccountService` | SDD-FORUM-003 | SDD-FORUM-011 (policies) |
| `IModeratorRoleService` / `ModeratorRoleService` | SDD-FORUM-004 | SDD-FORUM-011 (policy), 022 (Unit of Work) |
| `ModeratorRoleChangeResult` | SDD-FORUM-004 | — |
| `Result` / `Result<T>` (`Common/Result.cs`) | SDD-FORUM-003 | SDD-FORUM-001, 002, 004 |
| `ModerationOptions` | SDD-FORUM-023 | SDD-FORUM-020, 021 |

### Razor Pages (Web)

| Planned page | Authoritative spec | Guarding policy |
|---|---|---|
| `Pages/Threads/Details.cshtml(.cs)` | SDD-FORUM-001 | authenticated + active `User` |
| `Pages/Moderation/Queue.cshtml(.cs)` | SDD-FORUM-002 | `CanModerateComments` (SDD-FORUM-011) |
| `Pages/Moderation/Review.cshtml(.cs)` | SDD-FORUM-002 | `CanModerateComments` (SDD-FORUM-011) |
| `Areas/Identity/Pages/Account/Register.cshtml(.cs)` | SDD-FORUM-003 | anonymous (self-register) |
| `Areas/Identity/Pages/Account/Login.cshtml(.cs)` | SDD-FORUM-003 | anonymous + `IsActive` gate |
| `Pages/Admin/Accounts/Index.cshtml(.cs)` | SDD-FORUM-003 | `CanManageAccounts` (SDD-FORUM-011) |
| `Pages/Admin/Moderators/Index.cshtml(.cs)` | SDD-FORUM-004 | `CanManageModerators` (SDD-FORUM-011) |
| `Security/ActiveUserMiddleware.cs` | SDD-FORUM-003 | per-request `IsActive` re-check |
| `Program.cs` (DI composition root) | SDD-FORUM-011 (policies/workspaces), 020/021 (analysis/pipeline), 003 (Identity) | — |

### ModelTrainer (console app)

| Planned component | Authoritative spec | Also referenced by |
|---|---|---|
| `ForumGuard.ModelTrainer/Program.cs` | SDD-FORUM-024 | SDD-FORUM-020 (training shape) |
| `TrainingPipeline.cs` | SDD-FORUM-024 | SDD-FORUM-020 |
| `seed-comments.csv` | SDD-FORUM-024 | SDD-FORUM-020 (label space `Clean`/`Toxic`) |
| `model.zip` (output artifact) | SDD-FORUM-024 | SDD-FORUM-020 (`PredictionEnginePool.FromFile`) |
