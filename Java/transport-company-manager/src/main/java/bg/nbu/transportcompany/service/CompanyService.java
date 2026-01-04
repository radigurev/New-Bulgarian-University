package bg.nbu.transportcompany.service;

import bg.nbu.transportcompany.dto.TransportCompanyDTO;
import bg.nbu.transportcompany.report.CompanyRevenueDTO;

import java.math.BigDecimal;
import java.util.List;

public interface CompanyService {

    TransportCompanyDTO create(TransportCompanyDTO dto);

    List<TransportCompanyDTO> getAll();

    TransportCompanyDTO getById(Long id);

    TransportCompanyDTO update(Long id, TransportCompanyDTO dto);

    void delete(Long id);

    BigDecimal getTotalRevenue(Long companyId);

    List<CompanyRevenueDTO> getCompaniesSortedByRevenue();
}
