# ML Model Training & Packaging

| Field | Value |
|---|---|
| **Title** | ML Model Training & Packaging |
| **Spec ID** | SDD-FORUM-024 |
| **Category** | infrastructure |
| **Status** | Active |
| **Last updated** | 2026-06-21 |
| **Owner** | TBD |

> Greenfield spec — **authoritative**. It defines what WILL be built. No production code exists yet;
> all paths under `src/`, `data/`, and `tests/` in *Key Files (planned)* are **PLANNED/TARGET** locations.

---

## 1. Context & Scope

This spec defines the **offline ML training tooling** of ForumGuard: the
`ForumGuard.ModelTrainer` console application that fine-tunes the NAS-BERT TextClassification model,
the hand-authored **seed dataset** (`Text,Label` CSV) that constitutes the "comments written by you"
corpus the assignment requires, the training/validation/export workflow, and the packaging of the
result as a `model.zip` artifact that **SDD-FORUM-020** loads and scores at runtime.

This spec **produces** the artifact; **SDD-FORUM-020 consumes** it. The two specs share the exact same
`ModelInput { string Text }` / `ModelOutput { string PredictedLabel, float[] Score }` schema and the
binary label space `Clean` / `Toxic`. This spec realizes the "training pipeline shape" declared in
**SDD-FORUM-020 §2.3**.

### Patterns / responsibilities mandated by this spec

| Concern | Where |
|---|---|
| **Offline trainer** | `ForumGuard.ModelTrainer` console app — runs out-of-band, never at web-request time. |
| **Seed corpus** | `seed-comments.csv` — hand-authorable, extendable, UTF-8, header `Text,Label`. |
| **Training pipeline** | `MulticlassClassification.Trainers.TextClassification` (NAS-BERT roBERTa via `Microsoft.ML.TorchSharp` + `libtorch-cpu`). |
| **Packaging** | `mlContext.Model.Save(...)` → `model.zip`, loadable by `PredictionEnginePool.FromFile` (SDD-FORUM-020). |
| **Hot-reload coupling** | Re-exporting to `Moderation:ModelPath` triggers the running app's pool (`watchForChanges: true`) to reload. |

### Covered

- The `ForumGuard.ModelTrainer` console application as the **one-time / offline** trainer.
- The seed dataset contract: CSV with exactly the columns `Text,Label`, `Label ∈ { Clean, Toxic }`,
  UTF-8 encoding, a single header row, hand-authorable and extendable, with both classes present.
- The training pipeline: load CSV → map `Label` → key → `TextClassification`
  (`labelColumnName = "Label"`, `sentence1ColumnName = "Text"`) → map predicted key back to label string.
- Train/validation split, metric reporting (MicroAccuracy / MacroAccuracy / confusion matrix), and
  export to `model.zip` via `mlContext.Model.Save`.
- The retraining workflow and its `watchForChanges` hot-reload relationship with the running app.
- The hardware reality (CPU fine-tuning is slow; GPU optional) that justifies an offline trainer.
- Dataset-validation, pipeline-construction, and train-then-load-and-score round-trip behaviors.

### Excluded (covered elsewhere)

- Runtime scoring of the produced model, the `PredictionEnginePool` registration, the
  `NasBertCommentAnalyzer` Adapter, and the `ModelInput`/`ModelOutput` → `AnalysisResult` mapping —
  see **SDD-FORUM-020** (Comment Analysis).
- Binding and validation of `ModerationOptions` (including `Moderation:ModelPath`) from configuration —
  see **SDD-FORUM-023** (Configuration & Options).
- The Chain-of-Responsibility pipeline that invokes the analyzer — see **SDD-FORUM-021**.
- The comment lifecycle, `ToxicityLabel` enum definition, and persistence — see **SDD-FORUM-010** /
  **SDD-FORUM-022**.
- The web submission / auto-publish flow — see **SDD-FORUM-001**.

### Cross-referenced specs

- **SDD-FORUM-020** — Comment Analysis (NAS-BERT). **Consumes** the `model.zip` this spec produces and
  **shares** the `ModelInput` / `ModelOutput` schema and the `Clean` / `Toxic` label space. This spec
  realizes the training-pipeline shape declared in SDD-FORUM-020 §2.3.
