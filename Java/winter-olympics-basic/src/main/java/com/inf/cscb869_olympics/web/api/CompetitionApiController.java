package com.inf.cscb869_olympics.web.api;

import com.inf.cscb869_olympics.dto.BiathlonResultDTO;
import com.inf.cscb869_olympics.dto.CompetitionDTO;
import com.inf.cscb869_olympics.dto.CreateCompetitionDTO;
import com.inf.cscb869_olympics.dto.RankingEntryDTO;
import com.inf.cscb869_olympics.dto.SlalomRunDTO;
import com.inf.cscb869_olympics.service.BiathlonService;
import com.inf.cscb869_olympics.service.CompetitionService;
import com.inf.cscb869_olympics.service.OlympicsService;
import com.inf.cscb869_olympics.service.SlalomService;
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
@RequestMapping("/api/competitions")
@RequiredArgsConstructor
public class CompetitionApiController {

    private final CompetitionService competitionService;
    private final SlalomService slalomService;
    private final BiathlonService biathlonService;
    private final OlympicsService olympicsService;

    @GetMapping
    public List<CompetitionDTO> list() {
        return competitionService.getCompetitions();
    }

    @GetMapping("/{id}")
    public CompetitionDTO get(@PathVariable Long id) {
        return competitionService.getCompetition(id);
    }

    @PostMapping
    @PreAuthorize("hasAuthority('admin')")
    public ResponseEntity<CompetitionDTO> create(@Valid @RequestBody CreateCompetitionDTO dto) {
        return ResponseEntity.status(HttpStatus.CREATED).body(competitionService.createCompetition(dto));
    }

    @PutMapping("/{id}")
    @PreAuthorize("hasAuthority('admin')")
    public CompetitionDTO update(@PathVariable Long id, @Valid @RequestBody CreateCompetitionDTO dto) {
        return competitionService.updateCompetition(id, dto);
    }

    @DeleteMapping("/{id}")
    @PreAuthorize("hasAuthority('admin')")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        competitionService.deleteCompetition(id);
        return ResponseEntity.noContent().build();
    }

    @PostMapping("/{id}/finish")
    @PreAuthorize("hasAuthority('admin')")
    public CompetitionDTO finish(@PathVariable Long id) {
        return competitionService.finishCompetition(id);
    }

    @PostMapping("/{competitionId}/register/{athleteId}")
    @PreAuthorize("hasAuthority('admin') or hasAuthority('athlete')")
    public ResponseEntity<Void> register(@PathVariable Long competitionId, @PathVariable Long athleteId) {
        competitionService.registerAthlete(competitionId, athleteId);
        return ResponseEntity.status(HttpStatus.CREATED).build();
    }

    @PostMapping("/{id}/slalom/run-one")
    @PreAuthorize("hasAuthority('admin')")
    public ResponseEntity<Void> slalomRunOne(@PathVariable Long id, @Valid @RequestBody SlalomRunDTO dto) {
        slalomService.recordRunOne(id, dto);
        return ResponseEntity.status(HttpStatus.CREATED).build();
    }

    @PostMapping("/{id}/slalom/run-two")
    @PreAuthorize("hasAuthority('admin')")
    public ResponseEntity<Void> slalomRunTwo(@PathVariable Long id, @Valid @RequestBody SlalomRunDTO dto) {
        slalomService.recordRunTwo(id, dto);
        return ResponseEntity.status(HttpStatus.CREATED).build();
    }

    @GetMapping("/{id}/slalom/run-two/start-list")
    public List<RankingEntryDTO> slalomStartList(@PathVariable Long id) {
        return slalomService.getRunTwoStartList(id);
    }

    @PostMapping("/{id}/biathlon/result")
    @PreAuthorize("hasAuthority('admin')")
    public ResponseEntity<Void> biathlonResult(@PathVariable Long id, @Valid @RequestBody BiathlonResultDTO dto) {
        biathlonService.recordResult(id, dto);
        return ResponseEntity.status(HttpStatus.CREATED).build();
    }

    @GetMapping("/{id}/ranking")
    public List<RankingEntryDTO> ranking(@PathVariable Long id) {
        return olympicsService.getRanking(id);
    }
}
