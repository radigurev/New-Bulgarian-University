package com.inf.olympics.unit;

import com.inf.olympics.api.exception.RegistrationException;
import com.inf.olympics.application.mapper.CompetitionMapper;
import com.inf.olympics.application.service.AthleteService;
import com.inf.olympics.application.service.CompetitionService;
import com.inf.olympics.domain.model.CompetitionType;
import com.inf.olympics.domain.model.Gender;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.entity.CompetitionEntity;
import com.inf.olympics.infrastructure.persistence.repository.CompetitionRepository;
import com.inf.olympics.infrastructure.persistence.repository.RegistrationRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.time.LocalDate;
import java.util.Optional;

import static org.assertj.core.api.Assertions.assertThatThrownBy;
import static org.mockito.BDDMockito.given;

@ExtendWith(MockitoExtension.class)
class CompetitionServiceTest {

    @Mock
    private CompetitionRepository competitionRepository;
    @Mock
    private RegistrationRepository registrationRepository;
    @Mock
    private AthleteService athleteService;
    @Mock
    private CompetitionMapper competitionMapper;

    @InjectMocks
    private CompetitionService competitionService;

    private CompetitionEntity competition;
    private AthleteEntity athlete;

    @BeforeEach
    void setUp() {
        competition = new CompetitionEntity();
        competition.setId(10L);
        competition.setName("Men Slalom");
        competition.setType(CompetitionType.SKI_SLALOM);
        competition.setGender(Gender.MALE);
        competition.setMinAge(18);
        competition.setHeldOn(LocalDate.of(2026, 2, 14));

        athlete = new AthleteEntity();
        athlete.setId(20L);
        athlete.setGender(Gender.MALE);
        athlete.setDateOfBirth(LocalDate.of(2000, 1, 1));
    }

    @Test
    void register_rejectsWhenCompetitionFinished() {
        competition.setFinished(true);
        given(competitionRepository.findById(10L)).willReturn(Optional.of(competition));

        assertThatThrownBy(() -> competitionService.register(10L, 20L))
                .isInstanceOf(RegistrationException.class)
                .hasMessageContaining("already finished");
    }

    @Test
    void register_rejectsWhenGenderMismatch() {
        athlete.setGender(Gender.FEMALE);
        given(competitionRepository.findById(10L)).willReturn(Optional.of(competition));
        given(athleteService.requireById(20L)).willReturn(athlete);

        assertThatThrownBy(() -> competitionService.register(10L, 20L))
                .isInstanceOf(RegistrationException.class)
                .hasMessageContaining("gender");
    }

    @Test
    void register_rejectsWhenUnderMinAge() {
        athlete.setDateOfBirth(LocalDate.of(2015, 1, 1));
        given(competitionRepository.findById(10L)).willReturn(Optional.of(competition));
        given(athleteService.requireById(20L)).willReturn(athlete);

        assertThatThrownBy(() -> competitionService.register(10L, 20L))
                .isInstanceOf(RegistrationException.class)
                .hasMessageContaining("minimum age");
    }

    @Test
    void register_rejectsDuplicate() {
        given(competitionRepository.findById(10L)).willReturn(Optional.of(competition));
        given(athleteService.requireById(20L)).willReturn(athlete);
        given(registrationRepository.existsByCompetitionIdAndAthleteId(10L, 20L)).willReturn(true);

        assertThatThrownBy(() -> competitionService.register(10L, 20L))
                .isInstanceOf(RegistrationException.class)
                .hasMessageContaining("already registered");
    }
}
