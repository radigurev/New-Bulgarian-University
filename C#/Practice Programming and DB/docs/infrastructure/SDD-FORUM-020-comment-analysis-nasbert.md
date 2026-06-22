# Comment Analysis (NAS-BERT)

| Field | Value |
|---|---|
| **Title** | Comment Analysis (NAS-BERT) |
| **Spec ID** | SDD-FORUM-020 |
| **Category** | infrastructure |
| **Status** | Active |
| **Last updated** | 2026-06-21 |
| **Owner** | TBD |

> Greenfield spec — **authoritative**. It defines what WILL be built. No production code exists yet;
> all paths under `src/` in *Key Files (planned)* are **PLANNED/TARGET** locations.

---

## 1. Context & Scope

This spec defines the **comment text-analysis layer** of ForumGuard: the `ICommentAnalyzer`
Strategy abstraction and its two implementations, the ML.NET NAS-BERT scoring adapter, the
training pipeline shape, and the mapping from raw model output to the domain `AnalysisResult`.
It is the engine that classifies a `Comment.Body` as `Clean` or `Toxic` so the rest of the system
can auto-publish or queue it.

### Patterns mandated by this spec

| Pattern | Where |
|---|---|
| **Strategy** | `ICommentAnalyzer` (domain interface) with two interchangeable implementations. |
| **Adapter** | `NasBertCommentAnalyzer` adapts ML.NET `PredictionEnginePool<ModelInput, ModelOutput>` to `ICommentAnalyzer`. |
| **Object-Pool** | ML.NET `PredictionEnginePool<ModelInput, ModelOutput>` registered via `AddPredictionEnginePool(...).FromFile(modelName, ModelPath, watchForChanges: true)`. |
| **Options** | `ModerationOptions` supplies `ToxicityThreshold`, `ModelPath`, `MaxCommentLength`, `ProfanityListPath`. |

### Covered

- The `ICommentAnalyzer` Strategy contract: `AnalyzeAsync(string text)` → `AnalysisResult { ToxicityLabel Label, float Score }`.
- `NasBertCommentAnalyzer` (Adapter, Infrastructure) over the ML.NET `PredictionEnginePool`.
- `KeywordCommentAnalyzer` (second Strategy): a fast, model-free profanity keyword matcher usable
  as a Chain-of-Responsibility pre-filter and as a unit-test double.
- ML.NET training pipeline shape: `MulticlassClassification.Trainers.TextClassification`
  (NAS-BERT roBERTa via `Microsoft.ML.TorchSharp` + `libtorch-cpu`), binary labels `Clean` / `Toxic`,
  fine-tuning a pretrained model.
- The `ModelInput { string Text }` / `ModelOutput { string PredictedLabel, float[] Score }` contract
  and its mapping to `AnalysisResult`.
- The toxic-score decision rule against `Moderation:ToxicityThreshold`.
- DI registration of the pool and both analyzers; thread-safety rationale.
- Rationale for using ML.NET 4.x (on .NET 8) with the API that "ML.NET 2.0" introduced.

### Excluded (covered elsewhere)

- The end-to-end comment submission / auto-publish flow and lifecycle transitions — see **SDD-FORUM-001**.
- The Chain-of-Responsibility moderation pipeline that *invokes* `ICommentAnalyzer` (handlers
  `ProfanityPreFilter`, `MlToxicity`) — see **SDD-FORUM-021** (Moderation Pipeline).
- Persistence of `Comment.AnalysisLabel` / `AnalysisScore` / `AnalyzedAtUtc` and repositories —
  see **SDD-FORUM-022** (Data Access).
- Binding and validation of `ModerationOptions` from configuration — see **SDD-FORUM-023**.
- Training the model end-to-end, the seed CSV format, and `model.zip` packaging — see **SDD-FORUM-024** (ModelTrainer).
- Domain enums and the comment lifecycle state machine — see **SDD-FORUM-010**.

### Cross-referenced specs

