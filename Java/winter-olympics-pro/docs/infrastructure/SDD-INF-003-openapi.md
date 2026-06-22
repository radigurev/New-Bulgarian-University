# SDD-INF-003 — OpenAPI / Swagger UI

| Field | Value |
|---|---|
| ID | SDD-INF-003 |
| Title | OpenAPI specification and Swagger UI |
| Status | Implemented |
| Version | 1.0 |
| Last updated | 2026-05-18 |
| Related specs | [[SDD-CORE-006]] |

## 1. Context

The API is documented for graders, integrators, and for ad-hoc testing via
Swagger UI. The OpenAPI document is auto-generated from controller annotations
by `springdoc-openapi`.

## 2. Behavior

- **MUST** expose the OpenAPI 3 document at `/v3/api-docs` and Swagger UI at `/swagger-ui.html`.
- **MUST** declare a single security scheme `bearerAuth` of type `http`/`bearer`/`JWT` and apply it as the default security requirement.
- **MUST** annotate controllers with `@Tag` and key endpoints with `@Operation(summary = ...)`.
- **MUST** mark `/api/v1/auth/**` and `/api/v1/public/**` as security-free.

## 3. Validation

n/a.

## 4. Error Rules

| Trigger | Behavior |
|---|---|
| Springdoc missing on classpath | Endpoints unavailable; controllers still functional. |

## 5. Versioning

v1.0.

## 6. Test Plan

| Test | Type | Verifies |
|---|---|---|
| (Manual) | UI | `/swagger-ui.html` lists all controllers and lets the user authenticate with a bearer token. |

## 7. Change Log

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-05-18 | Initial spec. |
