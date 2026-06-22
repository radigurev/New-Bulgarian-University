package com.inf.olympics.api.exception;

public class AthleteNotFoundException extends RuntimeException {

    public AthleteNotFoundException(Long id) {
        super("Athlete with id " + id + " was not found");
    }
}
