package com.inf.cscb869_olympics.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.ToString;

import java.util.List;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@ToString
public class OlympicsStatsDTO {

    private double averageAge;
    private long totalAthletes;
    private long totalCompetitions;
    private AthleteDTO youngestMedalist;
    private AthleteDTO oldestMedalist;
    private List<MedalCountDTO> medalsByCountry;
}
