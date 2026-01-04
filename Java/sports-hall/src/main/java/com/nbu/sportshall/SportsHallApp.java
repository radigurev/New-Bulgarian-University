package com.nbu.sportshall;

import java.util.ArrayList;
import java.util.EnumMap;
import java.util.List;
import java.util.Map;
import java.util.Queue;
import java.util.Random;

public final class SportsHallApp {

    public static void main(String[] args) {
        Map<SeatCategory, Integer> capacities = new EnumMap<>(SeatCategory.class);
        capacities.put(SeatCategory.VIP, 20);
        capacities.put(SeatCategory.PREMIUM, 40);
        capacities.put(SeatCategory.STANDARD, 60);

        SportsHall hall = new SportsHall(capacities);

        List<Group> groups = generateGroups(12);

        System.out.println("=== Sports hall seating simulation ===");
        System.out.println("Capacities:");
        for (SeatCategory c : hall.getCapacitySnapshot().keySet()) {
            System.out.printf("  %s: %d seats%n", c, hall.getCapacity(c));
        }

        System.out.println();
        System.out.println("Incoming groups:");
        for (Group g : groups) {
            System.out.println("  " + g);
        }

        System.out.println();
        System.out.println("Seating (4 entrances / 4 threads)...");
        Queue<Receipt> receipts = SeatingSimulation.seatAllWithParallelStream(hall, groups, 4, true, true);

        System.out.println();
        System.out.println("=== Final occupied seats by category ===");
        for (SeatCategory c : hall.getCapacitySnapshot().keySet()) {
            int occupied = hall.getOccupied(c);
            int available = hall.getAvailable(c);
            int capacity = hall.getCapacity(c);
            System.out.printf("  %s: occupied=%d / %d, available=%d%n", c, occupied, capacity, available);
        }

        int totalRequested = receipts.stream().mapToInt(Receipt::getRequested).sum();
        int totalSeated = receipts.stream().mapToInt(Receipt::getSeated).sum();
        int totalRejected = receipts.stream().mapToInt(Receipt::getRejected).sum();

        System.out.println();
        System.out.printf("Totals: requested=%d, seated=%d, rejected=%d%n", totalRequested, totalSeated, totalRejected);
    }

    private static List<Group> generateGroups(int count) {
        Random rnd = new Random();
        SeatCategory[] categories = SeatCategory.values();

        List<Group> groups = new ArrayList<>();
        for (int i = 1; i <= count; i++) {
            SeatCategory category = categories[rnd.nextInt(categories.length)];
            int spectators = 3 + rnd.nextInt(15);
            groups.add(new Group(i, category, spectators));
        }
        return groups;
    }
}
