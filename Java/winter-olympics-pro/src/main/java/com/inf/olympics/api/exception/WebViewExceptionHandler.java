package com.inf.olympics.api.exception;

import jakarta.servlet.http.HttpServletResponse;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ExceptionHandler;

@ControllerAdvice(annotations = Controller.class)
public class WebViewExceptionHandler {

    @ExceptionHandler({AthleteNotFoundException.class, CompetitionNotFoundException.class})
    public String handleNotFound(RuntimeException ex, Model model, HttpServletResponse response) {
        response.setStatus(HttpStatus.NOT_FOUND.value());
        model.addAttribute("errorTitle", "Resource not found");
        model.addAttribute("errorMessage", ex.getMessage());
        return "error";
    }
}
