---
id: SDD-MN-CORE-002
title: Admin Game Catalog Management
status: Planned
version: 1.0
owner: catalog
last_updated: 2026-05-08
related:
  - SDD-MN-DOM-002
  - SDD-MN-CORE-003
  - SDD-MN-INF-002
---

# SDD-MN-CORE-002 — Admin Game Catalog Management

## 1. Context

The board game catalogue is the foundation of every other feature in MeepleNight —
hosts pick games when planning a night, sessions reference games, and statistics
roll up by game. Catalogue accuracy and consistency are therefore important.

To keep the data quality high, **only administrators** may add, edit, or remove
games. Regular users can only browse (SDD-MN-CORE-003) and reference games when
hosting nights and logging sessions.

**Key Files (planned):**
- `src/MeepleNight.Web/Areas/Admin/Controllers/GamesController.cs`
- `src/MeepleNight.Web/Areas/Admin/Views/Games/{Index,Create,Edit,Delete}.cshtml`
- `src/MeepleNight.Services/GameAdminService.cs`
- `src/MeepleNight.Services/Validators/GameValidator.cs`

## 2. Behavior

### 2.1 Authorisation

- **MUST** require role `Admin` for every action under `Areas/Admin/Games/*`.
  Unauthorised users **MUST** receive HTTP 403 (or be redirected to login if
  unauthenticated).

### 2.2 Listing

- **MUST** display all games (including soft-deleted ones, filtered by toggle)
  with: cover image thumbnail, title, min/max players, average duration, category,
  active flag.
- **MUST** support paging at 20 rows per page.
- **SHOULD** support text search on title and category.

### 2.3 Create

- **MUST** accept: Title, Description, MinPlayers, MaxPlayers, AverageDurationMinutes,
  Category, optional CoverImage upload.
- **MUST** persist the cover image to `wwwroot/uploads/games/` with a randomly
  generated filename and record the relative path on the entity.
- **MUST** validate image: ≤ 2 MB, content type in `{image/jpeg, image/png, image/webp}`.
- **MUST** stamp `CreatedAtUtc` and `CreatedByUserId` on creation.

### 2.4 Edit

- **MUST** allow updating any catalogue field.
- **MUST** allow replacing or removing the cover image. Removing **MUST** delete
  the file from disk after the database update succeeds.
- **MUST** stamp `UpdatedAtUtc` and `UpdatedByUserId` on every save.

### 2.5 Delete

- **MUST** perform a **soft delete** (set `IsActive = false`). Hard deletion is
  forbidden because referential integrity from past sessions must be preserved.
- **MUST** offer a "Restore" action that flips `IsActive` back to `true`.

## 3. Validation

| Field | Rule |
|---|---|
| Title | Required, 2–100 chars, unique (case-insensitive) among active games |
| Description | Optional, ≤ 2000 chars |
| MinPlayers | Required, ≥ 1 |
| MaxPlayers | Required, ≥ MinPlayers, ≤ 20 |
| AverageDurationMinutes | Required, 5–720 |
| Category | Required, from enum `GameCategory` (Strategy, Family, Party, Cooperative, RPG, Abstract, Other) |
| CoverImage | Optional file, ≤ 2 MB, content-type allow-list |

## 4. Errors

| Condition | UI Outcome |
|---|---|
| Duplicate title (active) | Field error on `Title`: "A game with this title already exists." |
| MaxPlayers < MinPlayers | Field error on `MaxPlayers` |
| Cover image too large | Field error on `CoverImage`: "Image must be ≤ 2 MB." |
| Cover image wrong type | Field error: "Only JPEG, PNG, or WebP images are allowed." |
| Soft-deleted game referenced | Game appears in past session lists with a strikethrough title and "(removed)" badge. |

## 5. Versioning

- Adding a new `GameCategory` enum value is a non-breaking change (new value +
  database seed update).
- Renaming or removing an existing category is **breaking** — requires a change
  entry and a data-migration script.
- Schema changes require a versioned migration (per SDD-MN-INF-003).

## 6. Test Plan

### Unit Tests
- `[Unit] Create_WithValidPayload_PersistsGameWithCreatedStamps`
- `[Unit] Create_WithDuplicateTitle_ReturnsValidationError`
- `[Unit] Create_WithMaxLessThanMin_ReturnsValidationError`
- `[Unit] Create_WithOversizedImage_ReturnsValidationError`
- `[Unit] Edit_UpdatesFieldsAndStampsUpdatedAt`
- `[Unit] Edit_RemoveImage_DeletesFileFromDiskAfterSave`
- `[Unit] Delete_FlipsIsActiveFalse_DoesNotRemoveRow`
- `[Unit] Restore_FlipsIsActiveTrue`

### Integration Tests
- `[Integration] AdminController_RequiresAdminRole_ReturnsForbiddenForUser`
- `[Integration] Create_PersistsGameRow_AndUploadedFileExists`
- `[Integration] Delete_GameStillReferencedBySessions_ReadableViaJoin`

## 7. Cross-References

- Domain: SDD-MN-DOM-002 (Game)
- Public surface: SDD-MN-CORE-003 (must filter out `IsActive = false` for non-admins)
- Infrastructure: SDD-MN-INF-002 (`Admin` role policy), SDD-MN-INF-001 (audit-log all create/edit/delete)
