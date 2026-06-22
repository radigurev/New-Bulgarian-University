package com.inf.olympics.api.controller;

import com.inf.olympics.application.dto.OlympicsStatsDto;
import com.inf.olympics.application.service.OlympicsAggregateService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/v1/public")
@RequiredArgsConstructor
@Tag(name = "Public", description = "Public read-only endpoints (no authentication required)")
public class PublicController {

    private final OlympicsAggregateService aggregateService;

    @GetMapping("/stats")
    @Operation(summary = "Aggregate Olympics statistics — medal table, average age, youngest/oldest medalist")
    public OlympicsStatsDto stats() {
        return aggregateService.getStats();
    }
}
