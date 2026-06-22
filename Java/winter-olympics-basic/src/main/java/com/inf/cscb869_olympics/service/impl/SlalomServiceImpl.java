package com.inf.cscb869_olympics.service.impl;

import com.inf.cscb869_olympics.data.entity.Athlete;
import com.inf.cscb869_olympics.data.entity.Competition;
import com.inf.cscb869_olympics.data.entity.CompetitionType;
import com.inf.cscb869_olympics.data.entity.ResultStatus;
import com.inf.cscb869_olympics.data.entity.SlalomResult;
import com.inf.cscb869_olympics.data.repo.SlalomResultRepository;
import com.inf.cscb869_olympics.dto.RankingEntryDTO;
import com.inf.cscb869_olympics.dto.SlalomRunDTO;
import com.inf.cscb869_olympics.exception.RegistrationException;
import com.inf.cscb869_olympics.service.AthleteService;
import com.inf.cscb869_olympics.service.CompetitionService;
import com.inf.cscb869_olympics.service.SlalomService;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

@Service
@RequiredArgsConstructor
public class SlalomServiceImpl implements SlalomService {

    private final SlalomResultRepository slalomResultRepository;
    private final CompetitionService competitionService;
    private final AthleteService athleteService;

    @Value("${app.slalom.qualifiers-for-run-two:30}")
    private int qualifiersForRunTwo;

    @Override
    @Transactional
    public void recordRunOne(Long competitionId, SlalomRunDTO dto) {
        Competition competition = requireSlalomCompetition(competitionId);
        Athlete athlete = athleteService.getAthleteEntity(dto.getAthleteId());

        SlalomResult result = slalomResultRepository
                .findByCompetitionIdAndAthleteId(competitionId, athlete.getId())
                .orElseGet(SlalomResult::new);
        result.setCompetition(competition);
        result.setAthlete(athlete);
        result.setRunOneStatus(dto.getStatus());
        result.setRunOneTimeSeconds(dto.getStatus() == ResultStatus.FINISHED ? dto.getTimeSeconds() : null);
        slalomResultRepository.save(result);
    }

    @Override
    @Transactional
    public void recordRunTwo(Long competitionId, SlalomRunDTO dto) {
        requireSlalomCompetition(competitionId);
        SlalomResult result = slalomResultRepository
                .findByCompetitionIdAndAthleteId(competitionId, dto.getAthleteId())
                .orElseThrow(() -> new RegistrationException("Athlete has no run-one result for this competition"));

        if (!result.qualifiedForRunTwo()) {
            throw new RegistrationException("Athlete did not qualify for run two");
        }

        result.setRunTwoStatus(dto.getStatus());
        result.setRunTwoTimeSeconds(dto.getStatus() == ResultStatus.FINISHED ? dto.getTimeSeconds() : null);
        slalomResultRepository.save(result);
    }

    @Override
    @Transactional(readOnly = true)
    public List<RankingEntryDTO> getRunTwoStartList(Long competitionId) {
        requireSlalomCompetition(competitionId);
        List<SlalomResult> qualifiers = slalomResultRepository.findAllByCompetitionId(competitionId).stream()
                .filter(SlalomResult::qualifiedForRunTwo)
                .sorted(Comparator.comparing(SlalomResult::getRunOneTimeSeconds))
                .limit(qualifiersForRunTwo)
                .toList();

        List<SlalomResult> reversed = new ArrayList<>(qualifiers);
        java.util.Collections.reverse(reversed);

        List<RankingEntryDTO> startList = new ArrayList<>();
        int position = 1;
        for (SlalomResult r : reversed) {
            startList.add(buildEntry(position++, r.getAthlete(), r.getRunOneTimeSeconds(), null));
        }
        return startList;
    }

    @Override
    @Transactional(readOnly = true)
    public List<RankingEntryDTO> getRanking(Long competitionId) {
        requireSlalomCompetition(competitionId);

        List<SlalomResult> classified = slalomResultRepository.findAllByCompetitionId(competitionId).stream()
                .filter(SlalomResult::isClassified)
                .sorted(Comparator.comparing(SlalomResult::getTotalTimeSeconds))
                .toList();

        List<RankingEntryDTO> ranking = new ArrayList<>();
        int position = 1;
        for (SlalomResult r : classified) {
            ranking.add(buildEntry(position, r.getAthlete(), r.getTotalTimeSeconds(), medalFor(position)));
            position++;
        }
        return ranking;
    }

    private Competition requireSlalomCompetition(Long id) {
        Competition competition = competitionService.getCompetitionEntity(id);
        if (competition.getType() != CompetitionType.SKI_SLALOM) {
            throw new RegistrationException("Competition is not a Ski Slalom event");
        }
        return competition;
    }

    private RankingEntryDTO buildEntry(int position, Athlete athlete, BigDecimal time, String medal) {
        return new RankingEntryDTO(
                position,
                athlete.getId(),
                athlete.getFirstName(),
                athlete.getLastName(),
                athlete.getCountry(),
                time,
                medal
        );
    }

    static String medalFor(int position) {
        return switch (position) {
            case 1 -> "GOLD";
            case 2 -> "SILVER";
            case 3 -> "BRONZE";
            default -> null;
        };
    }
}
