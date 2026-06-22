package com.inf.olympics.infrastructure.persistence.repository;

import com.inf.olympics.infrastructure.persistence.entity.SlalomResultEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface SlalomResultRepository extends JpaRepository<SlalomResultEntity, Long> {

    List<SlalomResultEntity> findAllByCompetitionId(Long competitionId);

    Optional<SlalomResultEntity> findByCompetitionIdAndAthleteId(Long competitionId, Long athleteId);
}
