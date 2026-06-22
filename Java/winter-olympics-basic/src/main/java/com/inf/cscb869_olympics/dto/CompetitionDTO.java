package com.inf.cscb869_olympics.dto;

import com.inf.cscb869_olympics.data.entity.CompetitionType;
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
public class CompetitionDTO {

    private Long id;
    private String name;
    private CompetitionType type;
    private Gender gender;
    private int minAge;
    private LocalDate heldOn;
    private boolean finished;
}
