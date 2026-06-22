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
@Table(name = "slalom_results",
        uniqueConstraints = @UniqueConstraint(columnNames = {"competition_id", "athlete_id"}))
@Getter
@Setter
@ToString(exclude = {"competition", "athlete"})
public class SlalomResult extends BaseEntity {

    @ManyToOne(optional = false)
    @JoinColumn(name = "competition_id")
    private Competition competition;

    @ManyToOne(optional = false)
    @JoinColumn(name = "athlete_id")
    private Athlete athlete;

    @Column(precision = 8, scale = 3)
    private BigDecimal runOneTimeSeconds;

    @Column(precision = 8, scale = 3)
    private BigDecimal runTwoTimeSeconds;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false, length = 10)
    private ResultStatus runOneStatus = ResultStatus.DNS;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false, length = 10)
    private ResultStatus runTwoStatus = ResultStatus.DNS;

    public boolean qualifiedForRunTwo() {
        return runOneStatus == ResultStatus.FINISHED && runOneTimeSeconds != null;
    }

    public boolean isClassified() {
        return runOneStatus == ResultStatus.FINISHED
                && runTwoStatus == ResultStatus.FINISHED
                && runOneTimeSeconds != null
                && runTwoTimeSeconds != null;
    }

    public BigDecimal getTotalTimeSeconds() {
        if (!isClassified()) {
            return null;
        }
        return runOneTimeSeconds.add(runTwoTimeSeconds);
    }
}
