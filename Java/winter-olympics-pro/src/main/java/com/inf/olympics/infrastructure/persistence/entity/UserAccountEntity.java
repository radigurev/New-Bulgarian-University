package com.inf.olympics.infrastructure.persistence.entity;

import jakarta.persistence.CollectionTable;
import jakarta.persistence.Column;
import jakarta.persistence.ElementCollection;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.FetchType;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToOne;
import jakarta.persistence.Table;
import lombok.Getter;
import lombok.Setter;

import java.util.HashSet;
import java.util.Set;

@Entity
@Table(name = "user_accounts")
@Getter
@Setter
public class UserAccountEntity extends BaseEntity {

    @Column(name = "username", nullable = false, unique = true, length = 60)
    private String username;

    @Column(name = "password_hash", nullable = false, length = 100)
    private String passwordHash;

    @Column(name = "enabled", nullable = false)
    private boolean enabled = true;

    @ElementCollection(fetch = FetchType.EAGER, targetClass = UserRole.class)
    @CollectionTable(name = "user_account_roles",
            joinColumns = @JoinColumn(name = "user_account_id"))
    @Column(name = "role", nullable = false, length = 30)
    @Enumerated(EnumType.STRING)
    private Set<UserRole> roles = new HashSet<>();

    @OneToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "athlete_id")
    private AthleteEntity athlete;
}
