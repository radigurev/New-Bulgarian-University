package com.inf.cscb869_olympics.web.api;

import com.inf.cscb869_olympics.dto.OlympicsStatsDTO;
import com.inf.cscb869_olympics.service.OlympicsService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/public")
@RequiredArgsConstructor
public class PublicApiController {

    private final OlympicsService olympicsService;

    @GetMapping("/stats")
    public OlympicsStatsDTO stats() {
        return olympicsService.getStats();
    }
}
