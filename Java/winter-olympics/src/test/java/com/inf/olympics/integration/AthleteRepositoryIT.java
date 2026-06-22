package com.inf.olympics.integration;

import com.inf.olympics.domain.model.Gender;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.repository.AthleteRepository;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.orm.jpa.DataJpaTest;
import org.springframework.boot.test.autoconfigure.orm.jpa.TestEntityManager;
import org.springframework.context.annotation.Import;

import com.inf.olympics.config.ApplicationConfig;

import java.time.LocalDate;
import java.util.List;

import static org.assertj.core.api.Assertions.assertThat;

@DataJpaTest
@Import(ApplicationConfig.class)
class AthleteRepositoryIT {

    @Autowired
    private TestEntityManager entityManager;

    @Autowired
    private AthleteRepository athleteRepository;

    @Test
    void findAllByCountryIgnoreCase_matchesByCountry() {
        entityManager.persistAndFlush(athlete("Eva", "Petrova", "Bulgaria", Gender.FEMALE));
        entityManager.persistAndFlush(athlete("Ivan", "Petrov", "BULGARIA", Gender.MALE));
        entityManager.persistAndFlush(athlete("Marco", "Rossi", "Italy", Gender.MALE));

        List<AthleteEntity> found = athleteRepository.findAllByCountryIgnoreCase("bulgaria");

        assertThat(found).hasSize(2);
    }

    private AthleteEntity athlete(String first, String last, String country, Gender gender) {
        AthleteEntity a = new AthleteEntity();
        a.setFirstName(first);
        a.setLastName(last);
        a.setCountry(country);
        a.setGender(gender);
        a.setDateOfBirth(LocalDate.of(2000, 1, 1));
        return a;
    }
}
