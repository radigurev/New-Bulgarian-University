package com.nbu.sportshall;

import java.time.Instant;
import java.util.Collections;
import java.util.EnumMap;
import java.util.Map;
import java.util.Objects;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.atomic.AtomicInteger;

public final class SportsHall {

    private final Map<SeatCategory, Integer> capacityByCategory;
    private final Map<SeatCategory, AtomicInteger> availableByCategory;

    public SportsHall(Map<SeatCategory, Integer> maxSeatsPerCategory) {
        Objects.requireNonNull(maxSeatsPerCategory, "maxSeatsPerCategory");
        if (maxSeatsPerCategory.isEmpty()) {
            throw new IllegalArgumentException("At least one category is required");
        }

        EnumMap<SeatCategory, Integer> capacity = new EnumMap<>(SeatCategory.class);
        for (Map.Entry<SeatCategory, Integer> entry : maxSeatsPerCategory.entrySet()) {
            SeatCategory category = Objects.requireNonNull(entry.getKey(), "category");
            Integer value = Objects.requireNonNull(entry.getValue(), "capacity");
            if (value < 0) {
                throw new IllegalArgumentException("Capacity cannot be negative for " + category);
            }
            capacity.put(category, value);
        }
        this.capacityByCategory = Collections.unmodifiableMap(capacity);

        this.availableByCategory = new ConcurrentHashMap<>();
        for (SeatCategory category : capacity.keySet()) {
            this.availableByCategory.put(category, new AtomicInteger(capacity.get(category)));
        }
    }

    public Receipt seatGroup(Group group) {
        Objects.requireNonNull(group, "group");
        SeatCategory category = group.getCategory();

        AtomicInteger available = availableByCategory.get(category);
        if (available == null) {
            throw new IllegalArgumentException("Category not configured in hall: " + category);
        }

        int requested = group.getSpectators();
        int seated = tryTakeSeats(available, requested);
        int rejected = requested - seated;

        return new Receipt(
                group.getId(),
                category,
                requested,
                seated,
                rejected,
                Thread.currentThread().getName(),
                Instant.now()
        );
    }

    private static int tryTakeSeats(AtomicInteger available, int requested) {
        while (true) {
            int current = available.get();
            if (current == 0) {
                return 0;
            }
            int toSeat = Math.min(current, requested);
            if (available.compareAndSet(current, current - toSeat)) {
                return toSeat;
            }
        }
    }

    public int getCapacity(SeatCategory category) {
        Integer cap = capacityByCategory.get(category);
        if (cap == null) {
            throw new IllegalArgumentException("Unknown category: " + category);
        }
        return cap;
    }

    public int getAvailable(SeatCategory category) {
        AtomicInteger avail = availableByCategory.get(category);
        if (avail == null) {
            throw new IllegalArgumentException("Unknown category: " + category);
        }
        return avail.get();
    }

    public int getOccupied(SeatCategory category) {
        return getCapacity(category) - getAvailable(category);
    }

    public Map<SeatCategory, Integer> getOccupiedSnapshot() {
        EnumMap<SeatCategory, Integer> snapshot = new EnumMap<>(SeatCategory.class);
        for (SeatCategory category : capacityByCategory.keySet()) {
            snapshot.put(category, getOccupied(category));
        }
        return snapshot;
    }

    public Map<SeatCategory, Integer> getCapacitySnapshot() {
        return capacityByCategory;
    }
}
