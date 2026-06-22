package com.inf.cscb869_olympics.dto;

import com.inf.cscb869_olympics.data.entity.ResultStatus;
import jakarta.validation.constraints.DecimalMin;
import jakarta.validation.constraints.Digits;
import jakarta.validation.constraints.NotNull;
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
public class SlalomRunDTO {

    @NotNull
    private Long athleteId;

    @NotNull
    private ResultStatus status;

    @DecimalMin(value = "0.001", message = "Time must be greater than zero")
    @Digits(integer = 5, fraction = 3, message = "Time must have at most 3 decimal places")
    private BigDecimal timeSeconds;
}
