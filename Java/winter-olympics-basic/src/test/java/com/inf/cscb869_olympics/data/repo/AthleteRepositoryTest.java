package com.inf.cscb869_olympics.data.repo;

import com.inf.cscb869_olympics.data.entity.Athlete;
import com.inf.cscb869_olympics.data.entity.Gender;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.orm.jpa.DataJpaTest;
import org.springframework.boot.test.autoconfigure.orm.jpa.TestEntityManager;

import java.time.LocalDate;
import java.util.List;

import static org.assertj.core.api.Assertions.assertThat;

@DataJpaTest
class AthleteRepositoryTest {

    @Autowired
    private TestEntityManager entityManager;

    @Autowired
    private AthleteRepository athleteRepository;

    @Test
    void findAllByCountryIgnoreCase_returnsMatchingAthletes() {
        entityManager.persistAndFlush(athlete("Hanna", "Lindholm", "Finland", Gender.FEMALE, "1998-04-12"));
        entityManager.persistAndFlush(athlete("Oskar", "Nikkanen", "FINLAND", Gender.MALE, "1995-09-01"));
        entityManager.persistAndFlush(athlete("Marco", "Rossi", "Italy", Gender.MALE, "2000-01-15"));

        List<Athlete> found = athleteRepository.findAllByCountryIgnoreCase("finland");

        assertThat(found).hasSize(2);
        assertThat(found).extracting(Athlete::getLastName).containsExactlyInAnyOrder("Lindholm", "Nikkanen");
    }

    @Test
    void findAllByLastNameContainingIgnoreCase_returnsPartialMatches() {
        entityManager.persistAndFlush(athlete("Eva", "Petrova", "Bulgaria", Gender.FEMALE, "1999-03-21"));
        entityManager.persistAndFlush(athlete("Ivan", "Petrov", "Bulgaria", Gender.MALE, "1996-06-06"));
        entityManager.persistAndFlush(athlete("Anna", "Ivanova", "Bulgaria", Gender.FEMALE, "2001-12-12"));

        List<Athlete> found = athleteRepository.findAllByLastNameContainingIgnoreCase("petr");

        assertThat(found).hasSize(2);
    }

    private static Athlete athlete(String first, String last, String country, Gender gender, String dob) {
        Athlete a = new Athlete();
        a.setFirstName(first);
        a.setLastName(last);
        a.setCountry(country);
        a.setGender(gender);
        a.setDateOfBirth(LocalDate.parse(dob));
        return a;
    }
}
