package com.inf.olympics.application.service;

import com.inf.olympics.api.exception.RegistrationException;
import com.inf.olympics.application.dto.BiathlonResultRequest;
import com.inf.olympics.domain.model.CompetitionType;
import com.inf.olympics.domain.model.ResultStatus;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.entity.BiathlonResultEntity;
import com.inf.olympics.infrastructure.persistence.entity.CompetitionEntity;
import com.inf.olympics.infrastructure.persistence.repository.BiathlonResultRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
@RequiredArgsConstructor
public class BiathlonService {

    private final BiathlonResultRepository biathlonResultRepository;
    private final CompetitionService competitionService;
    private final AthleteService athleteService;

    @Transactional
    public void recordResult(Long competitionId, BiathlonResultRequest request) {
        CompetitionEntity competition = competitionService.requireById(competitionId);
        if (competition.getType() != CompetitionType.BIATHLON) {
            throw new RegistrationException("Competition is not a Biathlon event");
        }
        AthleteEntity athlete = athleteService.requireById(request.athleteId());

        BiathlonResultEntity result = biathlonResultRepository
                .findByCompetitionIdAndAthleteId(competitionId, athlete.getId())
                .orElseGet(BiathlonResultEntity::new);
        result.setCompetition(competition);
        result.setAthlete(athlete);
        result.setStatus(request.status());
        result.setBaseTimeSeconds(request.status() == ResultStatus.FINISHED ? request.baseTimeSeconds() : null);
        result.setMisses(request.misses());
        biathlonResultRepository.save(result);
    }
}
