package com.inf.olympics.api.exception;

import jakarta.servlet.http.HttpServletRequest;
import org.springframework.http.HttpStatus;
import org.springframework.http.ProblemDetail;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.security.core.AuthenticationException;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.bind.annotation.RestControllerAdvice;

import java.net.URI;
import java.time.Instant;
import java.util.stream.Collectors;

@RestControllerAdvice(annotations = RestController.class)
public class GlobalExceptionHandler {

    @ExceptionHandler({AthleteNotFoundException.class, CompetitionNotFoundException.class})
    public ResponseEntity<ProblemDetail> handleNotFound(RuntimeException ex, HttpServletRequest request) {
        return problem(HttpStatus.NOT_FOUND, "Resource not found", ex.getMessage(), "/errors/not-found", request);
    }

    @ExceptionHandler(RegistrationException.class)
    public ResponseEntity<ProblemDetail> handleRegistration(RegistrationException ex, HttpServletRequest request) {
        return problem(HttpStatus.BAD_REQUEST, "Registration failed", ex.getMessage(), "/errors/registration", request);
    }

    @ExceptionHandler(MethodArgumentNotValidException.class)
    public ResponseEntity<ProblemDetail> handleValidation(MethodArgumentNotValidException ex, HttpServletRequest request) {
        String detail = ex.getBindingResult().getFieldErrors().stream()
                .map(fe -> fe.getField() + ": " + fe.getDefaultMessage())
                .collect(Collectors.joining(", "));
        return problem(HttpStatus.BAD_REQUEST, "Validation failed", detail, "/errors/validation", request);
    }

    @ExceptionHandler(AuthenticationException.class)
    public ResponseEntity<ProblemDetail> handleAuthentication(AuthenticationException ex, HttpServletRequest request) {
        return problem(HttpStatus.UNAUTHORIZED, "Authentication failed", ex.getMessage(), "/errors/auth", request);
    }

    @ExceptionHandler(AccessDeniedException.class)
    public ResponseEntity<ProblemDetail> handleAccessDenied(AccessDeniedException ex, HttpServletRequest request) {
        return problem(HttpStatus.FORBIDDEN, "Access denied", ex.getMessage(), "/errors/forbidden", request);
    }

    @ExceptionHandler(Exception.class)
    public ResponseEntity<ProblemDetail> handleGeneric(Exception ex, HttpServletRequest request) {
        return problem(HttpStatus.INTERNAL_SERVER_ERROR, "Internal error", ex.getMessage(), "/errors/internal", request);
    }

    private ResponseEntity<ProblemDetail> problem(HttpStatus status,
                                                   String title,
                                                   String detail,
                                                   String typeSuffix,
                                                   HttpServletRequest request) {
        ProblemDetail body = ProblemDetail.forStatusAndDetail(status, detail);
        body.setTitle(title);
        body.setType(URI.create(typeSuffix));
        body.setInstance(URI.create(request.getRequestURI()));
        body.setProperty("timestamp", Instant.now());
        return ResponseEntity.status(status).body(body);
    }
}
