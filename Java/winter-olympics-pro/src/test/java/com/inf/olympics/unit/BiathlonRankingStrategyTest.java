package com.inf.olympics.unit;

import com.inf.olympics.application.dto.RankingEntryDto;
import com.inf.olympics.domain.model.Gender;
import com.inf.olympics.domain.ranking.BiathlonRankingStrategy;
import com.inf.olympics.domain.model.ResultStatus;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.entity.BiathlonResultEntity;
import com.inf.olympics.infrastructure.persistence.entity.CompetitionEntity;
import com.inf.olympics.infrastructure.persistence.repository.BiathlonResultRepository;
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
class BiathlonRankingStrategyTest {

    @Mock
    private BiathlonResultRepository biathlonResultRepository;

    @InjectMocks
    private BiathlonRankingStrategy strategy;

    @BeforeEach
    void setUp() {
        ReflectionTestUtils.setField(strategy, "penaltySecondsPerMiss", 60);
    }

    @Test
    void rank_appliesPenaltiesExcludesDnfAndAssignsMedals() {
        given(biathlonResultRepository.findAllByCompetitionId(9L)).willReturn(List.of(
                result(1L, "ZeroMisses", "1500.000", 0),
                result(2L, "Penalized", "1450.000", 2),
                result(3L, "Loser", "1700.000", 1),
                dnf(4L, "Dropped")
        ));

        List<RankingEntryDto> ranking = strategy.rank(9L);

        assertThat(ranking).hasSize(3);
        assertThat(ranking.get(0).lastName()).isEqualTo("ZeroMisses");
        assertThat(ranking.get(0).totalTimeSeconds()).isEqualByComparingTo("1500.000");
        assertThat(ranking.get(1).lastName()).isEqualTo("Penalized");
        assertThat(ranking.get(1).totalTimeSeconds()).isEqualByComparingTo("1570.000");
        assertThat(ranking.get(2).lastName()).isEqualTo("Loser");
        assertThat(ranking.get(2).totalTimeSeconds()).isEqualByComparingTo("1760.000");
    }

    private BiathlonResultEntity result(long athleteId, String lastName, String base, int misses) {
        BiathlonResultEntity r = baseEntity(athleteId, lastName);
        r.setStatus(ResultStatus.FINISHED);
        r.setBaseTimeSeconds(new BigDecimal(base));
        r.setMisses(misses);
        return r;
    }

    private BiathlonResultEntity dnf(long athleteId, String lastName) {
        BiathlonResultEntity r = baseEntity(athleteId, lastName);
        r.setStatus(ResultStatus.DNF);
        return r;
    }

    private BiathlonResultEntity baseEntity(long athleteId, String lastName) {
        AthleteEntity athlete = new AthleteEntity();
        athlete.setId(athleteId);
        athlete.setFirstName("First");
        athlete.setLastName(lastName);
        athlete.setCountry("BG");
        athlete.setGender(Gender.FEMALE);
        athlete.setDateOfBirth(LocalDate.of(2000, 1, 1));
        CompetitionEntity competition = new CompetitionEntity();
        competition.setId(9L);
        BiathlonResultEntity e = new BiathlonResultEntity();
        e.setAthlete(athlete);
        e.setCompetition(competition);
        return e;
    }
}
