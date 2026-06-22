package com.inf.olympics.application.service;

import com.inf.olympics.api.exception.RegistrationException;
import com.inf.olympics.application.dto.RankingEntryDto;
import com.inf.olympics.application.dto.SlalomRunRequest;
import com.inf.olympics.domain.model.CompetitionType;
import com.inf.olympics.domain.model.ResultStatus;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.entity.CompetitionEntity;
import com.inf.olympics.infrastructure.persistence.entity.SlalomResultEntity;
import com.inf.olympics.infrastructure.persistence.repository.SlalomResultRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

@Service
@RequiredArgsConstructor
public class SlalomService {

    private final SlalomResultRepository slalomResultRepository;
    private final CompetitionService competitionService;
    private final AthleteService athleteService;

    @Value("${app.slalom.qualifiers-for-run-two:30}")
    private int qualifiersForRunTwo;

    @Transactional
    public void recordRunOne(Long competitionId, SlalomRunRequest request) {
        CompetitionEntity competition = requireSlalom(competitionId);
        AthleteEntity athlete = athleteService.requireById(request.athleteId());

        SlalomResultEntity result = slalomResultRepository
                .findByCompetitionIdAndAthleteId(competitionId, athlete.getId())
                .orElseGet(SlalomResultEntity::new);
        result.setCompetition(competition);
        result.setAthlete(athlete);
        result.setRunOneStatus(request.status());
        result.setRunOneTimeSeconds(request.status() == ResultStatus.FINISHED ? request.timeSeconds() : null);
        slalomResultRepository.save(result);
    }

    @Transactional
    public void recordRunTwo(Long competitionId, SlalomRunRequest request) {
        requireSlalom(competitionId);
        SlalomResultEntity result = slalomResultRepository
                .findByCompetitionIdAndAthleteId(competitionId, request.athleteId())
                .orElseThrow(() -> new RegistrationException("Athlete has no run-one result for this competition"));

        if (result.getRunOneStatus() != ResultStatus.FINISHED || result.getRunOneTimeSeconds() == null) {
            throw new RegistrationException("Athlete did not qualify for run two");
        }
        result.setRunTwoStatus(request.status());
        result.setRunTwoTimeSeconds(request.status() == ResultStatus.FINISHED ? request.timeSeconds() : null);
        slalomResultRepository.save(result);
    }

    @Transactional(readOnly = true)
    public List<RankingEntryDto> getRunTwoStartList(Long competitionId) {
        requireSlalom(competitionId);
        List<SlalomResultEntity> qualifiers = slalomResultRepository.findAllByCompetitionId(competitionId).stream()
                .filter(r -> r.getRunOneStatus() == ResultStatus.FINISHED && r.getRunOneTimeSeconds() != null)
                .sorted(Comparator.comparing(SlalomResultEntity::getRunOneTimeSeconds))
                .limit(qualifiersForRunTwo)
                .toList();

        List<SlalomResultEntity> reversed = new ArrayList<>(qualifiers);
        Collections.reverse(reversed);

        List<RankingEntryDto> startList = new ArrayList<>();
        int position = 1;
        for (SlalomResultEntity r : reversed) {
            AthleteEntity a = r.getAthlete();
            startList.add(new RankingEntryDto(position++, a.getId(),
                    a.getFirstName(), a.getLastName(), a.getCountry(),
                    r.getRunOneTimeSeconds(), null));
        }
        return startList;
    }

    private CompetitionEntity requireSlalom(Long id) {
        CompetitionEntity competition = competitionService.requireById(id);
        if (competition.getType() != CompetitionType.SKI_SLALOM) {
            throw new RegistrationException("Competition is not a Ski Slalom event");
        }
        return competition;
    }
}
