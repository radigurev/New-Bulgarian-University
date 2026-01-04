package com.nbu.sportshall;

import java.util.List;
import java.util.Objects;
import java.util.Queue;
import java.util.Random;
import java.util.concurrent.ConcurrentLinkedQueue;
import java.util.concurrent.ForkJoinPool;
import java.util.concurrent.TimeUnit;

public final class SeatingSimulation {

    private SeatingSimulation() {
    }

    public static Queue<Receipt> seatAllWithParallelStream(SportsHall hall,
                                                           List<Group> groups,
                                                           int entrances,
                                                           boolean simulateDelay,
                                                           boolean printProgress) {
        Objects.requireNonNull(hall, "hall");
        Objects.requireNonNull(groups, "groups");
        if (entrances <= 0) {
            throw new IllegalArgumentException("entrances must be positive");
        }

        Queue<Receipt> receipts = new ConcurrentLinkedQueue<>();
        Random rnd = simulateDelay ? new Random() : null;

        ForkJoinPool pool = new ForkJoinPool(
                entrances,
                new EntranceThreadFactory(),
                null,
                false
        );

        try {
            pool.submit(() ->
                    groups.parallelStream().forEach(group -> {
                        if (simulateDelay) {
                            sleepQuietly(10 + rnd.nextInt(30));
                        }
                        Receipt receipt = hall.seatGroup(group);
                        receipts.add(receipt);

                        if (printProgress) {
                            System.out.printf("%s seated group %d (%s) -> requested=%d, seated=%d, rejected=%d%n",
                                    receipt.getEntrance(),
                                    receipt.getGroupId(),
                                    receipt.getCategory(),
                                    receipt.getRequested(),
                                    receipt.getSeated(),
                                    receipt.getRejected()
                            );
                        }
                    })
            ).get();

            pool.shutdown();
            pool.awaitTermination(10, TimeUnit.SECONDS);
            return receipts;
        } catch (Exception e) {
            pool.shutdownNow();
            throw new RuntimeException("Seating simulation failed", e);
        }
    }

    private static void sleepQuietly(long ms) {
        try {
            Thread.sleep(ms);
        } catch (InterruptedException ignored) {
            Thread.currentThread().interrupt();
        }
    }
}
