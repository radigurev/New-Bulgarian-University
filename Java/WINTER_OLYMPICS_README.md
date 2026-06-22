# Winter Olympics — Two Spring Boot deliverables for CSCB869

This directory contains the materials and two candidate solutions for the
CSCB869 Java Web Services final project (NBU, Spring 2025/2026, lecturer
Hristina Kostadinova).

Project spec PDF: [`CSCB869_Kostadinova_Winter_Olympics.pdf`](CSCB869_Kostadinova_Winter_Olympics.pdf)
Course materials (PPTX + reference projects): [`source/`](source/)
PPTX text extracts and unzipped projects: [`_extracted/`](_extracted/)

## The two deliverables

### 1. [`winter-olympics-basic/`](winter-olympics-basic/) — strictly within the syllabus

- Gradle, Spring Boot 3.3, Java 17
- Mirrors the instructor's `pharmacy-application` reference style exactly
  (package layout, `BaseEntity` + ModelMapper + `MapperUtil` + DTO/ViewModel
  parallel layers, in-memory + JPA `UserDetails`, form login, `@ControllerAdvice`)
- MySQL with `ddl-auto=update` (no migration tool)
- Thymeleaf UI + REST API side by side
- `@DataJpaTest`, `@WebMvcTest`, `@SpringBootTest` tests in the style shown
  in pharmacy v1.8
- 6 SDD specs in `docs/features/`

**Safe pick for grading** — every concept used was taught in lectures.

### 2. [`winter-olympics-pro/`](winter-olympics-pro/) — professional, beyond the syllabus where it pays off

- Maven, Spring Boot 3.3, Java 21
- Layered architecture: `domain` (pure enums, Strategy abstraction) / `application`
  (services, DTOs as `record`s, MapStruct) / `infrastructure` (JPA, security)
  / `api` (controllers, exception handler)
- Liquibase changelogs instead of `ddl-auto=update`
- JWT for `/api/**` + form-login for the web UI (stateless API, stateful UI)
- RFC 7807 `ProblemDetail` error bodies via `@RestControllerAdvice`
- Springdoc OpenAPI 3 + Swagger UI
- Strategy pattern for ranking — adding a new competition type means adding
  one bean
- AOP `LoggingAspect` for per-service-call duration logging
- Spring Profiles: `dev` (H2 + H2 console) and `prod` (MySQL via env vars)
- Spring Boot Actuator
- Testcontainers wired into Maven for future integration tests
- 10 SDD specs across `docs/core/`, `docs/domain/`, `docs/infrastructure/`,
  `docs/integration/`, with a `cross-reference-map.md`

**Showcase pick** — uses Liquibase, AOP, JWT, OpenAPI, MapStruct, ProblemDetail,
Profiles, Actuator. All of these were either shown conceptually in lectures
or appeared in reference projects (Liquibase: Week 13 slides; AOP: Week 11
slides + `pharmacy-spring-inside-aop`; JWT: pharmacy v1.6 OAuth2 wiring;
Profiles: Week 13 slides).

## Which one to submit?

If your goal is the highest mark with the least review surprise, submit
**both** and use the professional edition as the headline implementation,
keeping the basic edition as a "matches the lecture style verbatim" companion.

If you can only submit one, submit the basic edition first — it cleanly maps
to the lecture content and is easier to defend orally — and only swap to the
pro edition if you are confident discussing the extra pieces (JWT
implementation details, MapStruct vs ModelMapper trade-offs, Liquibase
changeset versioning, AOP advice types).

## Common ground between the two

Both implement the full functional brief from the project PDF:

- Athletes: id, name, country, gender, date of birth
- Competition rules: gender-segregated, minimum-age-enforced, times in seconds
  with 3 decimal places, DNF excluded
- **Ski Slalom**: two runs, qualifier cut for run 2 (configurable, default 30),
  run-2 start order reversed (slowest first), total time = sum of two runs
- **Biathlon**: base time + (misses × penalty seconds), configurable penalty
  (default 60s per miss)
- **Olympics aggregates**: medal table by country, average age, youngest /
  oldest medalist, totals
- **Users / roles**: athletes self-register and sign up for competitions,
  admins manage everything, public reads work without authentication

## Building locally

Neither project has a checked-in Gradle/Maven wrapper. To build:

```
# Basic
cd winter-olympics-basic
gradle bootRun       # or: gradle build

# Pro
cd winter-olympics-pro
mvn spring-boot:run  # or: mvn package
```

Make sure JDK 17 (basic) or JDK 21 (pro) is on `PATH`. For MySQL setup the
basic project expects user `education_user` / password `education_user` with
`CREATE DATABASE` rights; the pro project reads `DB_URL` / `DB_USER` /
`DB_PASSWORD` from the environment in the `prod` profile.
