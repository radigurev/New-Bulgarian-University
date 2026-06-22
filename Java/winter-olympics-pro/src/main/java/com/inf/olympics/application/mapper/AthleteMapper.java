package com.inf.olympics.application.mapper;

import com.inf.olympics.application.dto.AthleteDto;
import com.inf.olympics.application.dto.CreateAthleteRequest;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import org.mapstruct.Mapper;
import org.mapstruct.MappingTarget;

import java.util.List;

@Mapper
public interface AthleteMapper {

    AthleteDto toDto(AthleteEntity entity);

    List<AthleteDto> toDtoList(List<AthleteEntity> entities);

    AthleteEntity toEntity(CreateAthleteRequest request);

    void updateEntity(CreateAthleteRequest request, @MappingTarget AthleteEntity entity);
}
