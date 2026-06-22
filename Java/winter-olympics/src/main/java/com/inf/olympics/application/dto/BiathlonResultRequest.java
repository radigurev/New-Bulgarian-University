package com.inf.olympics.application.dto;

import com.inf.olympics.domain.model.ResultStatus;
import jakarta.validation.constraints.DecimalMin;
import jakarta.validation.constraints.Digits;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;

import java.math.BigDecimal;

public record BiathlonResultRequest(
        @NotNull Long athleteId,
        @NotNull ResultStatus status,
        @DecimalMin("0.001") @Digits(integer = 5, fraction = 3) BigDecimal baseTimeSeconds,
        @Min(0) int misses
) {
}
