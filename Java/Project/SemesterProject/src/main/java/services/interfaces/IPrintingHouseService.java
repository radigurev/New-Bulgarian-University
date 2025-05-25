package services.interfaces;

import exceptions.NoPaperException;
import exceptions.OverCapacityException;
import exceptions.UnsupportedPrintModeException;
import models.Publication;
import models.base.Employee;
import models.enums.PaperSize;
import models.enums.PaperType;
import models.enums.PrintType;

import java.io.IOException;
import java.math.BigDecimal;
import java.nio.file.Path;

public interface IPrintingHouseService {
    void addMachine(String houseId, String machineId, PrintType printType, int pagePerMinute, int capacity);
    void hireEmployee(String houseId, Employee e);
    BigDecimal buyPaper(String houseId, PaperType type, PaperSize size, int sheets);
    void loadMachine(String houseId, String machineId, int sheets) throws OverCapacityException;
    void executePrintJob(String houseId, Publication pub, String machineId) throws UnsupportedPrintModeException, NoPaperException;
    void paySalaries(String houseId);
    String generateReport(String houseId) throws IOException;
    void closePeriod(String houseId, Path reportPath) throws IOException;
    String importReport(String houseId, Path reportFile) throws IOException;

    int getPrintedPages(String houseId, String machineId);

    void saveEmployees(String houseId, Path path) throws IOException;

    void loadEmployees(String houseId, Path path) throws IOException, ClassNotFoundException;

}
