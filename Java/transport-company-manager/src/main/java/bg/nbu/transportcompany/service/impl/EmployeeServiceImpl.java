package bg.nbu.transportcompany.service.impl;

import bg.nbu.transportcompany.dao.CompanyDao;
import bg.nbu.transportcompany.dao.EmployeeDao;
import bg.nbu.transportcompany.dao.impl.CompanyDaoImpl;
import bg.nbu.transportcompany.dao.impl.EmployeeDaoImpl;
import bg.nbu.transportcompany.dto.EmployeeDTO;
import bg.nbu.transportcompany.entity.DriverQualification;
import bg.nbu.transportcompany.entity.Employee;
import bg.nbu.transportcompany.entity.TransportCompany;
import bg.nbu.transportcompany.exception.EntityNotFoundException;
import bg.nbu.transportcompany.mapper.DtoMapper;
import bg.nbu.transportcompany.service.EmployeeService;
import bg.nbu.transportcompany.util.JPAUtil;
import bg.nbu.transportcompany.util.TxUtil;
import bg.nbu.transportcompany.util.ValidationUtil;

import javax.persistence.EntityManager;
import java.math.BigDecimal;
import java.util.List;
import java.util.stream.Collectors;

public class EmployeeServiceImpl implements EmployeeService {

    @Override
    public EmployeeDTO create(EmployeeDTO dto) {
        ValidationUtil.validateOrThrow(dto);
        EntityManager em = JPAUtil.getEntityManager();

        try {
            return TxUtil.doInTransaction(em, e -> {
                CompanyDao companyDao = new CompanyDaoImpl(e);
                TransportCompany company = companyDao.findById(dto.getCompanyId())
                        .orElseThrow(() -> new EntityNotFoundException("Company not found. id=" + dto.getCompanyId()));

                Employee employee = new Employee(dto.getFirstName(), dto.getLastName(), dto.getRole(), dto.getQualification(), dto.getSalary());
                company.addEmployee(employee);

                EmployeeDao dao = new EmployeeDaoImpl(e);
                dao.save(employee);

                return DtoMapper.toDto(employee);
            });
        }finally {
            em.close();
        }
    }

    @Override
    public List<EmployeeDTO> getAll() {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            EmployeeDao dao = new EmployeeDaoImpl(em);
            return dao.findAll().stream().map(DtoMapper::toDto).collect(Collectors.toList());
        }finally {
            em.close();
        }
    }

    @Override
    public List<EmployeeDTO> getAllByCompany(Long companyId) {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            EmployeeDao dao = new EmployeeDaoImpl(em);
            return dao.findAllByCompany(companyId).stream().map(DtoMapper::toDto).collect(Collectors.toList());
        }finally {
            em.close();
        }
    }

    @Override
    public EmployeeDTO getById(Long id) {
        EntityManager em = JPAUtil.getEntityManager();
        try {
            EmployeeDao dao = new EmployeeDaoImpl(em);
            Employee e = dao.findById(id).orElseThrow(() -> new EntityNotFoundException("Employee not found. id=" + id));
            return DtoMapper.toDto(e);
        }finally {
            em.close();
        }
    }

    @Override
    public EmployeeDTO update(Long id, EmployeeDTO dto) {
        ValidationUtil.validateOrThrow(dto);

        EntityManager em = JPAUtil.getEntityManager();
        try {
            return TxUtil.doInTransaction(em, e -> {
                EmployeeDao dao = new EmployeeDaoImpl(e);
                Employee updated = dao.updateById(id, dto.getFirstName(), dto.getLastName(), dto.getRole(), dto.getQualification(), dto.getSalary());
                if (updated == null) {
                    throw new EntityNotFoundException("Employee not found. id=" + id);
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
                EmployeeDao dao = new EmployeeDaoImpl(e);
                if (dao.findById(id).isEmpty()) {
                    throw new EntityNotFoundException("Employee not found. id=" + id);
                }
                dao.deleteById(id);
                return null;
            });
        }finally {
            em.close();
        }
    }

    @Override
    public List<EmployeeDTO> findDriversByQualificationAndSalary(DriverQualification qualification, BigDecimal minSalary, BigDecimal maxSalary) {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            EmployeeDao dao = new EmployeeDaoImpl(em);
            return dao.findDriversByQualificationAndSalary(qualification, minSalary, maxSalary)
                    .stream().map(DtoMapper::toDto).collect(Collectors.toList());
        }finally {
            em.close();
        }
    }
}
