package com.inf.cscb869_olympics.dto;

import com.inf.cscb869_olympics.data.entity.Gender;
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
public class AthleteDTO {

    private Long id;
    private String firstName;
    private String lastName;
    private String country;
    private Gender gender;
    private LocalDate dateOfBirth;
}