- **SDD-FORUM-023** — Configuration & Options. Owns `Moderation:ModelPath`, the on-disk location to
  which the operator exports `model.zip` and from which the running app reloads it.
- **SDD-FORUM-010** — Comment Entity & Lifecycle. Defines the `ToxicityLabel { Clean, Toxic }` enum
  whose string members are the CSV `Label` values and the trained `PredictedLabel` values.

> **Spec-relationship note:** SDD-FORUM-024 is the **single authoritative source** for the offline
> trainer, the seed-CSV contract, and the `model.zip` packaging. SDD-FORUM-020 is authoritative for
> how the artifact is loaded and scored at runtime. The shared `ModelInput`/`ModelOutput` schema is
> **defined** by SDD-FORUM-020 and **honored** here.

### Key Files (planned — DO NOT yet exist)

| Planned path | Responsibility |
|---|---|
| `src/ForumGuard.ModelTrainer/Program.cs` | Console entry point: parse args, load + validate dataset, train, report metrics, export `model.zip`. |
| `src/ForumGuard.ModelTrainer/TrainingPipeline.cs` | Builds the `TextClassification` pipeline; trains; returns the trained `ITransformer` + metrics. |
| `src/ForumGuard.ModelTrainer/SeedDatasetLoader.cs` | Loads + validates `seed-comments.csv` (header, encoding, both classes, minimum row count). |
| `src/ForumGuard.ModelTrainer/TrainingDataRow.cs` | CSV row schema `{ string Text, string Label }` (`LoadColumn` mapping). |
| `src/ForumGuard.ModelTrainer/TrainingMetrics.cs` | Captured metrics (`MicroAccuracy`, `MacroAccuracy`, confusion matrix text). |
| `src/ForumGuard.ModelTrainer/seed-comments.csv` | Hand-written, extendable seed corpus (canonical location; `Text,Label`). |
| `data/seed-comments.csv` | Optional repo-root alternate location for the corpus; the trainer MUST accept a dataset path argument so either layout works. |
| `src/ForumGuard.Infrastructure/Analysis/ModelInput.cs` | `{ string Text }` — schema shared with SDD-FORUM-020 (defined there). |
| `src/ForumGuard.Infrastructure/Analysis/ModelOutput.cs` | `{ string PredictedLabel, float[] Score }` — schema shared with SDD-FORUM-020 (defined there). |
| `tests/ForumGuard.Tests/ModelTrainer/SeedDatasetLoaderTests.cs` | `[Unit]` dataset loading/validation tests. |
| `tests/ForumGuard.Tests/ModelTrainer/TrainingPipelineTests.cs` | `[Unit]` pipeline-construction + `[Integration]` train-then-load round-trip. |

### Hardware reality (why training is offline)

NAS-BERT/roBERTa fine-tuning via `Microsoft.ML.TorchSharp` runs on `libtorch-cpu` by default.
CPU fine-tuning is **slow** (minutes to hours depending on corpus size and epochs), which is why
training MUST be a deliberate, out-of-band step run by an operator and MUST NOT execute inside the web
application. A CUDA-capable GPU with `libtorch-cuda` is **optional** and faster; it does not change the
trained model schema or the produced artifact, so swapping the libtorch backend MUST NOT alter the
`model.zip` contract consumed by SDD-FORUM-020.

---

## 2. Behavior

The trainer transforms a hand-authored `Text,Label` corpus into a `model.zip` that SDD-FORUM-020 can
load with `PredictionEnginePool.FromFile`. Every MUST/SHOULD rule below is independently testable —
dataset validation and pipeline construction as pure units; train/export as an integration round-trip.

### 2.1 Happy path — train on the seed corpus and export a usable model

1. `ForumGuard.ModelTrainer` MUST be a standalone **console application** invoked offline by an
   operator; it MUST NOT be referenced by, started from, or executed within `ForumGuard.Web` at
   web-request time.
2. The trainer MUST accept a **dataset path** and an **output model path** (via CLI argument or a
   trainer settings file), defaulting the dataset to `seed-comments.csv` and the output to a
   `model.zip` path; the output path SHOULD be the operator's `Moderation:ModelPath` (SDD-FORUM-023)
   when the goal is a hot-reload deployment.
