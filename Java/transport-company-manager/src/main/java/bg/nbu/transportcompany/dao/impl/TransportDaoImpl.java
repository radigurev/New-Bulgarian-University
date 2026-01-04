package bg.nbu.transportcompany.dao.impl;

import bg.nbu.transportcompany.dao.TransportDao;
import bg.nbu.transportcompany.entity.PaymentStatus;
import bg.nbu.transportcompany.entity.Transport;
import bg.nbu.transportcompany.report.CompanyRevenueDTO;
import bg.nbu.transportcompany.report.DriverRevenueDTO;
import bg.nbu.transportcompany.report.DriverTransportCountDTO;

import javax.persistence.EntityManager;
import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;
import java.util.Optional;

public class TransportDaoImpl implements TransportDao {

    private final EntityManager em;

    public TransportDaoImpl(EntityManager em) {
        this.em = em;
    }

    @Override
    public Transport save(Transport transport) {
        em.persist(transport);
        return transport;
    }

    @Override
    public Optional<Transport> findById(Long id) {
        return Optional.ofNullable(em.find(Transport.class, id));
    }

    @Override
    public List<Transport> findAll() {
        return em.createQuery("SELECT t FROM Transport t ORDER BY t.departureDate DESC, t.id DESC", Transport.class)
                .getResultList();
    }

    @Override
    public List<Transport> findByDestination(String destinationLike) {
        return em.createQuery(
                        "SELECT t FROM Transport t " +
                                "WHERE LOWER(t.destination) LIKE LOWER(:dest) " +
                                "ORDER BY t.departureDate DESC",
                        Transport.class)
                .setParameter("dest", "%" + destinationLike + "%")
                .getResultList();
    }

    @Override
    public Transport updateById(Long id, String origin, String destination, LocalDate departureDate, LocalDate arrivalDate,
                                Integer passengerCount, Double cargoWeightKg, BigDecimal price, PaymentStatus paymentStatus) {
        Transport t = em.find(Transport.class, id);
        if (t == null) {
            return null;
        }
        t.setOrigin(origin);
        t.setDestination(destination);
        t.setDepartureDate(departureDate);
        t.setArrivalDate(arrivalDate);
        t.setPassengerCount(passengerCount);
        t.setCargoWeightKg(cargoWeightKg);
        t.setPrice(price);
        t.setPaymentStatus(paymentStatus);
        return t;
    }

    @Override
    public void deleteById(Long id) {
        Transport t = em.find(Transport.class, id);
        if (t != null) {
            em.remove(t);
        }
    }

    @Override
    public Transport markPaid(Long id) {
        Transport t = em.find(Transport.class, id);
        if (t == null) {
            return null;
        }
        t.setPaymentStatus(PaymentStatus.PAID);
        return t;
    }

    @Override
    public long countAll() {
        return em.createQuery("SELECT COUNT(t) FROM Transport t", Long.class).getSingleResult();
    }

    @Override
    public BigDecimal sumPaidRevenue() {
        return em.createQuery("SELECT COALESCE(SUM(t.price), 0) FROM Transport t WHERE t.paymentStatus = :paid", BigDecimal.class)
                .setParameter("paid", PaymentStatus.PAID)
                .getSingleResult();
    }

    @Override
    public List<DriverTransportCountDTO> countTransportsPerDriver() {
        return em.createQuery(
                        "SELECT new bg.nbu.transportcompany.report.DriverTransportCountDTO(" +
                                "e.id, CONCAT(e.firstName, ' ', e.lastName), COUNT(t.id)) " +
                                "FROM Transport t JOIN t.driver e " +
                                "GROUP BY e.id, e.firstName, e.lastName " +
                                "ORDER BY COUNT(t.id) DESC",
                        DriverTransportCountDTO.class)
                .getResultList();
    }

    @Override
    public List<CompanyRevenueDTO> revenuePerCompany(LocalDate startDate, LocalDate endDate) {
        return em.createQuery(
                        "SELECT new bg.nbu.transportcompany.report.CompanyRevenueDTO(" +
                                "c.id, c.name, COALESCE(SUM(t.price), 0)) " +
                                "FROM Transport t JOIN t.company c " +
                                "WHERE t.paymentStatus = :paid AND t.departureDate BETWEEN :start AND :end " +
                                "GROUP BY c.id, c.name " +
                                "ORDER BY COALESCE(SUM(t.price), 0) DESC",
                        CompanyRevenueDTO.class)
                .setParameter("paid", PaymentStatus.PAID)
                .setParameter("start", startDate)
                .setParameter("end", endDate)
                .getResultList();
    }

    @Override
    public List<DriverRevenueDTO> revenuePerDriver(LocalDate startDate, LocalDate endDate) {
        return em.createQuery(
                        "SELECT new bg.nbu.transportcompany.report.DriverRevenueDTO(" +
                                "e.id, CONCAT(e.firstName, ' ', e.lastName), COALESCE(SUM(t.price), 0)) " +
                                "FROM Transport t JOIN t.driver e " +
                                "WHERE t.paymentStatus = :paid AND t.departureDate BETWEEN :start AND :end " +
                                "GROUP BY e.id, e.firstName, e.lastName " +
                                "ORDER BY COALESCE(SUM(t.price), 0) DESC",
                        DriverRevenueDTO.class)
                .setParameter("paid", PaymentStatus.PAID)
                .setParameter("start", startDate)
                .setParameter("end", endDate)
                .getResultList();
    }
}
