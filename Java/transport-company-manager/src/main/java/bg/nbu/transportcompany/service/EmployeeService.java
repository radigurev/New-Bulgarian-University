package bg.nbu.transportcompany.service;

import bg.nbu.transportcompany.dto.EmployeeDTO;
import bg.nbu.transportcompany.entity.DriverQualification;

import java.math.BigDecimal;
import java.util.List;

public interface EmployeeService {

    EmployeeDTO create(EmployeeDTO dto);

    List<EmployeeDTO> getAll();

    List<EmployeeDTO> getAllByCompany(Long companyId);

    EmployeeDTO getById(Long id);

    EmployeeDTO update(Long id, EmployeeDTO dto);

    void delete(Long id);

    List<EmployeeDTO> findDriversByQualificationAndSalary(DriverQualification qualification, BigDecimal minSalary, BigDecimal maxSalary);
}
