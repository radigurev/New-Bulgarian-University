package com.inf.olympics.application.dto;

import com.inf.olympics.domain.model.CompetitionType;
import com.inf.olympics.domain.model.Gender;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import java.time.LocalDate;

public record CreateCompetitionRequest(
        @NotBlank @Size(min = 3, max = 120) String name,
        @NotNull CompetitionType type,
        @NotNull Gender gender,
        @Min(15) int minAge,
        @NotNull LocalDate heldOn
) {
}
