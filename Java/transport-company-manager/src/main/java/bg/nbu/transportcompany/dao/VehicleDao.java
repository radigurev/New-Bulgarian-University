package bg.nbu.transportcompany.dao;

import bg.nbu.transportcompany.entity.Vehicle;
import bg.nbu.transportcompany.entity.VehicleType;

import java.util.List;
import java.util.Optional;

public interface VehicleDao {

    Vehicle save(Vehicle vehicle);

    Optional<Vehicle> findById(Long id);

    List<Vehicle> findAll();

    List<Vehicle> findAllByCompany(Long companyId);

    Vehicle updateById(Long id, String plateNumber, VehicleType type, int seatCapacity, double maxLoadKg);

    void deleteById(Long id);
}
