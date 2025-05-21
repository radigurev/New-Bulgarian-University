package services.interfaces;

import models.Publication;
import models.base.Employee;
import models.enums.PaperSize;
import models.enums.PaperType;

import java.io.IOException;
import java.math.BigDecimal;
import java.nio.file.Path;

public interface IPrintingHouseService {
    void addMachine(String houseId, String machineId);
    void hireEmployee(String houseId, Employee e);
    BigDecimal buyPaper(String houseId, PaperType type, PaperSize size, int sheets);
    void loadMachine(String houseId, String machineId, int sheets);
    void executePrintJob(String houseId, Publication pub, String machineId) throws Exception;
    void paySalaries(String houseId);
    String generateReport(String houseId);
    void closePeriod(String houseId, Path reportPath) throws IOException;
    String importReport(String houseId, Path reportFile) throws IOException;
}
