package com.inf.cscb869_olympics.data.repo;

import com.inf.cscb869_olympics.data.entity.Athlete;
import com.inf.cscb869_olympics.data.entity.Gender;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface AthleteRepository extends JpaRepository<Athlete, Long> {

    List<Athlete> findAllByCountryIgnoreCase(String country);

    List<Athlete> findAllByGender(Gender gender);

    List<Athlete> findAllByLastNameContainingIgnoreCase(String lastName);
}
