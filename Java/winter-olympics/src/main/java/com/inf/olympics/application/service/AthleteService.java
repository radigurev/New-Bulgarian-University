package com.inf.olympics.application.service;

import com.inf.olympics.api.exception.AthleteNotFoundException;
import com.inf.olympics.application.dto.AthleteDto;
import com.inf.olympics.application.dto.CreateAthleteRequest;
import com.inf.olympics.application.mapper.AthleteMapper;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.repository.AthleteRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
@RequiredArgsConstructor
public class AthleteService {

    private final AthleteRepository athleteRepository;
    private final AthleteMapper athleteMapper;

    @Transactional(readOnly = true)
    public List<AthleteDto> findAll() {
        return athleteMapper.toDtoList(athleteRepository.findAll());
    }

    @Transactional(readOnly = true)
    public AthleteDto findById(Long id) {
        return athleteMapper.toDto(requireById(id));
    }

    @Transactional(readOnly = true)
    public AthleteEntity requireById(Long id) {
        return athleteRepository.findById(id).orElseThrow(() -> new AthleteNotFoundException(id));
    }

    @Transactional
    public AthleteDto create(CreateAthleteRequest request) {
        AthleteEntity entity = athleteMapper.toEntity(request);
        return athleteMapper.toDto(athleteRepository.save(entity));
    }

    @Transactional
    public AthleteDto update(Long id, CreateAthleteRequest request) {
        AthleteEntity entity = requireById(id);
        athleteMapper.updateEntity(request, entity);
        return athleteMapper.toDto(athleteRepository.save(entity));
    }

    @Transactional
    public void delete(Long id) {
        athleteRepository.delete(requireById(id));
    }

    @Transactional(readOnly = true)
    public List<AthleteDto> searchByCountry(String country) {
        return athleteMapper.toDtoList(athleteRepository.findAllByCountryIgnoreCase(country));
    }

    @Transactional(readOnly = true)
    public List<AthleteDto> searchByLastName(String lastName) {
        return athleteMapper.toDtoList(athleteRepository.findAllByLastNameContainingIgnoreCase(lastName));
    }
}
