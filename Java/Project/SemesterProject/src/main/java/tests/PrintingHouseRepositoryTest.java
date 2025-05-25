package tests;

import models.PrintingHouse;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import repostories.PrintingHouseRepository;
import repostories.implementations.PrintingHouseRepositoryImpl;

import java.util.NoSuchElementException;

import static org.junit.jupiter.api.Assertions.*;

class PrintingHouseRepositoryTest {

    private PrintingHouseRepository repo;

    @BeforeEach
    void setUp() {
        repo = new PrintingHouseRepositoryImpl();
    }

    @Test
    void addAndGetHouse_successful() {
        PrintingHouse house = new PrintingHouse("h1");
        repo.save(house);

        PrintingHouse result = repo.get("h1");

        assertSame(house, result);
    }

    @Test
    void add_duplicateId_overwritesExisting() {
        PrintingHouse first = new PrintingHouse("h1");
        PrintingHouse second = new PrintingHouse("h1");

        repo.save(first);
        repo.save(second);

        PrintingHouse result = repo.get("h1");

        assertSame(second, result, "The second insertion should overwrite the first one");
    }

    @Test
    void get_nonexistent_throws() {
        assertThrows(NoSuchElementException.class, () -> repo.get("no-such"));
    }
}
