package com.inf.cscb869_olympics.dto;

import com.inf.cscb869_olympics.data.entity.CompetitionType;
import com.inf.cscb869_olympics.data.entity.Gender;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.ToString;

import java.time.LocalDate;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@ToString
public class CreateCompetitionDTO {

    @NotBlank
    @Size(min = 3, max = 120, message = "Name must be between 3 and 120 characters")
    private String name;

    @NotNull
    private CompetitionType type;

    @NotNull
    private Gender gender;

    @Min(value = 15, message = "Minimum age must be at least 15")
    private int minAge;

    @NotNull
    private LocalDate heldOn;
}
