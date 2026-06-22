package com.inf.olympics.infrastructure.persistence.repository;

import com.inf.olympics.infrastructure.persistence.entity.CompetitionEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface CompetitionRepository extends JpaRepository<CompetitionEntity, Long> {

    List<CompetitionEntity> findAllByFinished(boolean finished);
}
