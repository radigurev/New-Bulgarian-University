package com.nbu.sportshall;

import java.time.Instant;
import java.util.Objects;

public final class Receipt {
    private final int groupId;
    private final SeatCategory category;
    private final int requested;
    private final int seated;
    private final int rejected;
    private final String entrance;
    private final Instant timestamp;

    public Receipt(int groupId,
                   SeatCategory category,
                   int requested,
                   int seated,
                   int rejected,
                   String entrance,
                   Instant timestamp) {
        this.groupId = groupId;
        this.category = Objects.requireNonNull(category, "category");
        this.requested = requested;
        this.seated = seated;
        this.rejected = rejected;
        this.entrance = Objects.requireNonNull(entrance, "entrance");
        this.timestamp = Objects.requireNonNull(timestamp, "timestamp");
    }

    public int getGroupId() {
        return groupId;
    }

    public SeatCategory getCategory() {
        return category;
    }

    public int getRequested() {
        return requested;
    }

    public int getSeated() {
        return seated;
    }

    public int getRejected() {
        return rejected;
    }

    public String getEntrance() {
        return entrance;
    }

    public Instant getTimestamp() {
        return timestamp;
    }

    @Override
    public String toString() {
        return "Receipt{groupId=" + groupId +
                ", category=" + category +
                ", requested=" + requested +
                ", seated=" + seated +
                ", rejected=" + rejected +
                ", entrance='" + entrance + "'" +
                ", timestamp=" + timestamp +
                '}';
    }
}
