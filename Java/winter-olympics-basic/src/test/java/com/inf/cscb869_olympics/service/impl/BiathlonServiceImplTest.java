package com.inf.cscb869_olympics.service.impl;

import com.inf.cscb869_olympics.data.entity.Athlete;
import com.inf.cscb869_olympics.data.entity.BiathlonResult;
import com.inf.cscb869_olympics.data.entity.Competition;
import com.inf.cscb869_olympics.data.entity.CompetitionType;
import com.inf.cscb869_olympics.data.entity.Gender;
import com.inf.cscb869_olympics.data.entity.ResultStatus;
import com.inf.cscb869_olympics.data.repo.BiathlonResultRepository;
import com.inf.cscb869_olympics.dto.RankingEntryDTO;
import com.inf.cscb869_olympics.service.AthleteService;
import com.inf.cscb869_olympics.service.CompetitionService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.test.util.ReflectionTestUtils;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.BDDMockito.given;

@ExtendWith(MockitoExtension.class)
class BiathlonServiceImplTest {

    @Mock
    private BiathlonResultRepository biathlonResultRepository;

    @Mock
    private CompetitionService competitionService;

    @Mock
    private AthleteService athleteService;

    @InjectMocks
    private BiathlonServiceImpl biathlonService;

    private Competition competition;

    @BeforeEach
    void setUp() {
        ReflectionTestUtils.setField(biathlonService, "penaltySecondsPerMiss", 60);
        competition = new Competition();
        competition.setId(20L);
        competition.setType(CompetitionType.BIATHLON);
        competition.setGender(Gender.FEMALE);
        competition.setMinAge(18);
        competition.setHeldOn(LocalDate.of(2026, 2, 16));
    }

    @Test
    void getRanking_appliesPenaltiesAndOrdersByFinalTime() {
        given(competitionService.getCompetitionEntity(20L)).willReturn(competition);
        given(biathlonResultRepository.findAllByCompetitionId(20L)).willReturn(List.of(
                result(1L, "Lone", "1500.000", 0),
                result(2L, "Penalized", "1450.000", 2),
                result(3L, "Loser", "1800.000", 1),
                dnf(4L, "Quitter")
        ));

        List<RankingEntryDTO> ranking = biathlonService.getRanking(20L);

        assertThat(ranking).hasSize(3);
        assertThat(ranking.get(0).getLastName()).isEqualTo("Lone");
        assertThat(ranking.get(0).getTotalTimeSeconds()).isEqualByComparingTo("1500.000");
        assertThat(ranking.get(1).getLastName()).isEqualTo("Penalized");
        assertThat(ranking.get(1).getTotalTimeSeconds()).isEqualByComparingTo("1570.000");
        assertThat(ranking.get(2).getLastName()).isEqualTo("Loser");
    }

    private BiathlonResult result(long athleteId, String lastName, String base, int misses) {
        BiathlonResult r = baseResult(athleteId, lastName);
        r.setStatus(ResultStatus.FINISHED);
        r.setBaseTimeSeconds(new BigDecimal(base));
        r.setMisses(misses);
        return r;
    }

    private BiathlonResult dnf(long athleteId, String lastName) {
        BiathlonResult r = baseResult(athleteId, lastName);
        r.setStatus(ResultStatus.DNF);
        return r;
    }

    private BiathlonResult baseResult(long athleteId, String lastName) {
        Athlete athlete = new Athlete();
        athlete.setId(athleteId);
        athlete.setFirstName("Test");
        athlete.setLastName(lastName);
        athlete.setCountry("BG");
        athlete.setGender(Gender.FEMALE);
        athlete.setDateOfBirth(LocalDate.of(2000, 1, 1));
        BiathlonResult r = new BiathlonResult();
        r.setCompetition(competition);
        r.setAthlete(athlete);
        return r;
    }
}
