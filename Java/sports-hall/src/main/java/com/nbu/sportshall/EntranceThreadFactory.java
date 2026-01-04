package com.nbu.sportshall;

import java.util.concurrent.ForkJoinPool;
import java.util.concurrent.ForkJoinWorkerThread;
import java.util.concurrent.atomic.AtomicInteger;

public final class EntranceThreadFactory implements ForkJoinPool.ForkJoinWorkerThreadFactory {

    private final AtomicInteger counter = new AtomicInteger(0);

    @Override
    public ForkJoinWorkerThread newThread(ForkJoinPool pool) {
        ForkJoinWorkerThread t = ForkJoinPool.defaultForkJoinWorkerThreadFactory.newThread(pool);
        int id = counter.incrementAndGet();
        t.setName("Entrance-" + id);
        t.setDaemon(false);
        return t;
    }
}
