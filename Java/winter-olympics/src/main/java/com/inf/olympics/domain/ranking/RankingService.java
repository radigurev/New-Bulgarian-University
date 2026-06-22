package com.inf.olympics.domain.ranking;

import com.inf.olympics.application.dto.RankingEntryDto;
import com.inf.olympics.domain.model.CompetitionType;
import com.inf.olympics.infrastructure.persistence.entity.CompetitionEntity;
import com.inf.olympics.infrastructure.persistence.repository.CompetitionRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.EnumMap;
import java.util.List;
import java.util.Map;

@Service
@RequiredArgsConstructor
public class RankingService {

    private final List<RankingStrategy> strategies;
    private final CompetitionRepository competitionRepository;

    private Map<CompetitionType, RankingStrategy> indexed;

    @Transactional(readOnly = true)
    public List<RankingEntryDto> rank(Long competitionId) {
        CompetitionEntity competition = competitionRepository.findById(competitionId)
                .orElseThrow(() -> new com.inf.olympics.api.exception.CompetitionNotFoundException(competitionId));
        return resolve(competition.getType()).rank(competitionId);
    }

    private RankingStrategy resolve(CompetitionType type) {
        if (indexed == null) {
            indexed = new EnumMap<>(CompetitionType.class);
            strategies.forEach(s -> indexed.put(s.supports(), s));
        }
        RankingStrategy strategy = indexed.get(type);
        if (strategy == null) {
            throw new IllegalStateException("No ranking strategy for " + type);
        }
        return strategy;
    }
}
