package com.inf.olympics.application.service;

import com.inf.olympics.api.exception.RegistrationException;
import com.inf.olympics.application.dto.RegisterRequest;
import com.inf.olympics.application.mapper.AthleteMapper;
import com.inf.olympics.infrastructure.persistence.entity.AthleteEntity;
import com.inf.olympics.infrastructure.persistence.entity.UserAccountEntity;
import com.inf.olympics.infrastructure.persistence.entity.UserRole;
import com.inf.olympics.infrastructure.persistence.repository.AthleteRepository;
import com.inf.olympics.infrastructure.persistence.repository.UserAccountRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.EnumSet;

@Service
@RequiredArgsConstructor
public class UserAccountService {

    private final UserAccountRepository userAccountRepository;
    private final AthleteRepository athleteRepository;
    private final AthleteMapper athleteMapper;
    private final PasswordEncoder passwordEncoder;

    @Transactional
    public UserAccountEntity registerAthlete(RegisterRequest request) {
        if (userAccountRepository.existsByUsername(request.username())) {
            throw new RegistrationException("Username is already taken");
        }
        AthleteEntity athlete = athleteMapper.toEntity(request.athlete());
        athlete = athleteRepository.save(athlete);

        UserAccountEntity user = new UserAccountEntity();
        user.setUsername(request.username());
        user.setPasswordHash(passwordEncoder.encode(request.password()));
        user.setEnabled(true);
        user.setRoles(EnumSet.of(UserRole.ROLE_ATHLETE));
        user.setAthlete(athlete);
        return userAccountRepository.save(user);
    }

    @Transactional(readOnly = true)
    public UserAccountEntity findByUsername(String username) {
        return userAccountRepository.findByUsername(username)
                .orElseThrow(() -> new RegistrationException("User not found"));
    }
}
