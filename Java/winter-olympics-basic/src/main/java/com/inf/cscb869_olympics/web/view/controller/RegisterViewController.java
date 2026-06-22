package com.inf.cscb869_olympics.web.view.controller;

import com.inf.cscb869_olympics.dto.CreateAthleteDTO;
import com.inf.cscb869_olympics.dto.RegisterUserDTO;
import com.inf.cscb869_olympics.service.UserService;
import com.inf.cscb869_olympics.web.view.controller.model.RegisterUserViewModel;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PostMapping;

@Controller
@RequiredArgsConstructor
public class RegisterViewController {

    private final UserService userService;

    @GetMapping("/register")
    public String form(Model model) {
        if (isAuthenticated()) {
            return "redirect:/";
        }
        model.addAttribute("user", new RegisterUserViewModel());
        return "register";
    }

    @PostMapping("/register")
    public String submit(@Valid @ModelAttribute("user") RegisterUserViewModel viewModel,
                         BindingResult result,
                         Model model) {
        if (isAuthenticated()) {
            return "redirect:/";
        }
        if (result.hasErrors()) {
            return "register";
        }
        try {
            RegisterUserDTO dto = new RegisterUserDTO();
            dto.setUsername(viewModel.getUsername());
            dto.setPassword(viewModel.getPassword());
            CreateAthleteDTO athlete = new CreateAthleteDTO();
            athlete.setFirstName(viewModel.getFirstName());
            athlete.setLastName(viewModel.getLastName());
            athlete.setCountry(viewModel.getCountry());
            athlete.setGender(viewModel.getGender());
            athlete.setDateOfBirth(viewModel.getDateOfBirth());
            dto.setAthlete(athlete);
            userService.registerAthlete(dto);
        } catch (RuntimeException ex) {
            model.addAttribute("error", ex.getMessage());
            return "register";
        }
        return "redirect:/login?registered";
    }

    private static boolean isAuthenticated() {
        Authentication auth = SecurityContextHolder.getContext().getAuthentication();
        return auth != null
                && auth.isAuthenticated()
                && !"anonymousUser".equals(auth.getPrincipal());
    }
}
