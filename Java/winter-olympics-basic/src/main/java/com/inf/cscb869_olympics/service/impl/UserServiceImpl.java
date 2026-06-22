package com.inf.cscb869_olympics.service.impl;

import com.inf.cscb869_olympics.data.entity.Athlete;
import com.inf.cscb869_olympics.data.entity.Role;
import com.inf.cscb869_olympics.data.entity.User;
import com.inf.cscb869_olympics.data.repo.AthleteRepository;
import com.inf.cscb869_olympics.data.repo.RoleRepository;
import com.inf.cscb869_olympics.data.repo.UserRepository;
import com.inf.cscb869_olympics.dto.RegisterUserDTO;
import com.inf.cscb869_olympics.exception.RegistrationException;
import com.inf.cscb869_olympics.service.UserService;
import com.inf.cscb869_olympics.util.MapperUtil;
import lombok.RequiredArgsConstructor;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.Set;

@Service
@RequiredArgsConstructor
public class UserServiceImpl implements UserService {

    private final UserRepository userRepository;
    private final RoleRepository roleRepository;
    private final AthleteRepository athleteRepository;
    private final PasswordEncoder passwordEncoder;
    private final MapperUtil mapperUtil;

    @Override
    @Transactional(readOnly = true)
    public UserDetails loadUserByUsername(String username) throws UsernameNotFoundException {
        return findByUsername(username);
    }

    @Override
    @Transactional(readOnly = true)
    public User findByUsername(String username) {
        return userRepository.findByUsername(username)
                .orElseThrow(() -> new UsernameNotFoundException("User with username " + username + " not found"));
    }

    @Override
    @Transactional
    public User registerAthlete(RegisterUserDTO dto) {
        if (userRepository.existsByUsername(dto.getUsername())) {
            throw new RegistrationException("Username is already taken");
        }

        Role athleteRole = roleRepository.findByAuthority("athlete")
                .orElseThrow(() -> new IllegalStateException("Athlete role missing — seed data not initialized"));

        Athlete athlete = mapperUtil.map(dto.getAthlete(), Athlete.class);
        athlete.setId(null);
        athlete = athleteRepository.save(athlete);

        User user = new User();
        user.setUsername(dto.getUsername());
        user.setPassword(passwordEncoder.encode(dto.getPassword()));
        user.getAuthorities().add(athleteRole);
        user.setAthlete(athlete);
        return userRepository.save(user);
    }
}
