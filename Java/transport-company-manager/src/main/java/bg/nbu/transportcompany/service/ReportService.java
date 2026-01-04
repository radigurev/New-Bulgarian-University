package bg.nbu.transportcompany.service;

import bg.nbu.transportcompany.report.CompanyRevenueDTO;
import bg.nbu.transportcompany.report.DriverRevenueDTO;
import bg.nbu.transportcompany.report.DriverTransportCountDTO;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;

public interface ReportService {

    long getTotalTransportsCount();

    BigDecimal getTotalPaidRevenue();

    List<DriverTransportCountDTO> getTransportsPerDriver();

    List<CompanyRevenueDTO> getRevenuePerCompany(LocalDate start, LocalDate end);

    List<DriverRevenueDTO> getRevenuePerDriver(LocalDate start, LocalDate end);
}
