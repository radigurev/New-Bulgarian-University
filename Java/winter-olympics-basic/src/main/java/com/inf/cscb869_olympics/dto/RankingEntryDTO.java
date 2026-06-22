package com.inf.cscb869_olympics.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.ToString;

import java.math.BigDecimal;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@ToString
public class RankingEntryDTO {

    private int position;
    private Long athleteId;
    private String firstName;
    private String lastName;
    private String country;
    private BigDecimal totalTimeSeconds;
    private String medal;
}
