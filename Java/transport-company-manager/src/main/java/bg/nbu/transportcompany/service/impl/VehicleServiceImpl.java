package bg.nbu.transportcompany.service.impl;

import bg.nbu.transportcompany.dao.CompanyDao;
import bg.nbu.transportcompany.dao.VehicleDao;
import bg.nbu.transportcompany.dao.impl.CompanyDaoImpl;
import bg.nbu.transportcompany.dao.impl.VehicleDaoImpl;
import bg.nbu.transportcompany.dto.VehicleDTO;
import bg.nbu.transportcompany.entity.TransportCompany;
import bg.nbu.transportcompany.entity.Vehicle;
import bg.nbu.transportcompany.exception.EntityNotFoundException;
import bg.nbu.transportcompany.mapper.DtoMapper;
import bg.nbu.transportcompany.service.VehicleService;
import bg.nbu.transportcompany.util.JPAUtil;
import bg.nbu.transportcompany.util.TxUtil;
import bg.nbu.transportcompany.util.ValidationUtil;

import javax.persistence.EntityManager;
import java.util.List;
import java.util.stream.Collectors;

public class VehicleServiceImpl implements VehicleService {

    @Override
    public VehicleDTO create(VehicleDTO dto) {
        ValidationUtil.validateOrThrow(dto);
        EntityManager em = JPAUtil.getEntityManager();

        try {
            return TxUtil.doInTransaction(em, e -> {
                CompanyDao companyDao = new CompanyDaoImpl(e);
                TransportCompany company = companyDao.findById(dto.getCompanyId())
                        .orElseThrow(() -> new EntityNotFoundException("Company not found. id=" + dto.getCompanyId()));

                Vehicle vehicle = new Vehicle(dto.getPlateNumber(), dto.getType(), dto.getSeatCapacity(), dto.getMaxLoadKg());
                company.addVehicle(vehicle);

                VehicleDao dao = new VehicleDaoImpl(e);
                dao.save(vehicle);

                return DtoMapper.toDto(vehicle);
            });
        }finally {
            em.close();
        }
    }

    @Override
    public List<VehicleDTO> getAll() {
        EntityManager em = JPAUtil.getEntityManager();
        try {
            VehicleDao dao = new VehicleDaoImpl(em);
            return dao.findAll().stream().map(DtoMapper::toDto).collect(Collectors.toList());
        }finally {
            em.close();
        }
    }

    @Override
    public List<VehicleDTO> getAllByCompany(Long companyId) {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            VehicleDao dao = new VehicleDaoImpl(em);
            return dao.findAllByCompany(companyId).stream().map(DtoMapper::toDto).collect(Collectors.toList());
        }finally {
            em.close();
        }
    }

    @Override
    public VehicleDTO getById(Long id) {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            VehicleDao dao = new VehicleDaoImpl(em);
            Vehicle v = dao.findById(id).orElseThrow(() -> new EntityNotFoundException("Vehicle not found. id=" + id));
            return DtoMapper.toDto(v);
        }finally {
            em.close();
        }
    }

    @Override
    public VehicleDTO update(Long id, VehicleDTO dto) {
        ValidationUtil.validateOrThrow(dto);

        EntityManager em = JPAUtil.getEntityManager();

        try {
            return TxUtil.doInTransaction(em, e -> {
                VehicleDao dao = new VehicleDaoImpl(e);
                Vehicle updated = dao.updateById(id, dto.getPlateNumber(), dto.getType(), dto.getSeatCapacity(), dto.getMaxLoadKg());
                if (updated == null) {
                    throw new EntityNotFoundException("Vehicle not found. id=" + id);
                }
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
                VehicleDao dao = new VehicleDaoImpl(e);
                if (dao.findById(id).isEmpty()) {
                    throw new EntityNotFoundException("Vehicle not found. id=" + id);
                }
                dao.deleteById(id);
                return null;
            });
        }finally {
            em.close();
        }
    }
}
