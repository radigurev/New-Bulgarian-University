package services.implementations;

import models.base.Employee;
import services.interfaces.IPersistenceService;

import java.io.*;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;

public class PersistenceService implements IPersistenceService {
    @Override
    public void saveEmployees(String houseId, List<Employee> staff, Path file) throws IOException {
        Path parent = file.getParent();
        if (parent != null) Files.createDirectories(parent);

        try (ObjectOutputStream oos = new ObjectOutputStream(
                new BufferedOutputStream(Files.newOutputStream(file)))) {
            oos.writeObject(staff);
        }
    }

    @Override
    @SuppressWarnings("unchecked")
    public List<Employee> loadEmployees(String houseId, Path file) throws IOException, ClassNotFoundException {
        try (ObjectInputStream ois = new ObjectInputStream(
                new BufferedInputStream(Files.newInputStream(file)))) {
            return (List<Employee>) ois.readObject();
        }
    }
}