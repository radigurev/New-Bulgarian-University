package com.inf.olympics.application.dto;

import com.inf.olympics.domain.model.Gender;

import java.time.LocalDate;

public record AthleteDto(
        Long id,
        String firstName,
        String lastName,
        String country,
        Gender gender,
        LocalDate dateOfBirth
) {
}
