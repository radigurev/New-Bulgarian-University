package com.inf.cscb869_olympics.service.impl;

import com.inf.cscb869_olympics.data.entity.Athlete;
import com.inf.cscb869_olympics.data.entity.BiathlonResult;
import com.inf.cscb869_olympics.data.entity.Competition;
import com.inf.cscb869_olympics.data.entity.CompetitionType;
import com.inf.cscb869_olympics.data.entity.ResultStatus;
import com.inf.cscb869_olympics.data.repo.BiathlonResultRepository;
import com.inf.cscb869_olympics.dto.BiathlonResultDTO;
import com.inf.cscb869_olympics.dto.RankingEntryDTO;
import com.inf.cscb869_olympics.exception.RegistrationException;
import com.inf.cscb869_olympics.service.AthleteService;
import com.inf.cscb869_olympics.service.BiathlonService;
import com.inf.cscb869_olympics.service.CompetitionService;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

@Service
@RequiredArgsConstructor
public class BiathlonServiceImpl implements BiathlonService {

    private final BiathlonResultRepository biathlonResultRepository;
    private final CompetitionService competitionService;
    private final AthleteService athleteService;

    @Value("${app.biathlon.penalty-seconds-per-miss:60}")
    private int penaltySecondsPerMiss;

    @Override
    @Transactional
    public void recordResult(Long competitionId, BiathlonResultDTO dto) {
        Competition competition = requireBiathlonCompetition(competitionId);
        Athlete athlete = athleteService.getAthleteEntity(dto.getAthleteId());

        BiathlonResult result = biathlonResultRepository
                .findByCompetitionIdAndAthleteId(competitionId, athlete.getId())
                .orElseGet(BiathlonResult::new);
        result.setCompetition(competition);
        result.setAthlete(athlete);
        result.setStatus(dto.getStatus());
        result.setBaseTimeSeconds(dto.getStatus() == ResultStatus.FINISHED ? dto.getBaseTimeSeconds() : null);
        result.setMisses(dto.getMisses());
        biathlonResultRepository.save(result);
    }

    @Override
    @Transactional(readOnly = true)
    public List<RankingEntryDTO> getRanking(Long competitionId) {
        requireBiathlonCompetition(competitionId);

        List<BiathlonResult> classified = biathlonResultRepository.findAllByCompetitionId(competitionId).stream()
                .filter(BiathlonResult::isClassified)
                .sorted(Comparator.comparing(r -> r.computeFinalTimeSeconds(penaltySecondsPerMiss)))
                .toList();

        List<RankingEntryDTO> ranking = new ArrayList<>();
        int position = 1;
        for (BiathlonResult r : classified) {
            ranking.add(new RankingEntryDTO(
                    position,
                    r.getAthlete().getId(),
                    r.getAthlete().getFirstName(),
                    r.getAthlete().getLastName(),
                    r.getAthlete().getCountry(),
                    r.computeFinalTimeSeconds(penaltySecondsPerMiss),
                    SlalomServiceImpl.medalFor(position)
            ));
            position++;
        }
        return ranking;
    }

    private Competition requireBiathlonCompetition(Long id) {
        Competition competition = competitionService.getCompetitionEntity(id);
        if (competition.getType() != CompetitionType.BIATHLON) {
            throw new RegistrationException("Competition is not a Biathlon event");
        }
        return competition;
    }
}
