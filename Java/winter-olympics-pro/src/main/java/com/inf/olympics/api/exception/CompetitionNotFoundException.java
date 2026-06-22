package com.inf.olympics.api.exception;

public class CompetitionNotFoundException extends RuntimeException {

    public CompetitionNotFoundException(Long id) {
        super("Competition with id " + id + " was not found");
    }
}
