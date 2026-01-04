package bg.nbu.transportcompany.dao;

import bg.nbu.transportcompany.entity.TransportCompany;
import bg.nbu.transportcompany.report.CompanyRevenueDTO;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;
import java.util.Optional;

public interface CompanyDao {

    TransportCompany save(TransportCompany company);

    Optional<TransportCompany> findById(Long id);

    List<TransportCompany> findAll();

    TransportCompany updateById(Long id, String name, String address);

    void deleteById(Long id);

    BigDecimal getTotalRevenue(Long companyId);

    List<CompanyRevenueDTO> findAllSortedByRevenue();
}
