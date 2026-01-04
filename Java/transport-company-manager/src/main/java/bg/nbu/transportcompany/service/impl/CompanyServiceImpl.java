package bg.nbu.transportcompany.service.impl;

import bg.nbu.transportcompany.dao.CompanyDao;
import bg.nbu.transportcompany.dao.impl.CompanyDaoImpl;
import bg.nbu.transportcompany.dto.TransportCompanyDTO;
import bg.nbu.transportcompany.entity.TransportCompany;
import bg.nbu.transportcompany.exception.EntityNotFoundException;
import bg.nbu.transportcompany.mapper.DtoMapper;
import bg.nbu.transportcompany.report.CompanyRevenueDTO;
import bg.nbu.transportcompany.service.CompanyService;
import bg.nbu.transportcompany.util.JPAUtil;
import bg.nbu.transportcompany.util.TxUtil;
import bg.nbu.transportcompany.util.ValidationUtil;

import javax.persistence.EntityManager;
import java.math.BigDecimal;
import java.util.List;
import java.util.stream.Collectors;

public class CompanyServiceImpl implements CompanyService {

    @Override
    public TransportCompanyDTO create(TransportCompanyDTO dto) {
        ValidationUtil.validateOrThrow(dto);
        EntityManager em = JPAUtil.getEntityManager();
        try {
            return TxUtil.doInTransaction(em, e -> {
                CompanyDao dao = new CompanyDaoImpl(e);
                TransportCompany entity = DtoMapper.toEntity(dto);
                dao.save(entity);
                return DtoMapper.toDto(entity);
            });
        }finally {
            em.close();
        }
    }

    @Override
    public List<TransportCompanyDTO> getAll() {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            CompanyDao dao = new CompanyDaoImpl(em);
            return dao.findAll().stream().map(DtoMapper::toDto).collect(Collectors.toList());
        }finally {
            em.close();
        }
    }

    @Override
    public TransportCompanyDTO getById(Long id) {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            CompanyDao dao = new CompanyDaoImpl(em);
            TransportCompany c = dao.findById(id).orElseThrow(() -> new EntityNotFoundException("Company not found. id=" + id));
            return DtoMapper.toDto(c);
        }finally {
            em.close();
        }
    }

    @Override
    public TransportCompanyDTO update(Long id, TransportCompanyDTO dto) {
        ValidationUtil.validateOrThrow(dto);

        EntityManager em = JPAUtil.getEntityManager();
        try {
            return TxUtil.doInTransaction(em, e -> {
                CompanyDao dao = new CompanyDaoImpl(e);
                TransportCompany updated = dao.updateById(id, dto.getName(), dto.getAddress());
                if (updated == null) {
                    throw new EntityNotFoundException("Company not found. id=" + id);
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
                CompanyDao dao = new CompanyDaoImpl(e);
                if (dao.findById(id).isEmpty()) {
                    throw new EntityNotFoundException("Company not found. id=" + id);
                }
                dao.deleteById(id);
                return null;
            });
        }finally {
            em.close();
        }
    }

    @Override
    public BigDecimal getTotalRevenue(Long companyId) {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            CompanyDao dao = new CompanyDaoImpl(em);
            if (dao.findById(companyId).isEmpty()) {
                throw new EntityNotFoundException("Company not found. id=" + companyId);
            }
            return dao.getTotalRevenue(companyId);
        }finally {
            em.close();
        }
    }

    @Override
    public List<CompanyRevenueDTO> getCompaniesSortedByRevenue() {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            CompanyDao dao = new CompanyDaoImpl(em);
            return dao.findAllSortedByRevenue();
        }finally {
            em.close();
        }
    }
}
