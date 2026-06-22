package com.inf.cscb869_olympics.data.repo;

import com.inf.cscb869_olympics.data.entity.CompetitionRegistration;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface CompetitionRegistrationRepository extends JpaRepository<CompetitionRegistration, Long> {

    List<CompetitionRegistration> findAllByCompetitionId(Long competitionId);

    List<CompetitionRegistration> findAllByAthleteId(Long athleteId);

    Optional<CompetitionRegistration> findByCompetitionIdAndAthleteId(Long competitionId, Long athleteId);

    boolean existsByCompetitionIdAndAthleteId(Long competitionId, Long athleteId);
}
