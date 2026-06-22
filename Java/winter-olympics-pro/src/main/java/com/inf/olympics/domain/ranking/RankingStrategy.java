package com.inf.olympics.domain.ranking;

import com.inf.olympics.application.dto.RankingEntryDto;
import com.inf.olympics.domain.model.CompetitionType;

import java.util.List;

public interface RankingStrategy {

    CompetitionType supports();

    List<RankingEntryDto> rank(Long competitionId);
}
