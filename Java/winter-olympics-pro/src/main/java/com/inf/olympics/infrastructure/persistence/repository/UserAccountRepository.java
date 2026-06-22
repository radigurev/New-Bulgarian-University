package com.inf.olympics.infrastructure.persistence.repository;

import com.inf.olympics.infrastructure.persistence.entity.UserAccountEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface UserAccountRepository extends JpaRepository<UserAccountEntity, Long> {

    Optional<UserAccountEntity> findByUsername(String username);

    boolean existsByUsername(String username);
}