- **SDD-FORUM-001** — Comment Submission & Auto-Publish Flow (consumer of the analysis result).
- **SDD-FORUM-021** — Moderation Pipeline / Chain of Responsibility (orchestrates analyzers).
- **SDD-FORUM-010** — Comment Entity & Lifecycle (defines `ToxicityLabel` and the lifecycle).
- **SDD-FORUM-022** — Data Access (persists the analysis fields).
- **SDD-FORUM-023** — Configuration & Options (`ModerationOptions`).
- **SDD-FORUM-024** — ML Model Training & Packaging (produces `model.zip`).

### Key Files (planned — DO NOT yet exist)

| Planned path | Responsibility |
|---|---|
| `src/ForumGuard.Domain/Interfaces/ICommentAnalyzer.cs` | Strategy contract `AnalyzeAsync`. |
| `src/ForumGuard.Domain/Analysis/AnalysisResult.cs` | Result value object `{ ToxicityLabel Label, float Score }`. |
| `src/ForumGuard.Domain/Enums/ToxicityLabel.cs` | `Clean` / `Toxic` (defined by SDD-FORUM-010). |
| `src/ForumGuard.Infrastructure/Analysis/NasBertCommentAnalyzer.cs` | Adapter over `PredictionEnginePool`. |
| `src/ForumGuard.Infrastructure/Analysis/KeywordCommentAnalyzer.cs` | Keyword Strategy / pre-filter / test double. |
| `src/ForumGuard.Infrastructure/Analysis/ModelInput.cs` | `{ string Text }`. |
| `src/ForumGuard.Infrastructure/Analysis/ModelOutput.cs` | `{ string PredictedLabel, float[] Score }`. |
| `src/ForumGuard.Infrastructure/DependencyInjection.cs` | `AddPredictionEnginePool(...).FromFile(...)` + analyzer registration. |
| `src/ForumGuard.Application/Options/ModerationOptions.cs` | Options class (owned by SDD-FORUM-023). |
| `tests/ForumGuard.Tests/Analysis/KeywordCommentAnalyzerTests.cs` | Unit tests for the keyword Strategy. |
| `tests/ForumGuard.Tests/Analysis/NasBertCommentAnalyzerTests.cs` | Unit + integration tests for the adapter. |

### Technology rationale (ML.NET 2.0 → ML.NET 4.x on .NET 8)

The assignment names **NAS-BERT** / "ML.NET 2.0". ML.NET 2.0 was the release that **introduced**
the `MulticlassClassification.Trainers.TextClassification` API (NAS-BERT, backed by TorchSharp).
That exact API is **still supported** in the current **ML.NET 4.x** packages, which target
**.NET 8** (and newer). ForumGuard targets **.NET 8 LTS** — chosen over .NET 9, which reached
end-of-support in May 2026 — and uses **ML.NET 4.x** (`Microsoft.ML`, `Microsoft.ML.TorchSharp`,
`libtorch-cpu`) with the **identical NAS-BERT TextClassification API** the task references. This is a
version-currency decision only; the algorithm and API surface are unchanged.

---

## 2. Behavior

`AnalysisResult` is `{ ToxicityLabel Label, float Score }`, where `Score` is the model's probability
that the text is **`Toxic`** (i.e., the `ModelOutput.Score` element corresponding to the `Toxic` class),
normalized to the inclusive range `[0.0, 1.0]`.

### 2.1 Happy path — toxic-score scoring and labelling

1. The `ICommentAnalyzer` interface MUST expose exactly one analysis method:
   `Task<AnalysisResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default)`.
2. `NasBertCommentAnalyzer` MUST implement `ICommentAnalyzer` by **adapting** the injected
   `PredictionEnginePool<ModelInput, ModelOutput>`: it MUST rent an engine from the pool, call
   `Predict(new ModelInput { Text = text })`, and translate the resulting `ModelOutput` to `AnalysisResult`.
3. The analyzer MUST set `AnalysisResult.Score` to the toxic-class probability taken from
   `ModelOutput.Score` (the element aligned to the `Toxic` label).
4. The analyzer MUST set `AnalysisResult.Label` to `ToxicityLabel.Toxic` when
   `Score >= Moderation:ToxicityThreshold` (the `ToxicityThreshold` bound on `ModerationOptions`,
   default `0.5`), and to `ToxicityLabel.Clean` otherwise. The comparison MUST be inclusive (`>=`).
