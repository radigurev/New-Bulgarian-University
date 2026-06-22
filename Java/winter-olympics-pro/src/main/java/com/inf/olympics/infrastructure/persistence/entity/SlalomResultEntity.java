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
@Table(name = "slalom_results",
        uniqueConstraints = @UniqueConstraint(name = "uk_slalom_comp_athlete",
                columnNames = {"competition_id", "athlete_id"}))
@Getter
@Setter
public class SlalomResultEntity extends BaseEntity {

    @ManyToOne(optional = false)
    @JoinColumn(name = "competition_id", nullable = false)
    private CompetitionEntity competition;

    @ManyToOne(optional = false)
    @JoinColumn(name = "athlete_id", nullable = false)
    private AthleteEntity athlete;

    @Column(name = "run_one_time_seconds", precision = 8, scale = 3)
    private BigDecimal runOneTimeSeconds;

    @Column(name = "run_two_time_seconds", precision = 8, scale = 3)
    private BigDecimal runTwoTimeSeconds;

    @Enumerated(EnumType.STRING)
    @Column(name = "run_one_status", nullable = false, length = 10)
    private ResultStatus runOneStatus = ResultStatus.DNS;

    @Enumerated(EnumType.STRING)
    @Column(name = "run_two_status", nullable = false, length = 10)
    private ResultStatus runTwoStatus = ResultStatus.DNS;
}
