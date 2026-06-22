package com.inf.olympics.application.dto;

public record MedalCountDto(
        String country,
        long gold,
        long silver,
        long bronze
) {
    public long total() {
        return gold + silver + bronze;
    }
}
