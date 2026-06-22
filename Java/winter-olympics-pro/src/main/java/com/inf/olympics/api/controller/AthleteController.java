package com.inf.olympics.api.controller;

import com.inf.olympics.application.dto.AthleteDto;
import com.inf.olympics.application.dto.CreateAthleteRequest;
import com.inf.olympics.application.service.AthleteService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/v1/athletes")
@RequiredArgsConstructor
@Tag(name = "Athletes", description = "Athlete management endpoints")
public class AthleteController {

    private final AthleteService athleteService;

    @GetMapping
    @Operation(summary = "List athletes (optionally filtered by country or last name)")
    public List<AthleteDto> list(@RequestParam(required = false) String country,
                                 @RequestParam(required = false) String lastName) {
        if (country != null) return athleteService.searchByCountry(country);
        if (lastName != null) return athleteService.searchByLastName(lastName);
        return athleteService.findAll();
    }

    @GetMapping("/{id}")
    @Operation(summary = "Get an athlete by id")
    public AthleteDto get(@PathVariable Long id) {
        return athleteService.findById(id);
    }

    @PostMapping
    @PreAuthorize("hasAuthority('ROLE_ADMIN')")
    @Operation(summary = "Create an athlete (admin)")
    public ResponseEntity<AthleteDto> create(@Valid @RequestBody CreateAthleteRequest request) {
        return ResponseEntity.status(HttpStatus.CREATED).body(athleteService.create(request));
    }

    @PutMapping("/{id}")
    @PreAuthorize("hasAuthority('ROLE_ADMIN')")
    @Operation(summary = "Update an athlete (admin)")
    public AthleteDto update(@PathVariable Long id, @Valid @RequestBody CreateAthleteRequest request) {
        return athleteService.update(id, request);
    }

    @DeleteMapping("/{id}")
    @PreAuthorize("hasAuthority('ROLE_ADMIN')")
    @Operation(summary = "Delete an athlete (admin)")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        athleteService.delete(id);
        return ResponseEntity.noContent().build();
    }
}
