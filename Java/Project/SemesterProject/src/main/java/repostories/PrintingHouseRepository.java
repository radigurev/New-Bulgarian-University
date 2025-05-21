package repostories;

import models.PrintingHouse;

public interface PrintingHouseRepository {
    void save(PrintingHouse house);
    PrintingHouse get(String id);
}