5. The threshold value MUST be read from `ModerationOptions` (via the Options pattern, see
   **SDD-FORUM-023**) and MUST NOT be hard-coded in the analyzer.
6. `KeywordCommentAnalyzer` MUST implement the same `ICommentAnalyzer` contract by matching the text,
   case-insensitively, against the profanity keyword list loaded from `Moderation:ProfanityListPath`;
   it MUST return `ToxicityLabel.Toxic` with `Score = 1.0` when any keyword is matched and
   `ToxicityLabel.Clean` with `Score = 0.0` otherwise.
7. Both analyzers MUST be resolvable by DI through the `ICommentAnalyzer` abstraction so that callers
   (the Moderation Pipeline, **SDD-FORUM-021**) depend only on the Strategy interface, never on a
   concrete type.

### 2.2 Object-Pool registration and thread safety

8. The ML.NET prediction engine MUST be supplied through a **`PredictionEnginePool<ModelInput, ModelOutput>`**
   registered with `AddPredictionEnginePool<ModelInput, ModelOutput>().FromFile(modelName, ModelPath, watchForChanges: true)`,
   where `ModelPath` comes from `ModerationOptions.ModelPath`.
9. `NasBertCommentAnalyzer` MUST NOT construct, cache, or share a single `PredictionEngine`
   instance; it MUST obtain one per call through the pool. Rationale: ML.NET `PredictionEngine` is
   **not thread-safe**, and Razor Pages serves concurrent requests; the Object-Pool serializes safe reuse.
10. The underlying `ITransformer` / model file SHOULD be loaded **once** and reused via the pool; with
    `watchForChanges: true`, the pool SHOULD hot-reload when `model.zip` on disk changes, without an
    application restart.
11. The analyzer registration MUST register `NasBertCommentAnalyzer` as the production
    `ICommentAnalyzer` used by the `MlToxicity` handler, while `KeywordCommentAnalyzer` MUST be
    available for the `ProfanityPreFilter` pre-filter handler (both defined by **SDD-FORUM-021**).

### 2.3 Training pipeline shape (consumed by SDD-FORUM-024)

12. The training pipeline defined for the model MUST use
    `mlContext.MulticlassClassification.Trainers.TextClassification` (NAS-BERT roBERTa via
    `Microsoft.ML.TorchSharp`) fine-tuning a pretrained model, with `labelColumnName` mapped to the
    `Label` column and `sentence1ColumnName` mapped to the `Text` column of the seed CSV.
13. The label space MUST be exactly the two values `Clean` and `Toxic` (binary classification expressed
    through the multiclass TextClassification trainer); the trained `ModelOutput.PredictedLabel` MUST be
    one of those two strings.
14. Training MUST run on CPU (`libtorch-cpu`); this spec acknowledges CPU fine-tuning is **slow** and
    therefore training is a one-time offline step performed by `ForumGuard.ModelTrainer`
    (see **SDD-FORUM-024**), never at web-request time.

### 2.4 Edge case — empty or whitespace-only text

15. When `text` is `null`, empty, or whitespace-only, `AnalyzeAsync` MUST short-circuit and return
    `AnalysisResult { Label = ToxicityLabel.Clean, Score = 0.0 }` **without** invoking the model.
    Empty input is treated as neutral / non-toxic; no model call is wasted on it.
16. This short-circuit MUST hold identically for both `NasBertCommentAnalyzer` and
    `KeywordCommentAnalyzer`.

### 2.5 Edge case — model file missing or unloadable

17. When `Moderation:ModelPath` does not resolve to a readable model file at the time the pool first
    needs it, the `NasBertCommentAnalyzer` MUST surface the failure as a clear analyzer error (see
    Error Rules) rather than silently returning `Clean`. A missing model MUST NOT be interpreted as
    "all comments are clean".
18. Application startup SHOULD remain successful even when the model is not yet present (the pool with
    `watchForChanges: true` SHOULD pick the model up once it appears); the failure SHOULD manifest only
    when an analysis is actually requested. This lets the app boot before the first model export.

### 2.6 Edge case — text exceeding the NAS-BERT token limit (~510 tokens / ~512 with specials)

