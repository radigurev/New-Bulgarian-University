package com.inf.olympics.application.service;

import com.inf.olympics.api.exception.CompetitionNotFoundException;
import com.inf.olympics.api.exception.RegistrationException;
import com.inf.olympics.application.dto.CompetitionDto;
import com.inf.olympics.application.dto.CreateCompetitionRequest;
import com.inf.olympics.application.mapper.CompetitionMapper;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.entity.CompetitionEntity;
import com.inf.olympics.infrastructure.persistence.entity.RegistrationEntity;
import com.inf.olympics.infrastructure.persistence.repository.CompetitionRepository;
import com.inf.olympics.infrastructure.persistence.repository.RegistrationRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.Period;
import java.util.List;

@Service
@RequiredArgsConstructor
public class CompetitionService {

    private final CompetitionRepository competitionRepository;
    private final RegistrationRepository registrationRepository;
    private final AthleteService athleteService;
    private final CompetitionMapper competitionMapper;

    @Transactional(readOnly = true)
    public List<CompetitionDto> findAll() {
        return competitionMapper.toDtoList(competitionRepository.findAll());
    }

    @Transactional(readOnly = true)
    public CompetitionDto findById(Long id) {
        return competitionMapper.toDto(requireById(id));
    }

    @Transactional(readOnly = true)
    public CompetitionEntity requireById(Long id) {
        return competitionRepository.findById(id).orElseThrow(() -> new CompetitionNotFoundException(id));
    }

    @Transactional
    public CompetitionDto create(CreateCompetitionRequest request) {
        CompetitionEntity entity = competitionMapper.toEntity(request);
        return competitionMapper.toDto(competitionRepository.save(entity));
    }

    @Transactional
    public CompetitionDto update(Long id, CreateCompetitionRequest request) {
        CompetitionEntity entity = requireById(id);
        competitionMapper.updateEntity(request, entity);
        return competitionMapper.toDto(competitionRepository.save(entity));
    }

    @Transactional
    public void delete(Long id) {
        competitionRepository.delete(requireById(id));
    }

    @Transactional
    public CompetitionDto finish(Long id) {
        CompetitionEntity entity = requireById(id);
        entity.setFinished(true);
        return competitionMapper.toDto(competitionRepository.save(entity));
    }

    @Transactional
    public void register(Long competitionId, Long athleteId) {
        CompetitionEntity competition = requireById(competitionId);
        if (competition.isFinished()) {
            throw new RegistrationException("Competition is already finished");
        }
        AthleteEntity athlete = athleteService.requireById(athleteId);
        if (athlete.getGender() != competition.getGender()) {
            throw new RegistrationException("Athlete gender does not match the competition gender");
        }
        int age = Period.between(athlete.getDateOfBirth(), competition.getHeldOn()).getYears();
        if (age < competition.getMinAge()) {
            throw new RegistrationException("Athlete is younger than the minimum age of " + competition.getMinAge());
        }
        if (registrationRepository.existsByCompetitionIdAndAthleteId(competitionId, athleteId)) {
            throw new RegistrationException("Athlete is already registered for this competition");
        }
        RegistrationEntity registration = new RegistrationEntity();
        registration.setCompetition(competition);
        registration.setAthlete(athlete);
        registrationRepository.save(registration);
    }
}
