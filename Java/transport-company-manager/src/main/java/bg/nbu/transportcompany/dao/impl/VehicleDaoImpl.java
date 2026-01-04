package bg.nbu.transportcompany.dao.impl;

import bg.nbu.transportcompany.dao.VehicleDao;
import bg.nbu.transportcompany.entity.Vehicle;
import bg.nbu.transportcompany.entity.VehicleType;

import javax.persistence.EntityManager;
import java.util.List;
import java.util.Optional;

public class VehicleDaoImpl implements VehicleDao {

    private final EntityManager em;

    public VehicleDaoImpl(EntityManager em) {
        this.em = em;
    }

    @Override
    public Vehicle save(Vehicle vehicle) {
        em.persist(vehicle);
        return vehicle;
    }

    @Override
    public Optional<Vehicle> findById(Long id) {
        return Optional.ofNullable(em.find(Vehicle.class, id));
    }

    @Override
    public List<Vehicle> findAll() {
        return em.createQuery("SELECT v FROM Vehicle v ORDER BY v.plateNumber ASC", Vehicle.class)
                .getResultList();
    }

    @Override
    public List<Vehicle> findAllByCompany(Long companyId) {
        return em.createQuery("SELECT v FROM Vehicle v WHERE v.company.id = :companyId ORDER BY v.plateNumber ASC", Vehicle.class)
                .setParameter("companyId", companyId)
                .getResultList();
    }

    @Override
    public Vehicle updateById(Long id, String plateNumber, VehicleType type, int seatCapacity, double maxLoadKg) {
        Vehicle v = em.find(Vehicle.class, id);
        if (v == null) {
            return null;
        }
        v.setPlateNumber(plateNumber);
        v.setType(type);
        v.setSeatCapacity(seatCapacity);
        v.setMaxLoadKg(maxLoadKg);
        return v;
    }

    @Override
    public void deleteById(Long id) {
        Vehicle v = em.find(Vehicle.class, id);
        if (v != null) {
            em.remove(v);
        }
    }
}
