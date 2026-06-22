package com.inf.cscb869_olympics.service.impl;

import com.inf.cscb869_olympics.data.entity.Athlete;
import com.inf.cscb869_olympics.data.entity.Competition;
import com.inf.cscb869_olympics.data.entity.CompetitionRegistration;
import com.inf.cscb869_olympics.data.repo.CompetitionRegistrationRepository;
import com.inf.cscb869_olympics.data.repo.CompetitionRepository;
import com.inf.cscb869_olympics.dto.CompetitionDTO;
import com.inf.cscb869_olympics.dto.CreateCompetitionDTO;
import com.inf.cscb869_olympics.exception.CompetitionNotFoundException;
import com.inf.cscb869_olympics.exception.RegistrationException;
import com.inf.cscb869_olympics.service.AthleteService;
import com.inf.cscb869_olympics.service.CompetitionService;
import com.inf.cscb869_olympics.util.MapperUtil;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.time.Period;
import java.util.List;

@Service
@RequiredArgsConstructor
public class CompetitionServiceImpl implements CompetitionService {

    private final CompetitionRepository competitionRepository;
    private final CompetitionRegistrationRepository registrationRepository;
    private final AthleteService athleteService;
    private final MapperUtil mapperUtil;

    @Override
    @Transactional(readOnly = true)
    public List<CompetitionDTO> getCompetitions() {
        return mapperUtil.mapList(competitionRepository.findAll(), CompetitionDTO.class);
    }

    @Override
    @Transactional(readOnly = true)
    public CompetitionDTO getCompetition(Long id) {
        return mapperUtil.map(getCompetitionEntity(id), CompetitionDTO.class);
    }

    @Override
    @Transactional(readOnly = true)
    public Competition getCompetitionEntity(Long id) {
        return competitionRepository.findById(id)
                .orElseThrow(() -> new CompetitionNotFoundException(id));
    }

    @Override
    @Transactional
    public CompetitionDTO createCompetition(CreateCompetitionDTO dto) {
        Competition competition = mapperUtil.map(dto, Competition.class);
        competition.setId(null);
        competition.setFinished(false);
        return mapperUtil.map(competitionRepository.save(competition), CompetitionDTO.class);
    }

    @Override
    @Transactional
    public CompetitionDTO updateCompetition(Long id, CreateCompetitionDTO dto) {
        Competition competition = getCompetitionEntity(id);
        competition.setName(dto.getName());
        competition.setType(dto.getType());
        competition.setGender(dto.getGender());
        competition.setMinAge(dto.getMinAge());
        competition.setHeldOn(dto.getHeldOn());
        return mapperUtil.map(competitionRepository.save(competition), CompetitionDTO.class);
    }

    @Override
    @Transactional
    public void deleteCompetition(Long id) {
        Competition competition = getCompetitionEntity(id);
        competitionRepository.delete(competition);
    }

    @Override
    @Transactional
    public CompetitionDTO finishCompetition(Long id) {
        Competition competition = getCompetitionEntity(id);
        competition.setFinished(true);
        return mapperUtil.map(competitionRepository.save(competition), CompetitionDTO.class);
    }

    @Override
    @Transactional
    public void registerAthlete(Long competitionId, Long athleteId) {
        Competition competition = getCompetitionEntity(competitionId);
        if (competition.isFinished()) {
            throw new RegistrationException("Competition is already finished");
        }
        Athlete athlete = athleteService.getAthleteEntity(athleteId);

        if (athlete.getGender() != competition.getGender()) {
            throw new RegistrationException("Athlete gender does not match competition gender");
        }

        int age = Period.between(athlete.getDateOfBirth(), competition.getHeldOn()).getYears();
        if (age < competition.getMinAge()) {
            throw new RegistrationException("Athlete is younger than the minimum age of " + competition.getMinAge());
        }

        if (registrationRepository.existsByCompetitionIdAndAthleteId(competitionId, athleteId)) {
            throw new RegistrationException("Athlete is already registered for this competition");
        }

        CompetitionRegistration registration = new CompetitionRegistration();
        registration.setCompetition(competition);
        registration.setAthlete(athlete);
        registrationRepository.save(registration);
    }

    private static int yearsBetween(LocalDate from, LocalDate to) {
        return Period.between(from, to).getYears();
    }
}
