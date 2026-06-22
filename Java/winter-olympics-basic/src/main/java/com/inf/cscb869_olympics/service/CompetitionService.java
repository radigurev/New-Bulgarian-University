package com.inf.cscb869_olympics.service;

import com.inf.cscb869_olympics.data.entity.Competition;
import com.inf.cscb869_olympics.dto.CompetitionDTO;
import com.inf.cscb869_olympics.dto.CreateCompetitionDTO;

import java.util.List;

public interface CompetitionService {

    List<CompetitionDTO> getCompetitions();

    CompetitionDTO getCompetition(Long id);

    Competition getCompetitionEntity(Long id);

    CompetitionDTO createCompetition(CreateCompetitionDTO dto);

    CompetitionDTO updateCompetition(Long id, CreateCompetitionDTO dto);

    void deleteCompetition(Long id);

    CompetitionDTO finishCompetition(Long id);

    void registerAthlete(Long competitionId, Long athleteId);
}
