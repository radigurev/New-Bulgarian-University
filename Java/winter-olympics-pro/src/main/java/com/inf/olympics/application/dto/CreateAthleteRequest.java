package com.inf.olympics.application.dto;

import com.inf.olympics.domain.model.Gender;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Past;
import jakarta.validation.constraints.Size;

import java.time.LocalDate;

public record CreateAthleteRequest(
        @NotBlank @Size(min = 2, max = 100) String firstName,
        @NotBlank @Size(min = 2, max = 100) String lastName,
        @NotBlank @Size(min = 2, max = 60) String country,
        @NotNull Gender gender,
        @NotNull @Past LocalDate dateOfBirth
) {
}