3. The trainer MUST load the seed CSV into rows of `{ string Text, string Label }` using a CSV reader
   configured with `hasHeader: true`, `separatorChar: ','`, and **UTF-8** encoding.
4. The training pipeline MUST be built as:
   map `Label` → key (`MapValueToKey(outputColumnName: "Label", inputColumnName: "Label")`) →
   `mlContext.MulticlassClassification.Trainers.TextClassification(labelColumnName: "Label", sentence1ColumnName: "Text")`
   → map the predicted key back to its string (`MapKeyToValue(outputColumnName: "PredictedLabel")`).
   This is the **TextClassification** NAS-BERT roBERTa trainer from `Microsoft.ML.TorchSharp`.
5. The label space MUST be exactly the two values `Clean` and `Toxic`; the trained model's
   `PredictedLabel` MUST be one of those two strings, matching the `ToxicityLabel` members defined by
   **SDD-FORUM-010** and the runtime contract of **SDD-FORUM-020**.
6. The trainer MUST split the loaded data into a **train** set and a **validation** set
   (e.g. `mlContext.Data.TrainTestSplit`, default validation fraction `0.2`) and MUST train the
   pipeline on the train set.
7. After training, the trainer MUST evaluate on the validation set and MUST report
   `MicroAccuracy`, `MacroAccuracy`, and the **confusion matrix** to the console operator.
8. The trainer MUST export the trained model with `mlContext.Model.Save(model, schema, outputPath)`
   producing a single `model.zip`; the saved schema MUST be the input schema of the training data so
   the artifact is loadable by `PredictionEnginePool<ModelInput, ModelOutput>.FromFile(...)` in
   SDD-FORUM-020.
9. The produced `model.zip` MUST, when loaded via `PredictionEnginePool.FromFile` with the
   `ModelInput`/`ModelOutput` schema of SDD-FORUM-020, accept a `ModelInput { Text }` and return a
   `ModelOutput { PredictedLabel, Score }` where `PredictedLabel ∈ { "Clean", "Toxic" }` and `Score`
   is a non-empty `float[]` aligned to the trained classes.

### 2.2 Seed dataset contract (the "comments written by you" corpus)

10. The seed CSV MUST have **exactly one header row** whose value is `Text,Label` (in that order) and
    data rows of the form `<comment text>,<label>` where `<label>` is exactly `Clean` or `Toxic`.
11. The file MUST be **UTF-8** encoded and MUST be hand-authorable and extendable: an operator MUST be
    able to add rows with a plain text editor and re-run the trainer without code changes.
12. The dataset MUST contain **both** classes (`Clean` and `Toxic`); a dataset with only one class
    MUST be rejected (see §2.4 and Error Rules E2).
13. The dataset MUST contain enough rows to fine-tune; the trainer MUST enforce a configurable minimum
    total row count (default minimum `10`, with at least `2` rows of each class) and MUST reject a
    smaller corpus (see §2.4 and Error Rules E3).
14. Text values that contain commas, quotes, or newlines MUST be RFC-4180 quoted; the loader MUST parse
    quoted fields correctly so a comment body containing a comma is one `Text` value, not two columns.

### 2.3 Retraining workflow and hot-reload coupling (with SDD-FORUM-020)

15. Retraining MUST be performed by re-running the trainer against an updated `seed-comments.csv`; the
    workflow is **edit corpus → run trainer → export `model.zip` → deploy to `Moderation:ModelPath`**.
16. When the operator exports (or copies) the new `model.zip` over the file at `Moderation:ModelPath`,
    the running web app's `PredictionEnginePool` registered with `watchForChanges: true`
    (**SDD-FORUM-020 §2.2**) SHOULD reload the model **without an application restart**. This spec MUST
    NOT itself perform the reload; it only produces an artifact at a path the running app watches.
17. The exported artifact MUST keep the same `ModelInput`/`ModelOutput` schema across retrainings so a
    reload never breaks the consumer; changing the schema is a **breaking** change requiring a version
    note here and in SDD-FORUM-020.

### 2.4 Edge case — dataset missing one class

18. When the loaded dataset contains rows of only one label (e.g. all `Clean` or all `Toxic`), the
    trainer MUST abort **before** invoking the TextClassification trainer and MUST report a clear
    single-class error to the console; it MUST NOT produce a `model.zip`.

### 2.5 Edge case — too few rows / malformed or missing header

