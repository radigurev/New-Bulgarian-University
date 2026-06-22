package com.inf.cscb869_olympics.web.view.controller;

import com.inf.cscb869_olympics.dto.CreateCompetitionDTO;
import com.inf.cscb869_olympics.service.CompetitionService;
import com.inf.cscb869_olympics.service.OlympicsService;
import com.inf.cscb869_olympics.util.MapperUtil;
import com.inf.cscb869_olympics.web.view.controller.model.CompetitionViewModel;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.servlet.mvc.support.RedirectAttributes;
import com.inf.cscb869_olympics.exception.RegistrationException;

@Controller
@RequiredArgsConstructor
public class CompetitionViewController {

    private final CompetitionService competitionService;
    private final OlympicsService olympicsService;
    private final MapperUtil mapperUtil;

    @GetMapping("/competitions")
    public String list(Model model) {
        model.addAttribute("competitions", competitionService.getCompetitions());
        return "competitions/list";
    }

    @GetMapping("/competitions/{id}/ranking")
    public String ranking(@PathVariable Long id, Model model) {
        model.addAttribute("competition", competitionService.getCompetition(id));
        model.addAttribute("ranking", olympicsService.getRanking(id));
        return "competitions/ranking";
    }

    @GetMapping("/admin/competitions/create")
    @PreAuthorize("hasAuthority('admin')")
    public String createForm(Model model) {
        model.addAttribute("competition", new CompetitionViewModel());
        return "competitions/create";
    }

    @PostMapping("/admin/competitions/create")
    @PreAuthorize("hasAuthority('admin')")
    public String create(@Valid @ModelAttribute("competition") CompetitionViewModel viewModel,
                         BindingResult result) {
        if (result.hasErrors()) {
            return "competitions/create";
        }
        competitionService.createCompetition(mapperUtil.map(viewModel, CreateCompetitionDTO.class));
        return "redirect:/competitions";
    }

    @PostMapping("/admin/competitions/{id}/finish")
    @PreAuthorize("hasAuthority('admin')")
    public String finish(@PathVariable Long id, RedirectAttributes redirectAttributes) {
        com.inf.cscb869_olympics.dto.CompetitionDTO competition = competitionService.finishCompetition(id);
        redirectAttributes.addFlashAttribute("flashSuccess", "\"" + competition.getName() + "\" marked as finished.");
        return "redirect:/competitions";
    }

    @PostMapping("/admin/competitions/{id}/delete")
    @PreAuthorize("hasAuthority('admin')")
    public String delete(@PathVariable Long id, RedirectAttributes redirectAttributes) {
        competitionService.deleteCompetition(id);
        redirectAttributes.addFlashAttribute("flashSuccess", "Competition deleted.");
        return "redirect:/competitions";
    }

    @PostMapping("/competitions/{competitionId}/register")
    @PreAuthorize("hasAuthority('athlete')")
    public String register(@PathVariable Long competitionId,
                           org.springframework.security.core.Authentication auth,
                           RedirectAttributes redirectAttributes) {
        com.inf.cscb869_olympics.data.entity.User user = (com.inf.cscb869_olympics.data.entity.User) auth.getPrincipal();
        if (user.getAthlete() == null) {
            redirectAttributes.addFlashAttribute("flashError", "Your account has no athlete profile.");
            return "redirect:/competitions";
        }
        try {
            competitionService.registerAthlete(competitionId, user.getAthlete().getId());
            redirectAttributes.addFlashAttribute("flashSuccess", "Registered successfully.");
        } catch (RegistrationException ex) {
            redirectAttributes.addFlashAttribute("flashError", ex.getMessage());
        }
        return "redirect:/competitions";
    }
}
