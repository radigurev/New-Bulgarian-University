package com.inf.olympics.api.controller;

import com.inf.olympics.application.dto.LoginRequest;
import com.inf.olympics.application.dto.RegisterRequest;
import com.inf.olympics.application.dto.TokenResponse;
import com.inf.olympics.application.service.UserAccountService;
import com.inf.olympics.infrastructure.security.JwtService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.security.SecurityRequirements;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.Map;

@RestController
@RequestMapping("/api/v1/auth")
@RequiredArgsConstructor
@Tag(name = "Authentication", description = "Sign-in and self-service registration")
@SecurityRequirements({})
public class AuthController {

    private final UserAccountService userAccountService;
    private final AuthenticationManager authenticationManager;
    private final JwtService jwtService;

    @PostMapping("/register")
    @Operation(summary = "Self-register as an athlete and receive a 201")
    public ResponseEntity<Map<String, Object>> register(@Valid @RequestBody RegisterRequest request) {
        var user = userAccountService.registerAthlete(request);
        return ResponseEntity.status(HttpStatus.CREATED).body(Map.of(
                "id", user.getId(),
                "username", user.getUsername(),
                "athleteId", user.getAthlete().getId()
        ));
    }

    @PostMapping("/login")
    @Operation(summary = "Exchange username/password for a JWT access token")
    public TokenResponse login(@Valid @RequestBody LoginRequest request) {
        authenticationManager.authenticate(
                new UsernamePasswordAuthenticationToken(request.username(), request.password()));
        var user = userAccountService.findByUsername(request.username());
        var issued = jwtService.issue(user);
        return new TokenResponse(issued.value(), "Bearer", issued.expiresAt(), issued.username(), issued.roles());
    }
}
