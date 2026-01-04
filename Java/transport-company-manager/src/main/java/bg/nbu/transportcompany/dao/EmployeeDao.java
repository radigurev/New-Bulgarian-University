package bg.nbu.transportcompany.dao;

import bg.nbu.transportcompany.entity.DriverQualification;
import bg.nbu.transportcompany.entity.Employee;
import bg.nbu.transportcompany.entity.EmployeeRole;
import bg.nbu.transportcompany.report.DriverTransportCountDTO;

import java.math.BigDecimal;
import java.util.List;
import java.util.Optional;

public interface EmployeeDao {

    Employee save(Employee employee);

    Optional<Employee> findById(Long id);

    List<Employee> findAll();

    List<Employee> findAllByCompany(Long companyId);

    List<Employee> findDriversByQualificationAndSalary(DriverQualification qualification, BigDecimal minSalary, BigDecimal maxSalary);

    Employee updateById(Long id, String firstName, String lastName, EmployeeRole role, DriverQualification qualification, BigDecimal salary);

    void deleteById(Long id);
}
