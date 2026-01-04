package com.nbu.sportshall;

import java.util.Objects;

public final class Group {
    private final int id;
    private final SeatCategory category;
    private final int spectators;

    public Group(int id, SeatCategory category, int spectators) {
        if (id <= 0) {
            throw new IllegalArgumentException("id must be positive");
        }
        this.id = id;
        this.category = Objects.requireNonNull(category, "category");
        if (spectators <= 0) {
            throw new IllegalArgumentException("spectators must be positive");
        }
        this.spectators = spectators;
    }

    public int getId() {
        return id;
    }

    public SeatCategory getCategory() {
        return category;
    }

    public int getSpectators() {
        return spectators;
    }

    @Override
    public String toString() {
        return "Group{id=" + id + ", category=" + category + ", spectators=" + spectators + "}";
    }
}
