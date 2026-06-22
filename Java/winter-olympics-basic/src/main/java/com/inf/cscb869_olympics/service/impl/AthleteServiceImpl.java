package com.inf.cscb869_olympics.service.impl;

import com.inf.cscb869_olympics.data.entity.Athlete;
import com.inf.cscb869_olympics.data.repo.AthleteRepository;
import com.inf.cscb869_olympics.dto.AthleteDTO;
import com.inf.cscb869_olympics.dto.CreateAthleteDTO;
import com.inf.cscb869_olympics.exception.AthleteNotFoundException;
import com.inf.cscb869_olympics.service.AthleteService;
import com.inf.cscb869_olympics.util.MapperUtil;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
@RequiredArgsConstructor
public class AthleteServiceImpl implements AthleteService {

    private final AthleteRepository athleteRepository;
    private final MapperUtil mapperUtil;

    @Override
    @Transactional(readOnly = true)
    public List<AthleteDTO> getAthletes() {
        return mapperUtil.mapList(athleteRepository.findAll(), AthleteDTO.class);
    }

    @Override
    @Transactional(readOnly = true)
    public AthleteDTO getAthlete(Long id) {
        return mapperUtil.map(getAthleteEntity(id), AthleteDTO.class);
    }

    @Override
    @Transactional(readOnly = true)
    public Athlete getAthleteEntity(Long id) {
        return athleteRepository.findById(id)
                .orElseThrow(() -> new AthleteNotFoundException(id));
    }

    @Override
    @Transactional
    public AthleteDTO createAthlete(CreateAthleteDTO dto) {
        Athlete athlete = mapperUtil.map(dto, Athlete.class);
        athlete.setId(null);
        Athlete saved = athleteRepository.save(athlete);
        return mapperUtil.map(saved, AthleteDTO.class);
    }

    @Override
    @Transactional
    public AthleteDTO updateAthlete(Long id, CreateAthleteDTO dto) {
        Athlete athlete = getAthleteEntity(id);
        athlete.setFirstName(dto.getFirstName());
        athlete.setLastName(dto.getLastName());
        athlete.setCountry(dto.getCountry());
        athlete.setGender(dto.getGender());
        athlete.setDateOfBirth(dto.getDateOfBirth());
        return mapperUtil.map(athleteRepository.save(athlete), AthleteDTO.class);
    }

    @Override
    @Transactional
    public void deleteAthlete(Long id) {
        Athlete athlete = getAthleteEntity(id);
        athleteRepository.delete(athlete);
    }

    @Override
    @Transactional(readOnly = true)
    public List<AthleteDTO> searchByCountry(String country) {
        return mapperUtil.mapList(athleteRepository.findAllByCountryIgnoreCase(country), AthleteDTO.class);
    }

    @Override
    @Transactional(readOnly = true)
    public List<AthleteDTO> searchByLastName(String lastName) {
        return mapperUtil.mapList(athleteRepository.findAllByLastNameContainingIgnoreCase(lastName), AthleteDTO.class);
    }
}