19. When the dataset has fewer than the configured minimum rows (overall or per class), the trainer
    MUST abort with a clear "insufficient training data" error and MUST NOT produce a `model.zip`.
20. When the CSV is missing its header, has the wrong header columns, has a row whose `Label` is
    neither `Clean` nor `Toxic`, or is otherwise unparseable, the trainer MUST abort with a clear
    dataset-format error identifying the problem and MUST NOT produce a `model.zip`.

### 2.6 Edge case — output path not writable

21. When the configured output `model.zip` path is not writable (missing directory, locked file, or
    permission denied), the trainer MUST surface a clear export-failure error to the operator after
    training and MUST NOT leave a partial/corrupt `model.zip` at the destination.
22. A failed export MUST NOT be reported as success; the trainer's process exit code MUST be non-zero
    on any abort (E1–E5) so a deployment script can detect failure.

---

## 3. Validation Rules

### Field-level (seed CSV / `TrainingDataRow`)

| Field | Rule |
|---|---|
| CSV header | MUST be exactly `Text,Label` (two columns, that order, single header row). |
| CSV encoding | MUST be UTF-8. |
| `TrainingDataRow.Text` | MUST be non-null, non-whitespace; loaded from column index `0`. |
| `TrainingDataRow.Label` | MUST be exactly `"Clean"` or `"Toxic"`; loaded from column index `1`. Any other value invalidates the row. |
| Dataset path argument | MUST resolve to a readable file; absence/unreadable is Error Rule E1. |
| Output path argument | MUST resolve to a writable location; non-writable is Error Rule E5. |

### Cross-field / dataset-level

- **DV-1:** The set of distinct `Label` values across all rows MUST equal `{ Clean, Toxic }` — both
  present (rule 12); a strict subset is rejected (E2).
- **DV-2:** Total row count MUST be `>=` the configured minimum (default `10`) and each class MUST have
  `>=` the configured per-class minimum (default `2`) — rule 13 (E3).
- **DV-3:** Every data row MUST parse into exactly two fields after RFC-4180 quote handling; a row that
  parses to a different field count is a format error (E4).

### State-based / artifact

- **AV-1:** A `model.zip` MUST be produced **only** when DV-1, DV-2, and DV-3 all pass and training
  completes; any validation failure MUST short-circuit before export (no partial artifact).
- **AV-2:** The saved model's input schema MUST match the `ModelInput { string Text }` schema such that
  `PredictionEnginePool<ModelInput, ModelOutput>.FromFile` (SDD-FORUM-020) loads it without a schema
  mismatch error.
- **AV-3:** The trained `PredictedLabel` output MUST be drawn only from `{ "Clean", "Toxic" }`
  (rule 5); a model emitting any other label string is a defect.

---

## 4. Error Rules

The trainer is a **console application**; errors surface to the **console operator** and via a non-zero
process exit code — they are **not** end-user-facing. There is no API boundary.

| # | Trigger | Error type | Domain / process mapping | Operator-facing outcome |
|---|---|---|---|---|
| E1 | Dataset path is missing/empty or points to an unreadable file | notfound / configuration | `DatasetNotFoundException` (or argument failure) thrown by `SeedDatasetLoader`; process exits non-zero | Console prints the resolved path and "training dataset not found / unreadable"; no `model.zip` produced. |
| E2 | Dataset contains only one label class (DV-1 fails) | validation | `InvalidDatasetException` ("single-class dataset") from `SeedDatasetLoader`; training aborted before the trainer runs | Console states both `Clean` and `Toxic` rows are required; no `model.zip` produced. |
| E3 | Too few rows overall or per class (DV-2 fails) | validation | `InvalidDatasetException` ("insufficient training data") | Console states the required minimums and the actual counts; no `model.zip` produced. |
| E4 | CSV malformed: missing/wrong header, bad `Label` value, or unparseable row (DV-3) | validation | `InvalidDatasetException` ("dataset format error") identifying the offending row/header | Console prints the format problem; no `model.zip` produced. |
| E5 | Output `model.zip` path not writable / `Model.Save` fails after training | conflict / IO | `ModelExportException` (wraps the IO/ML.NET save failure) thrown from the export step | Console reports the export failure; no partial/corrupt artifact left at the destination; exit code non-zero. |
| E6 | Training cancelled by the operator (Ctrl+C) | (cooperative cancellation) | `OperationCanceledException` propagates | Process aborts; no `model.zip` produced. |

