package services.interfaces;

import models.base.Employee;

import java.io.IOException;
import java.nio.file.Path;
import java.util.List;

public interface IPersistenceService {
    void saveEmployees(String houseId, List<Employee> staff, Path file) throws IOException;
    List<Employee> loadEmployees(String houseId, Path file) throws IOException, ClassNotFoundException;
}
