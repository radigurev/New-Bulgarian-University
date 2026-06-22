package com.inf.cscb869_olympics.web.view.controller;

import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;

@Controller
public class ErrorPageController {

    @GetMapping("/error/forbidden")
    public String forbidden(Model model) {
        model.addAttribute("message", "You do not have access to this resource.");
        return "errors/forbidden";
    }
}
