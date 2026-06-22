package com.inf.cscb869_olympics.service;

import com.inf.cscb869_olympics.dto.OlympicsStatsDTO;
import com.inf.cscb869_olympics.dto.RankingEntryDTO;

import java.util.List;

public interface OlympicsService {

    OlympicsStatsDTO getStats();

    List<RankingEntryDTO> getRanking(Long competitionId);
}
