package com.inf.cscb869_olympics.service.impl;

import com.inf.cscb869_olympics.data.entity.Athlete;
import com.inf.cscb869_olympics.data.entity.Competition;
import com.inf.cscb869_olympics.data.entity.CompetitionType;
import com.inf.cscb869_olympics.data.repo.AthleteRepository;
import com.inf.cscb869_olympics.data.repo.CompetitionRepository;
import com.inf.cscb869_olympics.dto.AthleteDTO;
import com.inf.cscb869_olympics.dto.MedalCountDTO;
import com.inf.cscb869_olympics.dto.OlympicsStatsDTO;
import com.inf.cscb869_olympics.dto.RankingEntryDTO;
import com.inf.cscb869_olympics.service.BiathlonService;
import com.inf.cscb869_olympics.service.CompetitionService;
import com.inf.cscb869_olympics.service.OlympicsService;
import com.inf.cscb869_olympics.service.SlalomService;
import com.inf.cscb869_olympics.util.MapperUtil;
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
public class OlympicsServiceImpl implements OlympicsService {

    private final AthleteRepository athleteRepository;
    private final CompetitionRepository competitionRepository;
    private final CompetitionService competitionService;
    private final SlalomService slalomService;
    private final BiathlonService biathlonService;
    private final MapperUtil mapperUtil;

    @Override
    @Transactional(readOnly = true)
    public List<RankingEntryDTO> getRanking(Long competitionId) {
        Competition competition = competitionService.getCompetitionEntity(competitionId);
        return competition.getType() == CompetitionType.SKI_SLALOM
                ? slalomService.getRanking(competitionId)
                : biathlonService.getRanking(competitionId);
    }

    @Override
    @Transactional(readOnly = true)
    public OlympicsStatsDTO getStats() {
        List<Athlete> athletes = athleteRepository.findAll();
        List<Competition> competitions = competitionRepository.findAll();

        double averageAge = athletes.stream()
                .mapToInt(a -> Period.between(a.getDateOfBirth(), LocalDate.now()).getYears())
                .average()
                .orElse(0d);

        List<Athlete> medalists = new ArrayList<>();
        Map<String, long[]> medalsPerCountry = new HashMap<>();

        for (Competition competition : competitions) {
            if (!competition.isFinished()) {
                continue;
            }
            List<RankingEntryDTO> ranking = getRanking(competition.getId());
            for (RankingEntryDTO entry : ranking) {
                if (entry.getMedal() == null) {
                    continue;
                }
                Athlete athlete = athleteRepository.findById(entry.getAthleteId()).orElse(null);
                if (athlete == null) {
                    continue;
                }
                medalists.add(athlete);

                long[] counts = medalsPerCountry.computeIfAbsent(athlete.getCountry(), k -> new long[3]);
                switch (entry.getMedal()) {
                    case "GOLD" -> counts[0]++;
                    case "SILVER" -> counts[1]++;
                    case "BRONZE" -> counts[2]++;
                    default -> {
                    }
                }
            }
        }

        Athlete youngest = medalists.stream()
                .max(Comparator.comparing(Athlete::getDateOfBirth))
                .orElse(null);
        Athlete oldest = medalists.stream()
                .min(Comparator.comparing(Athlete::getDateOfBirth))
                .orElse(null);

        List<MedalCountDTO> medalTable = new ArrayList<>();
        for (Map.Entry<String, long[]> entry : medalsPerCountry.entrySet()) {
            long[] c = entry.getValue();
            medalTable.add(new MedalCountDTO(entry.getKey(), c[0], c[1], c[2]));
        }
        medalTable.sort(Comparator
                .comparingLong(MedalCountDTO::getGold).reversed()
                .thenComparing(Comparator.comparingLong(MedalCountDTO::getSilver).reversed())
                .thenComparing(Comparator.comparingLong(MedalCountDTO::getBronze).reversed()));

        OlympicsStatsDTO stats = new OlympicsStatsDTO();
        stats.setAverageAge(averageAge);
        stats.setTotalAthletes(athletes.size());
        stats.setTotalCompetitions(competitions.size());
        stats.setYoungestMedalist(youngest == null ? null : mapperUtil.map(youngest, AthleteDTO.class));
        stats.setOldestMedalist(oldest == null ? null : mapperUtil.map(oldest, AthleteDTO.class));
        stats.setMedalsByCountry(medalTable);
        return stats;
    }
}
