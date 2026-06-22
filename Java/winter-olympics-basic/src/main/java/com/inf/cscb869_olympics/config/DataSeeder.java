package com.inf.cscb869_olympics.config;

import com.inf.cscb869_olympics.data.entity.Role;
import com.inf.cscb869_olympics.data.entity.User;
import com.inf.cscb869_olympics.data.repo.RoleRepository;
import com.inf.cscb869_olympics.data.repo.UserRepository;
import jakarta.annotation.PostConstruct;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import java.util.Set;

@Component
@RequiredArgsConstructor
@Slf4j
public class DataSeeder {

    private final RoleRepository roleRepository;
    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;

    @PostConstruct
    @Transactional
    public void seed() {
        Role adminRole = roleRepository.findByAuthority("admin").orElseGet(() -> {
            Role r = new Role();
            r.setAuthority("admin");
            return roleRepository.save(r);
        });

        roleRepository.findByAuthority("athlete").orElseGet(() -> {
            Role r = new Role();
            r.setAuthority("athlete");
            return roleRepository.save(r);
        });

        if (!userRepository.existsByUsername("admin")) {
            User admin = new User();
            admin.setUsername("admin");
            admin.setPassword(passwordEncoder.encode("admin123"));
            admin.setAuthorities(Set.of(adminRole));
            userRepository.save(admin);
            log.info("Seeded default admin user: admin / admin123");
        }
    }
}
