# ForumGuard.ModelTrainer

Offline NAS-BERT TextClassification trainer for ForumGuard. Transforms the hand-authored
`Text,Label` seed corpus into a `model.zip` that the running web app loads through
`PredictionEnginePool<ModelInput, ModelOutput>.FromFile(...)`.

Specs: **SDD-FORUM-024** (this trainer + seed contract + packaging) and **SDD-FORUM-020**
(runtime scoring + the shared `ModelInput` / `ModelOutput` schema).

> The default build references only the **managed** ML.NET packages
> (`Microsoft.ML`, `Microsoft.ML.TorchSharp`). Training also needs the **native** `libtorch-cpu`
> backend, which is a large download deliberately **not** referenced here. Add it only when you are
> ready to train (step a below).

---

## Seed corpus

`seed-comments.csv` (copied to the build output) is the starter "comments written by you" corpus:

- Header row exactly `Text,Label`, UTF-8 encoded.
- `Label` is exactly `Clean` or `Toxic`.
- Text containing commas/quotes is RFC-4180 quoted.
- Currently 55 rows (28 `Clean`, 27 `Toxic`). Extend it with any plain-text editor and re-run the
  trainer — no code changes required. Minimums enforced: 10 rows total, 2 per class, both classes present.

---

## Enable NAS-BERT (one-time checklist)

### (a) Add the native libtorch backend to this trainer

This is the large (~GB) download. Run once from the repository root:

```bash
dotnet add src/ForumGuard.ModelTrainer package libtorch-cpu
```

(For a CUDA GPU, use `libtorch-cuda-12.1` instead — the produced `model.zip` schema is identical.)

### (b) Train and export a model

```bash
dotnet run --project src/ForumGuard.ModelTrainer -- --data seed-comments.csv --output ../models/model.zip
```

Arguments (all optional):

| Flag | Default | Meaning |
|---|---|---|
| `--data` | `seed-comments.csv` (next to the executable) | Path to the seed CSV. |
| `--output` | `model.zip` (next to the executable) | Destination `model.zip`. Point this at your `Moderation:ModelPath` for hot-reload. |
| `--validation-fraction` | `0.2` | Fraction held out for validation. |

CPU fine-tuning is **slow** (minutes to hours). The trainer prints `MicroAccuracy`, `MacroAccuracy`,
and a confusion matrix, then exports `model.zip`. Any abort (missing/invalid dataset, unwritable
output, cancellation, missing `libtorch-cpu`) prints a clear message and returns a **non-zero exit code**
without leaving a partial artifact.

### (c) Add the ML packages to ForumGuard.Web

The web app needs the native backend too, plus the TorchSharp trainer assembly the model depends on:

```bash
dotnet add src/ForumGuard.Web package Microsoft.ML.TorchSharp
dotnet add src/ForumGuard.Web package libtorch-cpu
```

(`Microsoft.ML` and `Microsoft.Extensions.ML` already flow in transitively through
`ForumGuard.Infrastructure`.)

### (d) Point config at the model and swap the registration (one line)

In `src/ForumGuard.Web/appsettings.json` set `Moderation:ModelPath` to the exported `model.zip` path.

In `src/ForumGuard.Web/Program.cs` replace the interim line:

```csharp
builder.Services.AddInterimNasBertAnalyzer();
```

with the production registration:

```csharp
builder.Services.AddNasBertCommentAnalyzer(builder.Configuration);
```

`AddNasBertCommentAnalyzer` registers
`AddPredictionEnginePool<ModelInput, ModelOutput>().FromFile(..., watchForChanges: true)` and the
`NasBertCommentAnalyzer` as the keyed `ICommentAnalyzer` under `AnalyzerKeys.NasBert` — the same key the
`MlToxicityHandler` injects. The interim keyword analyzer stays the default until you make this swap, so
the app keeps running end-to-end without a model or `libtorch-cpu`.

### (e) Hot-reload on retrain

Because the pool is registered with `watchForChanges: true`, re-running the trainer and overwriting the
file at `Moderation:ModelPath` makes the running app reload the new model **without a restart**
(SDD-FORUM-024 §2.3, SDD-FORUM-020 §2.2). Keep the `ModelInput` / `ModelOutput` schema unchanged across
retrainings; changing it is a breaking change coordinated with SDD-FORUM-020.
