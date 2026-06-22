# Winter Olympics — Professional Edition

Course project for CSCB869 Java Web Services (Spring 2025/2026, NBU). This
edition mirrors the basic edition's domain but goes beyond the lecture syllabus
where doing so cleanly improves the project:

- Layered package structure (`domain` / `application` / `infrastructure` / `api`)
- Liquibase-managed schema (instead of `ddl-auto=update`)
- MapStruct mappers (instead of ModelMapper)
- RFC 7807 ProblemDetail error bodies (Spring 6 native)
- Stateless JWT for the REST API, form login for the UI
- Strategy pattern for ranking — adding a new competition type means adding one bean
- AOP logging aspect for service-call tracing
- Springdoc OpenAPI 3 + Swagger UI
- Spring Profiles (`dev` = H2, `prod` = MySQL)
- Spring Boot Actuator for health/metrics
- Testcontainers wired in for future Mongo/MySQL integration tests

It is **not** a microservices decomposition — that would be over-engineering
for a single-Olympics scope. See [`docs/integration/SDD-INT-001`](docs/integration/SDD-INT-001-future-integrations.md)
for the intentional out-of-scope list.

## Tech stack

| Layer | Choice |
|---|---|
| Build | Maven, Spring Boot 3.3.4, Java 21 |
| Web | Spring MVC REST + Thymeleaf views + Springdoc OpenAPI |
| Persistence | Spring Data JPA + Hibernate + Liquibase migrations + MySQL (prod) / H2 (dev/test) |
| Validation | Jakarta Bean Validation |
| Mapping | MapStruct |
| Security | Spring Security, BCrypt, JWT (jjwt 0.12.x) |
| Errors | `ProblemDetail` (RFC 7807) via `@RestControllerAdvice` |
| Observability | Spring Boot Actuator, AOP `LoggingAspect` |
| Testing | JUnit 5 + Mockito + AssertJ + Spring Boot Test + Testcontainers (for future use) |

## Running

```
# Dev (H2 in-memory, default profile)
./mvnw spring-boot:run

# Prod (MySQL — set DB_URL, DB_USER, DB_PASSWORD)
SPRING_PROFILES_ACTIVE=prod ./mvnw spring-boot:run
```

| URL | Purpose |
|---|---|
| http://localhost:8091 | Web UI |
| http://localhost:8091/swagger-ui.html | Swagger UI |
| http://localhost:8091/v3/api-docs | OpenAPI JSON |
| http://localhost:8091/actuator/health | Liveness |
| http://localhost:8091/h2 | H2 console (dev profile only) |

Default admin: `admin` / `admin123` (created by Liquibase changeset
`v1.0.1-001-seed-default-admin`).

> Regenerate the BCrypt hash for production by replacing the
> `password_hash` value in `db/changelog/changes/v1.0.1-seed-admin-role.yaml`
> with the output of `BCryptPasswordEncoder().encode("your-password")`.

## Specifications

See [`docs/README.md`](docs/README.md) for the full SDD index, and
[`docs/cross-reference-map.md`](docs/cross-reference-map.md) for the
spec ↔ code ↔ test linkage.

## Project layout

```
src/main/java/com/inf/olympics/
├── WinterOlympicsProApplication.java
├── api/                          REST controllers + exception handler + web (Thymeleaf) controller
│   ├── controller/
│   └── exception/
├── application/                  Use cases — services, DTOs, mappers
│   ├── dto/
│   ├── mapper/
│   └── service/
├── config/                       Cross-cutting Spring configuration
│   ├── ApplicationConfig.java
│   ├── LoggingAspect.java
│   ├── OpenApiConfig.java
│   └── SecurityConfig.java
├── domain/                       Pure domain — enums + Strategy abstraction
│   ├── model/
│   └── ranking/
└── infrastructure/               Adapters to outside world
    ├── persistence/
    │   ├── entity/
    │   └── repository/
    └── security/                 JwtService, JwtAuthenticationFilter, UserDetails wiring
```
