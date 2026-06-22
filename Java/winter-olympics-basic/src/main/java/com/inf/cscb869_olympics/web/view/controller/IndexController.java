package com.inf.cscb869_olympics.web.view.controller;

import com.inf.cscb869_olympics.service.OlympicsService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;

@Controller
@RequiredArgsConstructor
public class IndexController {

    private final OlympicsService olympicsService;

    @GetMapping({"/", "/index"})
    public String index(Model model) {
        model.addAttribute("stats", olympicsService.getStats());
        return "index";
    }

    @GetMapping("/login")
    public String login() {
        return "login";
    }

    @GetMapping("/stats")
    public String stats(Model model) {
        model.addAttribute("stats", olympicsService.getStats());
        return "stats";
    }

    @GetMapping("/medals")
    public String medals(Model model) {
        model.addAttribute("stats", olympicsService.getStats());
        return "medals";
    }
}
