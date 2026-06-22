package com.inf.olympics.domain.ranking;

import com.inf.olympics.application.dto.RankingEntryDto;
import com.inf.olympics.domain.model.CompetitionType;
import com.inf.olympics.domain.model.Medal;
import com.inf.olympics.domain.model.ResultStatus;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.entity.SlalomResultEntity;
import com.inf.olympics.infrastructure.persistence.repository.SlalomResultRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

@Component
@RequiredArgsConstructor
public class SlalomRankingStrategy implements RankingStrategy {

    private final SlalomResultRepository slalomResultRepository;

    @Override
    public CompetitionType supports() {
        return CompetitionType.SKI_SLALOM;
    }

    @Override
    public List<RankingEntryDto> rank(Long competitionId) {
        List<SlalomResultEntity> classified = slalomResultRepository.findAllByCompetitionId(competitionId).stream()
                .filter(SlalomRankingStrategy::isClassified)
                .sorted(Comparator.comparing(SlalomRankingStrategy::totalTime))
                .toList();

        List<RankingEntryDto> ranking = new ArrayList<>(classified.size());
        int position = 1;
        for (SlalomResultEntity r : classified) {
            ranking.add(toEntry(position, r.getAthlete(), totalTime(r)));
            position++;
        }
        return ranking;
    }

    static boolean isClassified(SlalomResultEntity r) {
        return r.getRunOneStatus() == ResultStatus.FINISHED
                && r.getRunTwoStatus() == ResultStatus.FINISHED
                && r.getRunOneTimeSeconds() != null
                && r.getRunTwoTimeSeconds() != null;
    }

    static BigDecimal totalTime(SlalomResultEntity r) {
        return r.getRunOneTimeSeconds().add(r.getRunTwoTimeSeconds());
    }

    private static RankingEntryDto toEntry(int position, AthleteEntity athlete, BigDecimal time) {
        return new RankingEntryDto(
                position,
                athlete.getId(),
                athlete.getFirstName(),
                athlete.getLastName(),
                athlete.getCountry(),
                time,
                Medal.forPosition(position)
        );
    }
}
