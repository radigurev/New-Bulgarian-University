package com.inf.cscb869_olympics.service.impl;

import com.inf.cscb869_olympics.data.entity.Athlete;
import com.inf.cscb869_olympics.data.entity.Competition;
import com.inf.cscb869_olympics.data.entity.CompetitionType;
import com.inf.cscb869_olympics.data.entity.Gender;
import com.inf.cscb869_olympics.data.entity.ResultStatus;
import com.inf.cscb869_olympics.data.entity.SlalomResult;
import com.inf.cscb869_olympics.data.repo.SlalomResultRepository;
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
class SlalomServiceImplTest {

    @Mock
    private SlalomResultRepository slalomResultRepository;

    @Mock
    private CompetitionService competitionService;

    @Mock
    private AthleteService athleteService;

    @InjectMocks
    private SlalomServiceImpl slalomService;

    private Competition competition;

    @BeforeEach
    void setUp() {
        ReflectionTestUtils.setField(slalomService, "qualifiersForRunTwo", 3);
        competition = new Competition();
        competition.setId(10L);
        competition.setType(CompetitionType.SKI_SLALOM);
        competition.setGender(Gender.MALE);
        competition.setMinAge(18);
        competition.setHeldOn(LocalDate.of(2026, 2, 14));
    }

    @Test
    void getRanking_sortsByTotalTimeAndAssignsMedals() {
        given(competitionService.getCompetitionEntity(10L)).willReturn(competition);
        given(slalomResultRepository.findAllByCompetitionId(10L)).willReturn(List.of(
                slalomFinished(1L, "Alpha", "50.123", "49.000"),
                slalomFinished(2L, "Bravo", "51.000", "50.500"),
                slalomFinished(3L, "Charlie", "52.000", "51.500"),
                slalomDnf(4L, "Delta")
        ));

        List<RankingEntryDTO> ranking = slalomService.getRanking(10L);

        assertThat(ranking).hasSize(3);
        assertThat(ranking.get(0).getLastName()).isEqualTo("Alpha");
        assertThat(ranking.get(0).getMedal()).isEqualTo("GOLD");
        assertThat(ranking.get(1).getLastName()).isEqualTo("Bravo");
        assertThat(ranking.get(1).getMedal()).isEqualTo("SILVER");
        assertThat(ranking.get(2).getLastName()).isEqualTo("Charlie");
        assertThat(ranking.get(2).getMedal()).isEqualTo("BRONZE");
    }

    @Test
    void getRunTwoStartList_picksTopByRunOneAndReversesOrder() {
        given(competitionService.getCompetitionEntity(10L)).willReturn(competition);
        given(slalomResultRepository.findAllByCompetitionId(10L)).willReturn(List.of(
                slalomRunOneOnly(1L, "Fast", "55.123"),
                slalomRunOneOnly(2L, "Mid", "56.456"),
                slalomRunOneOnly(3L, "Slow", "58.789"),
                slalomRunOneOnly(4L, "Cut", "60.123")
        ));

        List<RankingEntryDTO> startList = slalomService.getRunTwoStartList(10L);

        assertThat(startList).hasSize(3);
        assertThat(startList.get(0).getLastName()).isEqualTo("Slow");
        assertThat(startList.get(2).getLastName()).isEqualTo("Fast");
    }

    private SlalomResult slalomFinished(long athleteId, String lastName, String run1, String run2) {
        SlalomResult r = baseResult(athleteId, lastName);
        r.setRunOneStatus(ResultStatus.FINISHED);
        r.setRunOneTimeSeconds(new BigDecimal(run1));
        r.setRunTwoStatus(ResultStatus.FINISHED);
        r.setRunTwoTimeSeconds(new BigDecimal(run2));
        return r;
    }

    private SlalomResult slalomDnf(long athleteId, String lastName) {
        SlalomResult r = baseResult(athleteId, lastName);
        r.setRunOneStatus(ResultStatus.DNF);
        r.setRunTwoStatus(ResultStatus.DNS);
        return r;
    }

    private SlalomResult slalomRunOneOnly(long athleteId, String lastName, String run1) {
        SlalomResult r = baseResult(athleteId, lastName);
        r.setRunOneStatus(ResultStatus.FINISHED);
        r.setRunOneTimeSeconds(new BigDecimal(run1));
        r.setRunTwoStatus(ResultStatus.DNS);
        return r;
    }

    private SlalomResult baseResult(long athleteId, String lastName) {
        Athlete athlete = new Athlete();
        athlete.setId(athleteId);
        athlete.setFirstName("Test");
        athlete.setLastName(lastName);
        athlete.setCountry("AT");
        athlete.setGender(Gender.MALE);
        athlete.setDateOfBirth(LocalDate.of(2000, 1, 1));
        SlalomResult r = new SlalomResult();
        r.setCompetition(competition);
        r.setAthlete(athlete);
        return r;
    }
}
