# Cross-Reference Map

| Spec | Implementation key files | Tests |
|---|---|---|
| SDD-CORE-001 | `application/service/AthleteService.java`, `api/controller/AthleteController.java`, `infrastructure/persistence/repository/AthleteRepository.java`, `application/mapper/AthleteMapper.java`, `application/dto/CreateAthleteRequest.java` | `integration/AthleteControllerIT.java`, `integration/AthleteRepositoryIT.java` |
| SDD-CORE-002 | `application/service/CompetitionService.java`, `api/controller/CompetitionController.java`, `infrastructure/persistence/repository/CompetitionRepository.java`, `application/mapper/CompetitionMapper.java`, `infrastructure/persistence/repository/RegistrationRepository.java` | `unit/CompetitionServiceTest.java` |
| SDD-CORE-003 | `application/service/SlalomService.java`, `domain/ranking/SlalomRankingStrategy.java`, `infrastructure/persistence/repository/SlalomResultRepository.java` | `unit/SlalomRankingStrategyTest.java` |
| SDD-CORE-004 | `application/service/BiathlonService.java`, `domain/ranking/BiathlonRankingStrategy.java`, `infrastructure/persistence/repository/BiathlonResultRepository.java` | `unit/BiathlonRankingStrategyTest.java` |
| SDD-CORE-005 | `application/service/OlympicsAggregateService.java`, `api/controller/PublicController.java`, `api/controller/WebController.java` | — (covered indirectly by ranking tests + manual UI walkthrough) |
| SDD-CORE-006 | `api/controller/AuthController.java`, `application/service/UserAccountService.java`, `infrastructure/security/JwtService.java`, `infrastructure/security/JwtAuthenticationFilter.java`, `infrastructure/security/OlympicsUserDetailsService.java`, `config/SecurityConfig.java` | `integration/AthleteControllerIT.java` (auth matrix) |
| SDD-DOM-001 | `domain/ranking/RankingStrategy.java`, `domain/ranking/RankingService.java`, `domain/ranking/SlalomRankingStrategy.java`, `domain/ranking/BiathlonRankingStrategy.java`, `domain/model/Medal.java` | `unit/SlalomRankingStrategyTest.java`, `unit/BiathlonRankingStrategyTest.java` |
| SDD-INF-001 | `src/main/resources/db/changelog/db.changelog-master.yaml` + `changes/*.yaml` | — |
| SDD-INF-002 | `config/LoggingAspect.java`, `application.yml` (management.endpoints) | — |
| SDD-INF-003 | `config/OpenApiConfig.java`, `springdoc.*` properties | — |
| SDD-INT-001 | (no implementation — spec describes the boundary surface) | — |
