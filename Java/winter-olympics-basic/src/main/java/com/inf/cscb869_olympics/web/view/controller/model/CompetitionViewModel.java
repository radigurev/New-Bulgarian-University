package com.inf.cscb869_olympics.web.view.controller.model;

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
import org.springframework.format.annotation.DateTimeFormat;

import java.time.LocalDate;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@ToString
public class CompetitionViewModel {

    private Long id;

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
    @DateTimeFormat(iso = DateTimeFormat.ISO.DATE)
    private LocalDate heldOn;

    private boolean finished;
}
