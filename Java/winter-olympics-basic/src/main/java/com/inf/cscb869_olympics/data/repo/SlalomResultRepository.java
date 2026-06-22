package com.inf.cscb869_olympics.data.repo;

import com.inf.cscb869_olympics.data.entity.SlalomResult;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface SlalomResultRepository extends JpaRepository<SlalomResult, Long> {

    List<SlalomResult> findAllByCompetitionId(Long competitionId);

    Optional<SlalomResult> findByCompetitionIdAndAthleteId(Long competitionId, Long athleteId);
}
