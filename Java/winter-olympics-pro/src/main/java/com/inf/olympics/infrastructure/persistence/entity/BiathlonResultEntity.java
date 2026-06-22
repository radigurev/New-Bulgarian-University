package com.inf.olympics.infrastructure.persistence.entity;

import com.inf.olympics.domain.model.ResultStatus;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;
import jakarta.persistence.UniqueConstraint;
import lombok.Getter;
import lombok.Setter;

import java.math.BigDecimal;

@Entity
@Table(name = "biathlon_results",
        uniqueConstraints = @UniqueConstraint(name = "uk_biathlon_comp_athlete",
                columnNames = {"competition_id", "athlete_id"}))
@Getter
@Setter
public class BiathlonResultEntity extends BaseEntity {

    @ManyToOne(optional = false)
    @JoinColumn(name = "competition_id", nullable = false)
    private CompetitionEntity competition;

    @ManyToOne(optional = false)
    @JoinColumn(name = "athlete_id", nullable = false)
    private AthleteEntity athlete;

    @Column(name = "base_time_seconds", precision = 8, scale = 3)
    private BigDecimal baseTimeSeconds;

    @Column(name = "misses", nullable = false)
    private int misses;

    @Enumerated(EnumType.STRING)
    @Column(name = "status", nullable = false, length = 10)
    private ResultStatus status = ResultStatus.DNS;
}
