package com.inf.olympics.unit;

import com.inf.olympics.application.dto.RankingEntryDto;
import com.inf.olympics.domain.model.Gender;
import com.inf.olympics.domain.model.Medal;
import com.inf.olympics.domain.model.ResultStatus;
import com.inf.olympics.domain.ranking.SlalomRankingStrategy;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.entity.CompetitionEntity;
import com.inf.olympics.infrastructure.persistence.entity.SlalomResultEntity;
import com.inf.olympics.infrastructure.persistence.repository.SlalomResultRepository;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.BDDMockito.given;

@ExtendWith(MockitoExtension.class)
class SlalomRankingStrategyTest {

    @Mock
    private SlalomResultRepository slalomResultRepository;

    @InjectMocks
    private SlalomRankingStrategy strategy;

    @Test
    void rank_sortsByTotalTimeAndAssignsMedals_excludingDnf() {
        given(slalomResultRepository.findAllByCompetitionId(7L)).willReturn(List.of(
                slalomFinished(1L, "Bravo", "55.000", "50.500"),
                slalomFinished(2L, "Alpha", "50.123", "49.000"),
                slalomFinished(3L, "Charlie", "60.000", "55.500"),
                slalomDnf(4L, "Delta")
        ));

        List<RankingEntryDto> ranking = strategy.rank(7L);

        assertThat(ranking).hasSize(3);
        assertThat(ranking.get(0).lastName()).isEqualTo("Alpha");
        assertThat(ranking.get(0).medal()).isEqualTo(Medal.GOLD);
        assertThat(ranking.get(1).medal()).isEqualTo(Medal.SILVER);
        assertThat(ranking.get(2).medal()).isEqualTo(Medal.BRONZE);
    }

    private SlalomResultEntity slalomFinished(long athleteId, String lastName, String r1, String r2) {
        SlalomResultEntity e = baseEntity(athleteId, lastName);
        e.setRunOneStatus(ResultStatus.FINISHED);
        e.setRunOneTimeSeconds(new BigDecimal(r1));
        e.setRunTwoStatus(ResultStatus.FINISHED);
        e.setRunTwoTimeSeconds(new BigDecimal(r2));
        return e;
    }

    private SlalomResultEntity slalomDnf(long athleteId, String lastName) {
        SlalomResultEntity e = baseEntity(athleteId, lastName);
        e.setRunOneStatus(ResultStatus.DNF);
        e.setRunTwoStatus(ResultStatus.DNS);
        return e;
    }

    private SlalomResultEntity baseEntity(long athleteId, String lastName) {
        AthleteEntity athlete = new AthleteEntity();
        athlete.setId(athleteId);
        athlete.setFirstName("First");
        athlete.setLastName(lastName);
        athlete.setCountry("AT");
        athlete.setGender(Gender.MALE);
        athlete.setDateOfBirth(LocalDate.of(2000, 1, 1));
        CompetitionEntity competition = new CompetitionEntity();
        competition.setId(7L);
        SlalomResultEntity e = new SlalomResultEntity();
        e.setAthlete(athlete);
        e.setCompetition(competition);
        return e;
    }
}
