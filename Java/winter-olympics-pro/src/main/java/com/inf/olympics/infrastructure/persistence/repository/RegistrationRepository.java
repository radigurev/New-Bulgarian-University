package com.inf.olympics.infrastructure.persistence.repository;

import com.inf.olympics.infrastructure.persistence.entity.RegistrationEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface RegistrationRepository extends JpaRepository<RegistrationEntity, Long> {

    List<RegistrationEntity> findAllByCompetitionId(Long competitionId);

    List<RegistrationEntity> findAllByAthleteId(Long athleteId);

    Optional<RegistrationEntity> findByCompetitionIdAndAthleteId(Long competitionId, Long athleteId);

    boolean existsByCompetitionIdAndAthleteId(Long competitionId, Long athleteId);
}
