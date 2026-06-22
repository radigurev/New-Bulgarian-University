package com.inf.olympics.application.dto;

import java.util.List;

public record OlympicsStatsDto(
        double averageAge,
        long totalAthletes,
        long totalCompetitions,
        AthleteDto youngestMedalist,
        AthleteDto oldestMedalist,
        List<MedalCountDto> medalsByCountry
) {
}
