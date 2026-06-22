package com.inf.cscb869_olympics.web.api;

import com.inf.cscb869_olympics.dto.AthleteDTO;
import com.inf.cscb869_olympics.dto.CreateAthleteDTO;
import com.inf.cscb869_olympics.service.AthleteService;
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
@RequestMapping("/api/athletes")
@RequiredArgsConstructor
public class AthleteApiController {

    private final AthleteService athleteService;

    @GetMapping
    public List<AthleteDTO> list(@RequestParam(required = false) String country,
                                 @RequestParam(required = false) String lastName) {
        if (country != null) return athleteService.searchByCountry(country);
        if (lastName != null) return athleteService.searchByLastName(lastName);
        return athleteService.getAthletes();
    }

    @GetMapping("/{id}")
    public AthleteDTO get(@PathVariable Long id) {
        return athleteService.getAthlete(id);
    }

    @PostMapping
    @PreAuthorize("hasAuthority('admin')")
    public ResponseEntity<AthleteDTO> create(@Valid @RequestBody CreateAthleteDTO dto) {
        return ResponseEntity.status(HttpStatus.CREATED).body(athleteService.createAthlete(dto));
    }

    @PutMapping("/{id}")
    @PreAuthorize("hasAuthority('admin')")
    public AthleteDTO update(@PathVariable Long id, @Valid @RequestBody CreateAthleteDTO dto) {
        return athleteService.updateAthlete(id, dto);
    }

    @DeleteMapping("/{id}")
    @PreAuthorize("hasAuthority('admin')")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        athleteService.deleteAthlete(id);
        return ResponseEntity.noContent().build();
    }
}
