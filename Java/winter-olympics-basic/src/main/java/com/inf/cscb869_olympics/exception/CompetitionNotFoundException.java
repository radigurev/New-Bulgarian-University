package com.inf.cscb869_olympics.exception;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ResponseStatus;

@ResponseStatus(HttpStatus.NOT_FOUND)
public class CompetitionNotFoundException extends RuntimeException {

    public CompetitionNotFoundException(Long id) {
        super("Competition with id " + id + " was not found");
    }
}
