package com.nbu.sportshall;

import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.EnumMap;
import java.util.List;
import java.util.Map;
import java.util.Queue;
import java.util.Random;
import java.util.Set;
import java.util.stream.Collectors;

import static org.junit.jupiter.api.Assertions.*;

class SportsHallConcurrencyTest {

    @Test
    void seatingIsThreadSafe_neverExceedsCapacity_andNeverGoesNegative() {
        Map<SeatCategory, Integer> capacities = new EnumMap<>(SeatCategory.class);
        capacities.put(SeatCategory.VIP, 50);
        capacities.put(SeatCategory.PREMIUM, 50);
        capacities.put(SeatCategory.STANDARD, 50);

        SportsHall hall = new SportsHall(capacities);

        List<Group> groups = randomGroups(200, 1);

        Queue<Receipt> receipts = SeatingSimulation.seatAllWithParallelStream(
                hall,
                groups,
                4,
                false,
                false
        );

        // Check basic invariants per category
        for (SeatCategory category : capacities.keySet()) {
            int cap = hall.getCapacity(category);
            int available = hall.getAvailable(category);
            int occupied = hall.getOccupied(category);

            assertTrue(available >= 0, "available must be non-negative");
            assertTrue(occupied >= 0, "occupied must be non-negative");
            assertTrue(occupied <= cap, "occupied must not exceed capacity");
            assertEquals(cap, occupied + available, "capacity must equal occupied + available");

            int seatedFromReceipts = receipts.stream()
                    .filter(r -> r.getCategory() == category)
                    .mapToInt(Receipt::getSeated)
                    .sum();

            assertEquals(occupied, seatedFromReceipts, "occupied must equal sum of seated for " + category);
        }

        // Entrance threads are named and limited to 4
        Set<String> entrancesUsed = receipts.stream()
                .map(Receipt::getEntrance)
                .collect(Collectors.toSet());

        assertFalse(entrancesUsed.isEmpty());
        assertTrue(entrancesUsed.size() <= 4, "must use at most 4 entrance threads");
        assertTrue(entrancesUsed.stream().allMatch(n -> n.startsWith("Entrance-")));
    }

    private static List<Group> randomGroups(int count, long seed) {
        Random rnd = new Random(seed);
        SeatCategory[] categories = SeatCategory.values();
        List<Group> groups = new ArrayList<>(count);

        for (int i = 1; i <= count; i++) {
            SeatCategory category = categories[rnd.nextInt(categories.length)];
            int spectators = 1 + rnd.nextInt(10);
            groups.add(new Group(i, category, spectators));
        }
        return groups;
    }
}
