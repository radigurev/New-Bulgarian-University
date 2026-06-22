package com.inf.olympics.api.controller;

import com.inf.olympics.application.service.CompetitionService;
import com.inf.olympics.application.service.OlympicsAggregateService;
import com.inf.olympics.domain.ranking.RankingService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;

@Controller
@RequiredArgsConstructor
public class WebController {

    private final OlympicsAggregateService aggregateService;
    private final CompetitionService competitionService;
    private final RankingService rankingService;

    @GetMapping({"/", "/index"})
    public String index(Model model) {
        model.addAttribute("stats", aggregateService.getStats());
        return "index";
    }

    @GetMapping("/login")
    public String login() {
        return "login";
    }

    @GetMapping("/register")
    public String register() {
        return "register-info";
    }

    @GetMapping("/stats")
    public String stats(Model model) {
        model.addAttribute("stats", aggregateService.getStats());
        return "stats";
    }

    @GetMapping("/medals")
    public String medals(Model model) {
        model.addAttribute("stats", aggregateService.getStats());
        return "medals";
    }

    @GetMapping("/competitions")
    public String competitions(Model model) {
        model.addAttribute("competitions", competitionService.findAll());
        return "competitions";
    }

    @GetMapping("/competitions/{id}/ranking")
    public String ranking(@PathVariable Long id, Model model) {
        model.addAttribute("competition", competitionService.findById(id));
        model.addAttribute("ranking", rankingService.rank(id));
        return "ranking";
    }
}
