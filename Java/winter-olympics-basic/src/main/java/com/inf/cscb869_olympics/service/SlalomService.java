package com.inf.cscb869_olympics.service;

import com.inf.cscb869_olympics.dto.RankingEntryDTO;
import com.inf.cscb869_olympics.dto.SlalomRunDTO;

import java.util.List;

public interface SlalomService {

    void recordRunOne(Long competitionId, SlalomRunDTO runDto);

    void recordRunTwo(Long competitionId, SlalomRunDTO runDto);

    List<RankingEntryDTO> getRunTwoStartList(Long competitionId);

    List<RankingEntryDTO> getRanking(Long competitionId);
}
