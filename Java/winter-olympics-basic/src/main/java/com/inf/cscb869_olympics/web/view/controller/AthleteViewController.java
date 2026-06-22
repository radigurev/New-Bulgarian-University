package com.inf.cscb869_olympics.web.view.controller;

import com.inf.cscb869_olympics.dto.CreateAthleteDTO;
import com.inf.cscb869_olympics.service.AthleteService;
import com.inf.cscb869_olympics.util.MapperUtil;
import com.inf.cscb869_olympics.web.view.controller.model.AthleteViewModel;
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

@Controller
@RequestMapping("/admin/athletes")
@RequiredArgsConstructor
@PreAuthorize("hasAuthority('admin')")
public class AthleteViewController {

    private final AthleteService athleteService;
    private final MapperUtil mapperUtil;

    @GetMapping
    public String list(Model model) {
        model.addAttribute("athletes", athleteService.getAthletes());
        return "athletes/list";
    }

    @GetMapping("/create")
    public String createForm(Model model) {
        model.addAttribute("athlete", new AthleteViewModel());
        return "athletes/create";
    }

    @PostMapping("/create")
    public String create(@Valid @ModelAttribute("athlete") AthleteViewModel viewModel,
                         BindingResult result) {
        if (result.hasErrors()) {
            return "athletes/create";
        }
        athleteService.createAthlete(mapperUtil.map(viewModel, CreateAthleteDTO.class));
        return "redirect:/admin/athletes";
    }

    @GetMapping("/edit/{id}")
    public String editForm(@PathVariable Long id, Model model) {
        model.addAttribute("athlete", mapperUtil.map(athleteService.getAthlete(id), AthleteViewModel.class));
        return "athletes/edit";
    }

    @PostMapping("/edit/{id}")
    public String edit(@PathVariable Long id,
                       @Valid @ModelAttribute("athlete") AthleteViewModel viewModel,
                       BindingResult result) {
        if (result.hasErrors()) {
            return "athletes/edit";
        }
        athleteService.updateAthlete(id, mapperUtil.map(viewModel, CreateAthleteDTO.class));
        return "redirect:/admin/athletes";
    }

    @PostMapping("/delete/{id}")
    public String delete(@PathVariable Long id) {
        athleteService.deleteAthlete(id);
        return "redirect:/admin/athletes";
    }
}
