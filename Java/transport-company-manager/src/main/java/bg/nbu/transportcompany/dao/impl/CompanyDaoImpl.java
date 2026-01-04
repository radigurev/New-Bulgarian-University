package bg.nbu.transportcompany.dao.impl;

import bg.nbu.transportcompany.dao.CompanyDao;
import bg.nbu.transportcompany.entity.PaymentStatus;
import bg.nbu.transportcompany.entity.TransportCompany;
import bg.nbu.transportcompany.report.CompanyRevenueDTO;

import javax.persistence.EntityManager;
import java.math.BigDecimal;
import java.util.List;
import java.util.Optional;

public class CompanyDaoImpl implements CompanyDao {

    private final EntityManager em;

    public CompanyDaoImpl(EntityManager em) {
        this.em = em;
    }

    @Override
    public TransportCompany save(TransportCompany company) {
        em.persist(company);
        return company;
    }

    @Override
    public Optional<TransportCompany> findById(Long id) {
        return Optional.ofNullable(em.find(TransportCompany.class, id));
    }

    @Override
    public List<TransportCompany> findAll() {
        return em.createQuery("SELECT c FROM TransportCompany c ORDER BY c.name ASC", TransportCompany.class)
                .getResultList();
    }

    @Override
    public TransportCompany updateById(Long id, String name, String address) {
        TransportCompany c = em.find(TransportCompany.class, id);
        if (c == null) {
            return null;
        }
        c.setName(name);
        c.setAddress(address);
        return c;
    }

    @Override
    public void deleteById(Long id) {
        TransportCompany c = em.find(TransportCompany.class, id);
        if (c != null) {
            em.remove(c);
        }
    }

    @Override
    public BigDecimal getTotalRevenue(Long companyId) {
        BigDecimal sum = em.createQuery(
                        "SELECT COALESCE(SUM(t.price), 0) FROM Transport t " +
                                "WHERE t.company.id = :companyId AND t.paymentStatus = :paid", BigDecimal.class)
                .setParameter("companyId", companyId)
                .setParameter("paid", PaymentStatus.PAID)
                .getSingleResult();
        return sum;
    }

    @Override
    public List<CompanyRevenueDTO> findAllSortedByRevenue() {
        return em.createQuery(
                        "SELECT new bg.nbu.transportcompany.report.CompanyRevenueDTO(" +
                                "c.id, c.name, COALESCE(SUM(t.price), 0)) " +
                                "FROM TransportCompany c " +
                                "LEFT JOIN c.transports t ON t.paymentStatus = :paid " +
                                "GROUP BY c.id, c.name " +
                                "ORDER BY COALESCE(SUM(t.price), 0) DESC, c.name ASC",
                        CompanyRevenueDTO.class)
                .setParameter("paid", PaymentStatus.PAID)
                .getResultList();
    }
}