19. NAS-BERT/roBERTa accepts at most ~512 tokens (~510 content tokens). `NasBertCommentAnalyzer` MUST
    NOT throw on long input: it MUST allow the ML.NET pipeline's built-in truncation to apply so the
    call still returns a valid `AnalysisResult`.
20. Because `Comment.Body` is capped at `MaxCommentLength` (domain max `4000`, see **SDD-FORUM-001** /
    **SDD-FORUM-010**) and a body can exceed ~510 tokens, the analyzer SHOULD score the **truncated**
    text and document that only the leading window is analyzed; it MUST still produce a definite
    `Clean` / `Toxic` label for the comment.

---

## 3. Validation Rules

### Field-level

| Field | Rule |
|---|---|
| `AnalyzeAsync` `text` argument | MAY be `null`/empty/whitespace; handled by the empty-text short-circuit (returns `Clean`, score `0.0`). |
| `AnalysisResult.Score` | MUST be a finite `float` in the inclusive range `[0.0, 1.0]`. |
| `AnalysisResult.Label` | MUST be one of `ToxicityLabel.Clean` or `ToxicityLabel.Toxic` — never `null`. |
| `ModelOutput.PredictedLabel` | MUST be one of the trained labels (`"Clean"` / `"Toxic"`). |
| `ModelOutput.Score` | MUST be a non-empty `float[]` whose entries align to the trained classes. |

### Cross-field

- The `Label` MUST be derived **only** from `Score` versus `ToxicityThreshold` (rule 4) for
  `NasBertCommentAnalyzer`. The analyzer MUST NOT trust `ModelOutput.PredictedLabel` over the
  threshold rule when they disagree; the threshold rule is authoritative so the operator-tunable
  `ToxicityThreshold` controls behavior.
- For `KeywordCommentAnalyzer`, `Score = 1.0` MUST co-occur with `Label = Toxic`, and `Score = 0.0`
  with `Label = Clean` (no intermediate scores).

### State-based / configuration

- `ToxicityThreshold` MUST be within `[0.0, 1.0]` (validated by **SDD-FORUM-023**); the analyzer
  consumes the bound value and MUST behave deterministically for any in-range threshold.
- `ModelPath` and `ProfanityListPath` MUST be non-empty configured values; their absence is an Error
  Rule below.
- The same `text` analyzed twice with the same model and threshold MUST yield the same
  `AnalysisResult` (deterministic scoring at inference time).

---

## 4. Error Rules

| # | Trigger | Error type | Domain mapping | User-facing outcome (via the caller / Razor handler) |
|---|---|---|---|---|
| E1 | `Moderation:ModelPath` is unset/empty or points to a missing/unreadable file when the pool first scores | `notfound` / configuration | `AnalyzerUnavailableException` thrown from `NasBertCommentAnalyzer` (wraps the pool/IO failure) | The submission flow (SDD-FORUM-001) MUST treat the comment as **not analyzable**; it SHOULD keep the comment out of public view (leave `PendingAnalysis` or route to `FlaggedForReview` per SDD-FORUM-021) and show a non-technical "your comment is being reviewed" message. It MUST NOT auto-publish on analyzer failure. |
| E2 | `model.zip` exists but is corrupt / wrong schema, so the transformer cannot load | `conflict` / configuration | `AnalyzerUnavailableException` (wraps the ML.NET load exception) | Same fail-safe outcome as E1 — never auto-publish; comment withheld. |
| E3 | `Moderation:ProfanityListPath` missing/unreadable when `KeywordCommentAnalyzer` initializes | `notfound` / configuration | `AnalyzerConfigurationException` thrown by `KeywordCommentAnalyzer` | Surfaced at startup/first-use; the pre-filter handler is misconfigured — the pipeline (SDD-FORUM-021) MUST fail closed (do not pass the comment as clean). |
| E4 | Underlying ML.NET `Predict` throws at inference (transient TorchSharp/native failure) | `conflict` | `AnalyzerUnavailableException` (wraps the inner exception; original preserved) | Treated as analyzer failure → comment withheld, user told it is under review. MUST NOT be swallowed into a `Clean` result. |
| E5 | `text` is `null`/empty/whitespace | (not an error) | No exception — returns `AnalysisResult { Clean, 0.0 }` (rule 15) | Comment proceeds as `Clean`. |
| E6 | `cancellationToken` is cancelled during `AnalyzeAsync` | (cooperative cancellation) | `OperationCanceledException` propagates (not wrapped) | The request is aborted by the host; no partial result is persisted. |

