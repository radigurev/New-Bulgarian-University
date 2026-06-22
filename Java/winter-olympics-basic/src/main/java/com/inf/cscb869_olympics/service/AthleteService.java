package com.inf.cscb869_olympics.service;

import com.inf.cscb869_olympics.data.entity.Athlete;
import com.inf.cscb869_olympics.dto.AthleteDTO;
import com.inf.cscb869_olympics.dto.CreateAthleteDTO;

import java.util.List;

public interface AthleteService {

    List<AthleteDTO> getAthletes();

    AthleteDTO getAthlete(Long id);

    Athlete getAthleteEntity(Long id);

    AthleteDTO createAthlete(CreateAthleteDTO dto);

    AthleteDTO updateAthlete(Long id, CreateAthleteDTO dto);

    void deleteAthlete(Long id);

    List<AthleteDTO> searchByCountry(String country);

    List<AthleteDTO> searchByLastName(String lastName);
}
