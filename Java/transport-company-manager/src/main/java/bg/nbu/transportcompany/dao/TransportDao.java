package bg.nbu.transportcompany.dao;

import bg.nbu.transportcompany.entity.PaymentStatus;
import bg.nbu.transportcompany.entity.Transport;
import bg.nbu.transportcompany.report.CompanyRevenueDTO;
import bg.nbu.transportcompany.report.DriverRevenueDTO;
import bg.nbu.transportcompany.report.DriverTransportCountDTO;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;
import java.util.Optional;

public interface TransportDao {

    Transport save(Transport transport);

    Optional<Transport> findById(Long id);

    List<Transport> findAll();

    List<Transport> findByDestination(String destinationLike);

    Transport updateById(Long id, String origin, String destination, LocalDate departureDate, LocalDate arrivalDate,
                         Integer passengerCount, Double cargoWeightKg, BigDecimal price, PaymentStatus paymentStatus);

    void deleteById(Long id);

    Transport markPaid(Long id);

    long countAll();

    BigDecimal sumPaidRevenue();

    List<DriverTransportCountDTO> countTransportsPerDriver();

    List<CompanyRevenueDTO> revenuePerCompany(LocalDate startDate, LocalDate endDate);

    List<DriverRevenueDTO> revenuePerDriver(LocalDate startDate, LocalDate endDate);
}