**Fail-safe principle (MUST):** an analyzer error MUST NEVER be silently converted into a `Clean`
result. Any analysis failure (E1–E4) MUST result in the comment being **withheld**, never auto-published.

---

## 5. Versioning Notes

- **v1 — Initial specification (2026-06-21).** Defines `ICommentAnalyzer` Strategy, the
  `NasBertCommentAnalyzer` Adapter over the ML.NET `PredictionEnginePool` Object-Pool, the
  `KeywordCommentAnalyzer` second Strategy, `ModelInput`/`ModelOutput` → `AnalysisResult` mapping, the
  `ToxicityThreshold` decision rule, the NAS-BERT TextClassification training-pipeline shape, the
  thread-safety rationale, the ML.NET 2.0 → 4.x version-currency rationale, and the empty-text /
  missing-model / token-limit edge cases.

---

## 6. Test Plan

Business tests reference this spec via `[Category("SDD-FORUM-020")]`. Test names follow
`MethodName_Scenario_ExpectedResult`. This spec crosses the DI / model-file boundary, so it carries
**both** `[Unit]` and `[Integration]` tests.

### Unit — `KeywordCommentAnalyzer` (model-free Strategy / test double)

- `[Unit]` `AnalyzeAsync_TextContainingProfanityKeyword_ReturnsToxicWithScoreOne`
- `[Unit]` `AnalyzeAsync_CleanText_ReturnsCleanWithScoreZero`
- `[Unit]` `AnalyzeAsync_KeywordDifferentCasing_MatchesCaseInsensitively`
- `[Unit]` `AnalyzeAsync_EmptyOrWhitespaceText_ReturnsCleanWithoutMatching`
- `[Unit]` `Constructor_MissingProfanityListPath_ThrowsAnalyzerConfigurationException`

### Unit — `NasBertCommentAnalyzer` decision logic (pool/output mocked)

- `[Unit]` `AnalyzeAsync_ToxicScoreEqualToThreshold_ReturnsToxic`
- `[Unit]` `AnalyzeAsync_ToxicScoreAboveThreshold_ReturnsToxic`
- `[Unit]` `AnalyzeAsync_ToxicScoreBelowThreshold_ReturnsClean`
- `[Unit]` `AnalyzeAsync_NullOrWhitespaceText_ReturnsCleanWithoutInvokingPool`
- `[Unit]` `AnalyzeAsync_PredictedLabelDisagreesWithThreshold_ThresholdRuleIsAuthoritative`
- `[Unit]` `AnalyzeAsync_ModelOutputScore_MapsToxicProbabilityIntoAnalysisResultScore`
- `[Unit]` `AnalyzeAsync_CustomThresholdFromModerationOptions_AppliesConfiguredCutoff`
- `[Unit]` `AnalyzeAsync_PoolPredictThrows_WrapsInAnalyzerUnavailableExceptionNotClean`
- `[Unit]` `AnalyzeAsync_CancellationRequested_PropagatesOperationCanceledException`

### Integration — pool registration & real model file (crosses DI / model boundary)

- `[Integration]` `AddPredictionEnginePool_FromFileWithModelPath_ResolvesPredictionEnginePool`
- `[Integration]` `Analyzer_ResolvedViaICommentAnalyzer_ReturnsNasBertImplementation`
- `[Integration]` `AnalyzeAsync_RealModelObviouslyToxicText_ReturnsToxic`
- `[Integration]` `AnalyzeAsync_RealModelObviouslyCleanText_ReturnsClean`
- `[Integration]` `AnalyzeAsync_TextExceedingTokenLimit_TruncatesAndReturnsDefiniteLabel`
- `[Integration]` `AnalyzeAsync_ModelFileMissingAtScoreTime_ThrowsAnalyzerUnavailableException`
- `[Integration]` `AnalyzeAsync_ConcurrentRequestsThroughPool_AllReturnValidResults`
- `[Integration]` `Pool_WatchForChangesTrue_ReloadsModelWhenFileReplaced`
