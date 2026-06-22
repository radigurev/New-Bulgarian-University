package com.inf.olympics.application.service;

import com.inf.olympics.application.dto.AthleteDto;
import com.inf.olympics.application.dto.MedalCountDto;
import com.inf.olympics.application.dto.OlympicsStatsDto;
import com.inf.olympics.application.dto.RankingEntryDto;
import com.inf.olympics.application.mapper.AthleteMapper;
import com.inf.olympics.domain.model.Medal;
import com.inf.olympics.domain.ranking.RankingService;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.entity.CompetitionEntity;
import com.inf.olympics.infrastructure.persistence.repository.AthleteRepository;
import com.inf.olympics.infrastructure.persistence.repository.CompetitionRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.time.Period;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@Service
@RequiredArgsConstructor
public class OlympicsAggregateService {

    private final AthleteRepository athleteRepository;
    private final CompetitionRepository competitionRepository;
    private final RankingService rankingService;
    private final AthleteMapper athleteMapper;

    @Transactional(readOnly = true)
    public OlympicsStatsDto getStats() {
        List<AthleteEntity> athletes = athleteRepository.findAll();
        List<CompetitionEntity> competitions = competitionRepository.findAll();

        double averageAge = athletes.stream()
                .mapToInt(a -> Period.between(a.getDateOfBirth(), LocalDate.now()).getYears())
                .average()
                .orElse(0d);

        List<AthleteEntity> medalists = new ArrayList<>();
        Map<String, long[]> medalsPerCountry = new HashMap<>();

        for (CompetitionEntity competition : competitions) {
            if (!competition.isFinished()) {
                continue;
            }
            List<RankingEntryDto> ranking = rankingService.rank(competition.getId());
            for (RankingEntryDto entry : ranking) {
                if (entry.medal() == null) {
                    continue;
                }
                AthleteEntity athlete = athleteRepository.findById(entry.athleteId()).orElse(null);
                if (athlete == null) {
                    continue;
                }
                medalists.add(athlete);
                long[] counts = medalsPerCountry.computeIfAbsent(athlete.getCountry(), k -> new long[3]);
                int idx = entry.medal() == Medal.GOLD ? 0 : entry.medal() == Medal.SILVER ? 1 : 2;
                counts[idx]++;
            }
        }

        AthleteEntity youngest = medalists.stream()
                .max(Comparator.comparing(AthleteEntity::getDateOfBirth))
                .orElse(null);
        AthleteEntity oldest = medalists.stream()
                .min(Comparator.comparing(AthleteEntity::getDateOfBirth))
                .orElse(null);

        List<MedalCountDto> medalTable = new ArrayList<>();
        for (Map.Entry<String, long[]> entry : medalsPerCountry.entrySet()) {
            long[] c = entry.getValue();
            medalTable.add(new MedalCountDto(entry.getKey(), c[0], c[1], c[2]));
        }
        medalTable.sort(Comparator
                .comparingLong(MedalCountDto::gold).reversed()
                .thenComparing(Comparator.comparingLong(MedalCountDto::silver).reversed())
                .thenComparing(Comparator.comparingLong(MedalCountDto::bronze).reversed()));

        AthleteDto youngestDto = youngest == null ? null : athleteMapper.toDto(youngest);
        AthleteDto oldestDto = oldest == null ? null : athleteMapper.toDto(oldest);

        return new OlympicsStatsDto(
                averageAge,
                athletes.size(),
                competitions.size(),
                youngestDto,
                oldestDto,
                medalTable
        );
    }
}
