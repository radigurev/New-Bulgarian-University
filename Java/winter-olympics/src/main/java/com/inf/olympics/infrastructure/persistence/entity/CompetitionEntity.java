package com.inf.olympics.infrastructure.persistence.entity;

import com.inf.olympics.domain.model.CompetitionType;
import com.inf.olympics.domain.model.Gender;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.Table;
import lombok.Getter;
import lombok.Setter;

import java.time.LocalDate;

@Entity
@Table(name = "competitions")
@Getter
@Setter
public class CompetitionEntity extends BaseEntity {

    @Column(name = "name", nullable = false, length = 120)
    private String name;

    @Enumerated(EnumType.STRING)
    @Column(name = "type", nullable = false, length = 20)
    private CompetitionType type;

    @Enumerated(EnumType.STRING)
    @Column(name = "gender", nullable = false, length = 10)
    private Gender gender;

    @Column(name = "min_age", nullable = false)
    private int minAge;

    @Column(name = "held_on", nullable = false)
    private LocalDate heldOn;

    @Column(name = "finished", nullable = false)
    private boolean finished = false;
}
