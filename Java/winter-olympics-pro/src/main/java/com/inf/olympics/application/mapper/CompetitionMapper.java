package com.inf.olympics.application.mapper;

import com.inf.olympics.application.dto.CompetitionDto;
import com.inf.olympics.application.dto.CreateCompetitionRequest;
import com.inf.olympics.infrastructure.persistence.entity.CompetitionEntity;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;

import java.util.List;

@Mapper
public interface CompetitionMapper {

    CompetitionDto toDto(CompetitionEntity entity);

    List<CompetitionDto> toDtoList(List<CompetitionEntity> entities);

    @Mapping(target = "finished", constant = "false")
    CompetitionEntity toEntity(CreateCompetitionRequest request);

    @Mapping(target = "finished", ignore = true)
    void updateEntity(CreateCompetitionRequest request, @MappingTarget CompetitionEntity entity);
}
