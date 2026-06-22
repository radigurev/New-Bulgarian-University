package com.inf.cscb869_olympics.data.entity;

import jakarta.persistence.Entity;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;
import jakarta.persistence.UniqueConstraint;
import lombok.Getter;
import lombok.Setter;
import lombok.ToString;

@Entity
@Table(name = "competition_registrations",
        uniqueConstraints = @UniqueConstraint(columnNames = {"competition_id", "athlete_id"}))
@Getter
@Setter
@ToString(exclude = {"competition", "athlete"})
public class CompetitionRegistration extends BaseEntity {

    @ManyToOne(optional = false)
    @JoinColumn(name = "competition_id")
    private Competition competition;

    @ManyToOne(optional = false)
    @JoinColumn(name = "athlete_id")
    private Athlete athlete;
}
