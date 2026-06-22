package com.inf.cscb869_olympics.data.repo;

import com.inf.cscb869_olympics.data.entity.BiathlonResult;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface BiathlonResultRepository extends JpaRepository<BiathlonResult, Long> {

    List<BiathlonResult> findAllByCompetitionId(Long competitionId);

    Optional<BiathlonResult> findByCompetitionIdAndAthleteId(Long competitionId, Long athleteId);
}
