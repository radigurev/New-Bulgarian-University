package com.inf.cscb869_olympics.exception;

import org.springframework.security.access.AccessDeniedException;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ExceptionHandler;

@ControllerAdvice(basePackages = "com.inf.cscb869_olympics.web.view")
public class ViewExceptionHandler {

    @ExceptionHandler({AthleteNotFoundException.class, CompetitionNotFoundException.class})
    public String handleNotFound(RuntimeException ex, Model model) {
        model.addAttribute("message", ex.getMessage());
        return "errors/not-found";
    }

    @ExceptionHandler(AccessDeniedException.class)
    public String handleAccessDenied(AccessDeniedException ex, Model model) {
        model.addAttribute("message", ex.getMessage());
        return "errors/forbidden";
    }

    @ExceptionHandler(RegistrationException.class)
    public String handleBadRequest(RegistrationException ex, Model model) {
        model.addAttribute("message", ex.getMessage());
        return "errors/bad-request";
    }

    @ExceptionHandler(Exception.class)
    public String handleAny(Exception ex, Model model) {
        model.addAttribute("message", ex.getMessage());
        return "errors/generic";
    }
}
