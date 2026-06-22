package com.inf.olympics.application.dto;

import com.inf.olympics.domain.model.CompetitionType;
import com.inf.olympics.domain.model.Gender;

import java.time.LocalDate;

public record CompetitionDto(
        Long id,
        String name,
        CompetitionType type,
        Gender gender,
        int minAge,
        LocalDate heldOn,
        boolean finished
) {
}
