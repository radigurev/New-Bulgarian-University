package com.inf.olympics.application.dto;

import com.inf.olympics.domain.model.Medal;

import java.math.BigDecimal;

public record RankingEntryDto(
        int position,
        Long athleteId,
        String firstName,
        String lastName,
        String country,
        BigDecimal totalTimeSeconds,
        Medal medal
) {
}
