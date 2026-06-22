package com.inf.olympics.infrastructure.persistence.repository;

import com.inf.olympics.infrastructure.persistence.entity.BiathlonResultEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface BiathlonResultRepository extends JpaRepository<BiathlonResultEntity, Long> {

    List<BiathlonResultEntity> findAllByCompetitionId(Long competitionId);

    Optional<BiathlonResultEntity> findByCompetitionIdAndAthleteId(Long competitionId, Long athleteId);
}
