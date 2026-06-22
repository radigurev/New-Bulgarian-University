package com.inf.olympics.infrastructure.persistence.entity;

import jakarta.persistence.Entity;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;
import jakarta.persistence.UniqueConstraint;
import lombok.Getter;
import lombok.Setter;

@Entity
@Table(name = "registrations",
        uniqueConstraints = @UniqueConstraint(name = "uk_registration_comp_athlete",
                columnNames = {"competition_id", "athlete_id"}))
@Getter
@Setter
public class RegistrationEntity extends BaseEntity {

    @ManyToOne(optional = false)
    @JoinColumn(name = "competition_id", nullable = false)
    private CompetitionEntity competition;

    @ManyToOne(optional = false)
    @JoinColumn(name = "athlete_id", nullable = false)
    private AthleteEntity athlete;
}
