package com.inf.olympics.infrastructure.security;

import org.springframework.boot.context.properties.ConfigurationProperties;

@ConfigurationProperties(prefix = "app.security.jwt")
public record JwtProperties(
        String issuer,
        long ttlMinutes,
        String secret
) {
}
