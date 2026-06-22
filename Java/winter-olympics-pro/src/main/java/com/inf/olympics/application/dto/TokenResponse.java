package com.inf.olympics.application.dto;

import java.time.Instant;
import java.util.List;

public record TokenResponse(
        String accessToken,
        String tokenType,
        Instant expiresAt,
        String username,
        List<String> roles
) {
}
