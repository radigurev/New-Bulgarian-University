# SDD-INF-002 — Observability

| Field | Value |
|---|---|
| ID | SDD-INF-002 |
| Title | Logging, AOP service-call tracing, Actuator endpoints |
| Status | Implemented |
| Version | 1.0 |
| Last updated | 2026-05-18 |
| Related specs | (none) |

## 1. Context

The application needs basic observability for development and for grading:
structured logs, easy health/metrics endpoints, and per-service-call timing
without sprinkling stopwatches through the code.

## 2. Behavior

- **MUST** declare a single `LoggingAspect` `@Aspect @Component` matching `execution(* com.inf.olympics.application.service..*.*(..))` and `execution(* com.inf.olympics.domain.ranking..*Strategy.*(..))`.
- **MUST** log per-call duration in microseconds at `DEBUG` level and exceptions at `WARN` level.
- **MUST** expose Spring Boot Actuator endpoints `health`, `info`, `metrics` at `/actuator/**`.
- **MUST** keep `/actuator/**` publicly readable in dev. In prod, `management.endpoint.health.show-details: when-authorized` hides health detail behind auth.

## 3. Validation

n/a — operational concern.

## 4. Error Rules

| Trigger | Behavior |
|---|---|
| Actuator dependency missing | Application boots; endpoints simply not exposed. |
| AOP pointcut matches a non-existing package | No-op — aspect simply does not advise anything. |

## 5. Versioning

v1.0.

## 6. Test Plan

| Test | Type | Verifies |
|---|---|---|
| (Manual) | curl | `GET /actuator/health` returns 200 with status `UP`. |
| (Implicit via unit tests) | Unit | All service tests run through the aspect without exception. |

## 7. Change Log

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-05-18 | Initial spec. |
