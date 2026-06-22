package com.inf.cscb869_olympics.data.repo;

import com.inf.cscb869_olympics.data.entity.Competition;
import com.inf.cscb869_olympics.data.entity.CompetitionType;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface CompetitionRepository extends JpaRepository<Competition, Long> {

    List<Competition> findAllByType(CompetitionType type);

    List<Competition> findAllByFinished(boolean finished);
}
