package bg.nbu.transportcompany.service.impl;

import bg.nbu.transportcompany.dao.TransportDao;
import bg.nbu.transportcompany.dao.impl.TransportDaoImpl;
import bg.nbu.transportcompany.report.CompanyRevenueDTO;
import bg.nbu.transportcompany.report.DriverRevenueDTO;
import bg.nbu.transportcompany.report.DriverTransportCountDTO;
import bg.nbu.transportcompany.service.ReportService;
import bg.nbu.transportcompany.util.JPAUtil;

import javax.persistence.EntityManager;
import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;

public class ReportServiceImpl implements ReportService {

    @Override
    public long getTotalTransportsCount() {
        EntityManager em = JPAUtil.getEntityManager();
        try {
            TransportDao dao = new TransportDaoImpl(em);
            return dao.countAll();
        }finally {
            em.close();
        }
    }

    @Override
    public BigDecimal getTotalPaidRevenue() {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            TransportDao dao = new TransportDaoImpl(em);
            return dao.sumPaidRevenue();
        }finally {
            em.close();
        }
    }

    @Override
    public List<DriverTransportCountDTO> getTransportsPerDriver() {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            TransportDao dao = new TransportDaoImpl(em);
            return dao.countTransportsPerDriver();
        }finally {
            em.close();
        }
    }

    @Override
    public List<CompanyRevenueDTO> getRevenuePerCompany(LocalDate start, LocalDate end) {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            TransportDao dao = new TransportDaoImpl(em);
            return dao.revenuePerCompany(start, end);
        }finally {
            em.close();
        }
    }

    @Override
    public List<DriverRevenueDTO> getRevenuePerDriver(LocalDate start, LocalDate end) {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            TransportDao dao = new TransportDaoImpl(em);
            return dao.revenuePerDriver(start, end);
        }finally {
            em.close();
        }
    }
}
