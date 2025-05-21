package repostories.implementations;

import models.PrintingHouse;
import repostories.PrintingHouseRepository;

import java.util.Map;
import java.util.NoSuchElementException;
import java.util.concurrent.ConcurrentHashMap;

public class PrintingHouseRepositoryImpl implements PrintingHouseRepository {
    private final Map<String, PrintingHouse> store = new ConcurrentHashMap<>();

    @Override
    public void save(PrintingHouse house) {
        store.put(house.getName(), house);
    }

    @Override
    public PrintingHouse get(String id) {
        PrintingHouse house = store.get(id);
        if (house == null) throw new NoSuchElementException("No printing house with id " + id);
        return house;
    }
}