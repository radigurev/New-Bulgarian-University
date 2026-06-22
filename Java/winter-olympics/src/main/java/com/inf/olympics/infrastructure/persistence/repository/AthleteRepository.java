package com.inf.olympics.infrastructure.persistence.repository;

import com.inf.olympics.domain.model.Gender;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface AthleteRepository extends JpaRepository<AthleteEntity, Long> {

    List<AthleteEntity> findAllByCountryIgnoreCase(String country);

    List<AthleteEntity> findAllByGender(Gender gender);

    List<AthleteEntity> findAllByLastNameContainingIgnoreCase(String lastName);
}
