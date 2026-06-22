package com.inf.olympics.domain.model;

public enum Medal {
    GOLD,
    SILVER,
    BRONZE;

    public static Medal forPosition(int position) {
        return switch (position) {
            case 1 -> GOLD;
            case 2 -> SILVER;
            case 3 -> BRONZE;
            default -> null;
        };
    }
}
