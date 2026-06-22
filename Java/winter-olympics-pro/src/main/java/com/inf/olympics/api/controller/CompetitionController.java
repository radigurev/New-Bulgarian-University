package com.inf.olympics.api.controller;

import com.inf.olympics.application.dto.BiathlonResultRequest;
import com.inf.olympics.application.dto.CompetitionDto;
import com.inf.olympics.application.dto.CreateCompetitionRequest;
import com.inf.olympics.application.dto.RankingEntryDto;
import com.inf.olympics.application.dto.SlalomRunRequest;
import com.inf.olympics.application.service.BiathlonService;
import com.inf.olympics.application.service.CompetitionService;
import com.inf.olympics.application.service.SlalomService;
import com.inf.olympics.domain.ranking.RankingService;
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
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/v1/competitions")
@RequiredArgsConstructor
@Tag(name = "Competitions", description = "Competition management and results")
public class CompetitionController {

    private final CompetitionService competitionService;
    private final SlalomService slalomService;
    private final BiathlonService biathlonService;
    private final RankingService rankingService;

    @GetMapping
    @Operation(summary = "List all competitions")
    public List<CompetitionDto> list() {
        return competitionService.findAll();
    }

    @GetMapping("/{id}")
    @Operation(summary = "Get a competition by id")
    public CompetitionDto get(@PathVariable Long id) {
        return competitionService.findById(id);
    }

    @PostMapping
    @PreAuthorize("hasAuthority('ROLE_ADMIN')")
    @Operation(summary = "Create a competition (admin)")
    public ResponseEntity<CompetitionDto> create(@Valid @RequestBody CreateCompetitionRequest request) {
        return ResponseEntity.status(HttpStatus.CREATED).body(competitionService.create(request));
    }

    @PutMapping("/{id}")
    @PreAuthorize("hasAuthority('ROLE_ADMIN')")
    @Operation(summary = "Update a competition (admin)")
    public CompetitionDto update(@PathVariable Long id, @Valid @RequestBody CreateCompetitionRequest request) {
        return competitionService.update(id, request);
    }

    @DeleteMapping("/{id}")
    @PreAuthorize("hasAuthority('ROLE_ADMIN')")
    @Operation(summary = "Delete a competition (admin)")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        competitionService.delete(id);
        return ResponseEntity.noContent().build();
    }

    @PostMapping("/{id}/finish")
    @PreAuthorize("hasAuthority('ROLE_ADMIN')")
    @Operation(summary = "Mark competition as finished (admin)")
    public CompetitionDto finish(@PathVariable Long id) {
        return competitionService.finish(id);
    }

    @PostMapping("/{competitionId}/registrations/{athleteId}")
    @PreAuthorize("hasAnyAuthority('ROLE_ADMIN','ROLE_ATHLETE')")
    @Operation(summary = "Register an athlete to a competition")
    public ResponseEntity<Void> register(@PathVariable Long competitionId, @PathVariable Long athleteId) {
        competitionService.register(competitionId, athleteId);
        return ResponseEntity.status(HttpStatus.CREATED).build();
    }

    @PostMapping("/{id}/slalom/run-one")
    @PreAuthorize("hasAuthority('ROLE_ADMIN')")
    @Operation(summary = "Record a ski-slalom run-one time (admin)")
    public ResponseEntity<Void> slalomRunOne(@PathVariable Long id, @Valid @RequestBody SlalomRunRequest request) {
        slalomService.recordRunOne(id, request);
        return ResponseEntity.status(HttpStatus.CREATED).build();
    }

    @PostMapping("/{id}/slalom/run-two")
    @PreAuthorize("hasAuthority('ROLE_ADMIN')")
    @Operation(summary = "Record a ski-slalom run-two time (admin)")
    public ResponseEntity<Void> slalomRunTwo(@PathVariable Long id, @Valid @RequestBody SlalomRunRequest request) {
        slalomService.recordRunTwo(id, request);
        return ResponseEntity.status(HttpStatus.CREATED).build();
    }

    @GetMapping("/{id}/slalom/run-two/start-list")
    @Operation(summary = "Get the run-two start list (slowest qualifier first)")
    public List<RankingEntryDto> slalomStartList(@PathVariable Long id) {
        return slalomService.getRunTwoStartList(id);
    }

    @PostMapping("/{id}/biathlon/results")
    @PreAuthorize("hasAuthority('ROLE_ADMIN')")
    @Operation(summary = "Record a biathlon result (admin)")
    public ResponseEntity<Void> biathlonResult(@PathVariable Long id, @Valid @RequestBody BiathlonResultRequest request) {
        biathlonService.recordResult(id, request);
        return ResponseEntity.status(HttpStatus.CREATED).build();
    }

    @GetMapping("/{id}/ranking")
    @Operation(summary = "Get the final ranking for a competition")
    public List<RankingEntryDto> ranking(@PathVariable Long id) {
        return rankingService.rank(id);
    }
}
