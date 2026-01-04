# Sports Hall Seating (Console Java App)

Task summary:
- Keep info about seat categories and maximum seats per category.
- Groups of spectators enter through entrances (threads).
- Seat each group and update occupied seats per category.
- Run concurrently at least 10 groups in **4 entrances (threads)**.
- After all groups are seated, print occupied seats per category.

Implementation highlights:
- `ConcurrentHashMap<SeatCategory, AtomicInteger>` for available seats per category
- CAS (`compareAndSet`) for safe decrements (no negative seats)
- `ConcurrentLinkedQueue<Receipt>` to collect results
- **Parallel looping:** `groups.parallelStream()` executed in a custom `ForkJoinPool(4)`
- Entrance thread names: `Entrance-1` ... `Entrance-4`

## Run
```bash
mvn -q test
mvn -q -DskipTests package
java -cp target/sports-hall-1.0.0.jar com.nbu.sportshall.SportsHallApp
```