**No-silent-success principle (MUST):** any abort (E1–E6) MUST result in **no** `model.zip` (or no
overwrite of an existing good artifact) and a **non-zero** exit code. The trainer MUST NOT print
success or emit a usable artifact when validation, training, or export fails.

---

## 5. Versioning Notes

- **v1 — Initial specification (2026-06-21).** Defines the `ForumGuard.ModelTrainer` offline console
  trainer; the `Text,Label` UTF-8 seed-CSV contract (both classes, minimum row counts, RFC-4180
  quoting); the `MapValueToKey → TextClassification(labelColumnName=Label, sentence1ColumnName=Text) →
  MapKeyToValue` pipeline (NAS-BERT roBERTa via `Microsoft.ML.TorchSharp` + `libtorch-cpu`); the
  train/validation split with MicroAccuracy/MacroAccuracy/confusion-matrix reporting; export to
  `model.zip` via `mlContext.Model.Save` loadable by `PredictionEnginePool.FromFile` (SDD-FORUM-020);
  the retraining + `watchForChanges` hot-reload coupling; the offline/hardware rationale; and the
  single-class / too-few-rows / malformed-CSV / unwritable-output edge cases. Non-breaking (greenfield
  first version). Changing the `model.zip` input/output schema in a future version is **breaking** and
  MUST be coordinated with SDD-FORUM-020.

---

## 6. Test Plan

Business tests reference this spec via `[Category("SDD-FORUM-024")]`. Test names follow
`MethodName_Scenario_ExpectedResult`. Dataset validation and pipeline construction are pure `[Unit]`
tests; the train-then-load-and-score round-trip crosses the ML.NET / file-system boundary and is an
`[Integration]` test against a tiny fixture corpus.

### Unit — seed dataset loading & validation (`SeedDatasetLoader`)

```
Required tests:

- Load_ValidTwoClassCsv_ReturnsAllRowsWithTextAndLabel                        [Unit]
- Load_MissingDatasetFile_ThrowsDatasetNotFoundException                      [Unit]
- Load_SingleClassDataset_ThrowsInvalidDatasetExceptionSingleClass            [Unit]
- Load_TooFewRowsOverall_ThrowsInvalidDatasetExceptionInsufficientData        [Unit]
- Load_TooFewRowsForOneClass_ThrowsInvalidDatasetExceptionInsufficientData    [Unit]
- Load_MissingHeaderRow_ThrowsInvalidDatasetExceptionFormat                   [Unit]
- Load_WrongHeaderColumns_ThrowsInvalidDatasetExceptionFormat                 [Unit]
- Load_LabelNotCleanOrToxic_ThrowsInvalidDatasetExceptionFormat               [Unit]
- Load_QuotedTextContainingComma_ParsesAsSingleTextField                      [Unit]
- Load_NonUtf8OrWhitespaceText_FailsValidation                                [Unit]
```

### Unit — training pipeline construction (`TrainingPipeline`, trainer not executed)

```
- BuildPipeline_UsesTextClassificationTrainer_WithLabelAndTextColumns         [Unit]
- BuildPipeline_MapsLabelToKeyAndPredictedKeyBackToValue                      [Unit]
- BuildPipeline_LabelSpace_IsExactlyCleanAndToxic                            [Unit]
```

### Integration — train, export, reload & score (tiny fixture corpus)

```
- Train_TinyTwoClassFixture_ProducesModelZipFile                              [Integration]
- Train_ThenLoadViaPredictionEnginePoolFromFile_ScoresWithoutSchemaMismatch   [Integration]
- Train_ExportedModel_PredictsLabelInCleanOrToxicForSampleText                [Integration]
- Train_ReportsMicroAndMacroAccuracyAndConfusionMatrix                        [Integration]
- Train_OutputPathNotWritable_ThrowsModelExportExceptionAndLeavesNoArtifact   [Integration]
- Train_SingleClassFixture_AbortsBeforeTrainingAndWritesNoModelZip            [Integration]
- Train_ReExportToWatchedPath_NewModelZipHasSameInputOutputSchema             [Integration]
```
