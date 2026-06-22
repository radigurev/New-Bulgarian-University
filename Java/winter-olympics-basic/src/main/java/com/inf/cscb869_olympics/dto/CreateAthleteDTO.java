package com.inf.cscb869_olympics.dto;

import com.inf.cscb869_olympics.data.entity.Gender;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Past;
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
public class CreateAthleteDTO {

    @NotBlank
    @Size(min = 2, max = 100, message = "First name must be between 2 and 100 characters")
    private String firstName;

    @NotBlank
    @Size(min = 2, max = 100, message = "Last name must be between 2 and 100 characters")
    private String lastName;

    @NotBlank
    @Size(min = 2, max = 60, message = "Country must be between 2 and 60 characters")
    private String country;

    @NotNull
    private Gender gender;

    @NotNull
    @Past(message = "Date of birth must be in the past")
    private LocalDate dateOfBirth;
}
