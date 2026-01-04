package bg.nbu.transportcompany.service.impl;

import bg.nbu.transportcompany.dao.*;
import bg.nbu.transportcompany.dao.impl.*;
import bg.nbu.transportcompany.dto.TransportDTO;
import bg.nbu.transportcompany.entity.*;
import bg.nbu.transportcompany.exception.EntityNotFoundException;
import bg.nbu.transportcompany.exception.InvalidDataException;
import bg.nbu.transportcompany.mapper.DtoMapper;
import bg.nbu.transportcompany.service.TransportService;
import bg.nbu.transportcompany.util.JPAUtil;
import bg.nbu.transportcompany.util.TxUtil;
import bg.nbu.transportcompany.util.ValidationUtil;

import javax.persistence.EntityManager;
import java.util.List;
import java.util.stream.Collectors;

public class TransportServiceImpl implements TransportService {

    @Override
    public TransportDTO create(TransportDTO dto) {
        ValidationUtil.validateOrThrow(dto);

        if (dto.getArrivalDate().isBefore(dto.getDepartureDate())) {
            throw new InvalidDataException("arrivalDate cannot be before departureDate");
        }

        EntityManager em = JPAUtil.getEntityManager();
        try {
            return TxUtil.doInTransaction(em, e -> {
                CompanyDao companyDao = new CompanyDaoImpl(e);
                ClientDao clientDao = new ClientDaoImpl(e);
                EmployeeDao employeeDao = new EmployeeDaoImpl(e);
                VehicleDao vehicleDao = new VehicleDaoImpl(e);
                TransportDao transportDao = new TransportDaoImpl(e);

                TransportCompany company = companyDao.findById(dto.getCompanyId())
                        .orElseThrow(() -> new EntityNotFoundException("Company not found. id=" + dto.getCompanyId()));
                Client client = clientDao.findById(dto.getClientId())
                        .orElseThrow(() -> new EntityNotFoundException("Client not found. id=" + dto.getClientId()));
                Employee driver = employeeDao.findById(dto.getDriverId())
                        .orElseThrow(() -> new EntityNotFoundException("Employee (driver) not found. id=" + dto.getDriverId()));
                Vehicle vehicle = vehicleDao.findById(dto.getVehicleId())
                        .orElseThrow(() -> new EntityNotFoundException("Vehicle not found. id=" + dto.getVehicleId()));

                if (!company.getId().equals(driver.getCompany().getId())) {
                    throw new InvalidDataException("Driver must belong to the selected company.");
                }
                if (!company.getId().equals(vehicle.getCompany().getId())) {
                    throw new InvalidDataException("Vehicle must belong to the selected company.");
                }
                if (driver.getRole() != EmployeeRole.DRIVER) {
                    throw new InvalidDataException("Selected employee is not a DRIVER.");
                }

                Transport transport = new Transport(
                        dto.getType(),
                        dto.getOrigin(),
                        dto.getDestination(),
                        dto.getDepartureDate(),
                        dto.getArrivalDate(),
                        dto.getPassengerCount(),
                        dto.getCargoWeightKg(),
                        dto.getPrice(),
                        dto.getPaymentStatus()
                );
                transport.setCompany(company);
                transport.setClient(client);
                transport.setDriver(driver);
                transport.setVehicle(vehicle);

                ValidationUtil.validateOrThrow(transport);

                transportDao.save(transport);
                return DtoMapper.toDto(transport);
            });
        }finally {
            em.close();
        }
    }

    @Override
    public List<TransportDTO> getAll() {
        EntityManager em = JPAUtil.getEntityManager();
        try {
            TransportDao dao = new TransportDaoImpl(em);
            return dao.findAll().stream().map(DtoMapper::toDto).collect(Collectors.toList());
        }finally {
            em.close();
        }
    }

    @Override
    public TransportDTO getById(Long id) {
        EntityManager em = JPAUtil.getEntityManager();
        try {
            TransportDao dao = new TransportDaoImpl(em);
            Transport t = dao.findById(id).orElseThrow(() -> new EntityNotFoundException("Transport not found. id=" + id));
            return DtoMapper.toDto(t);
        }finally {
            em.close();
        }
    }

    @Override
    public List<TransportDTO> findByDestination(String destinationLike) {
        EntityManager em = JPAUtil.getEntityManager();
        try {
            TransportDao dao = new TransportDaoImpl(em);
            return dao.findByDestination(destinationLike).stream().map(DtoMapper::toDto).collect(Collectors.toList());
        }finally {
            em.close();
        }
    }

    @Override
    public TransportDTO update(Long id, TransportDTO dto) {
        ValidationUtil.validateOrThrow(dto);

        if (dto.getArrivalDate().isBefore(dto.getDepartureDate())) {
            throw new InvalidDataException("arrivalDate cannot be before departureDate");
        }

        EntityManager em = JPAUtil.getEntityManager();
        try {
            return TxUtil.doInTransaction(em, e -> {
                TransportDao dao = new TransportDaoImpl(e);
                Transport updated = dao.updateById(
                        id,
                        dto.getOrigin(),
                        dto.getDestination(),
                        dto.getDepartureDate(),
                        dto.getArrivalDate(),
                        dto.getPassengerCount(),
                        dto.getCargoWeightKg(),
                        dto.getPrice(),
                        dto.getPaymentStatus()
                );
                if (updated == null) {
                    throw new EntityNotFoundException("Transport not found. id=" + id);
                }

                ValidationUtil.validateOrThrow(updated);

                return DtoMapper.toDto(updated);
            });
        }finally {
            em.close();
        }
    }

    @Override
    public void delete(Long id) {
        EntityManager em = JPAUtil.getEntityManager();
        try {
            TxUtil.doInTransaction(em, e -> {
                TransportDao dao = new TransportDaoImpl(e);
                if (dao.findById(id).isEmpty()) {
                    throw new EntityNotFoundException("Transport not found. id=" + id);
                }
                dao.deleteById(id);
                return null;
            });
        }finally {
            em.close();
        }
    }

    @Override
    public TransportDTO markPaid(Long id) {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            return TxUtil.doInTransaction(em, e -> {
                TransportDao dao = new TransportDaoImpl(e);
                Transport updated = dao.markPaid(id);
                if (updated == null) {
                    throw new EntityNotFoundException("Transport not found. id=" + id);
                }
                return DtoMapper.toDto(updated);
            });
        }finally {
            em.close();
        }
    }
}
