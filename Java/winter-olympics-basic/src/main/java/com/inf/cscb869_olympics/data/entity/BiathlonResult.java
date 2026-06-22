package com.inf.cscb869_olympics.data.entity;

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
import lombok.ToString;

import java.math.BigDecimal;

@Entity
@Table(name = "biathlon_results",
        uniqueConstraints = @UniqueConstraint(columnNames = {"competition_id", "athlete_id"}))
@Getter
@Setter
@ToString(exclude = {"competition", "athlete"})
public class BiathlonResult extends BaseEntity {

    @ManyToOne(optional = false)
    @JoinColumn(name = "competition_id")
    private Competition competition;

    @ManyToOne(optional = false)
    @JoinColumn(name = "athlete_id")
    private Athlete athlete;

    @Column(precision = 8, scale = 3)
    private BigDecimal baseTimeSeconds;

    @Column(nullable = false)
    private int misses = 0;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false, length = 10)
    private ResultStatus status = ResultStatus.DNS;

    public boolean isClassified() {
        return status == ResultStatus.FINISHED && baseTimeSeconds != null;
    }

    public BigDecimal computePenaltySeconds(int penaltySecondsPerMiss) {
        return BigDecimal.valueOf((long) misses * penaltySecondsPerMiss);
    }

    public BigDecimal computeFinalTimeSeconds(int penaltySecondsPerMiss) {
        if (!isClassified()) {
            return null;
        }
        return baseTimeSeconds.add(computePenaltySeconds(penaltySecondsPerMiss));
    }
}
