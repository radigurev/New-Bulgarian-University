package com.inf.olympics.domain.ranking;

import com.inf.olympics.application.dto.RankingEntryDto;
import com.inf.olympics.domain.model.CompetitionType;
import com.inf.olympics.domain.model.Medal;
import com.inf.olympics.domain.model.ResultStatus;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.entity.BiathlonResultEntity;
import com.inf.olympics.infrastructure.persistence.repository.BiathlonResultRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

@Component
@RequiredArgsConstructor
public class BiathlonRankingStrategy implements RankingStrategy {

    private final BiathlonResultRepository biathlonResultRepository;

    @Value("${app.biathlon.penalty-seconds-per-miss:60}")
    private int penaltySecondsPerMiss;

    @Override
    public CompetitionType supports() {
        return CompetitionType.BIATHLON;
    }

    @Override
    public List<RankingEntryDto> rank(Long competitionId) {
        List<BiathlonResultEntity> classified = biathlonResultRepository.findAllByCompetitionId(competitionId).stream()
                .filter(this::isClassified)
                .sorted(Comparator.comparing(this::finalTime))
                .toList();

        List<RankingEntryDto> ranking = new ArrayList<>(classified.size());
        int position = 1;
        for (BiathlonResultEntity r : classified) {
            ranking.add(toEntry(position, r.getAthlete(), finalTime(r)));
            position++;
        }
        return ranking;
    }

    boolean isClassified(BiathlonResultEntity r) {
        return r.getStatus() == ResultStatus.FINISHED && r.getBaseTimeSeconds() != null;
    }

    BigDecimal finalTime(BiathlonResultEntity r) {
        return r.getBaseTimeSeconds().add(BigDecimal.valueOf((long) r.getMisses() * penaltySecondsPerMiss));
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
