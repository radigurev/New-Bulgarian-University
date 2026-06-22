package com.inf.cscb869_olympics.service;

import com.inf.cscb869_olympics.dto.BiathlonResultDTO;
import com.inf.cscb869_olympics.dto.RankingEntryDTO;

import java.util.List;

public interface BiathlonService {

    void recordResult(Long competitionId, BiathlonResultDTO dto);

    List<RankingEntryDTO> getRanking(Long competitionId);
}
