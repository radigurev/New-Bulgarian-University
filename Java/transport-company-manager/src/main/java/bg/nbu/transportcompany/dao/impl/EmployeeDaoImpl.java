package bg.nbu.transportcompany.dao.impl;

import bg.nbu.transportcompany.dao.EmployeeDao;
import bg.nbu.transportcompany.entity.DriverQualification;
import bg.nbu.transportcompany.entity.Employee;
import bg.nbu.transportcompany.entity.EmployeeRole;

import javax.persistence.EntityManager;
import java.math.BigDecimal;
import java.util.List;
import java.util.Optional;

public class EmployeeDaoImpl implements EmployeeDao {

    private final EntityManager em;

    public EmployeeDaoImpl(EntityManager em) {
        this.em = em;
    }

    @Override
    public Employee save(Employee employee) {
        em.persist(employee);
        return employee;
    }

    @Override
    public Optional<Employee> findById(Long id) {
        return Optional.ofNullable(em.find(Employee.class, id));
    }

    @Override
    public List<Employee> findAll() {
        return em.createQuery("SELECT e FROM Employee e ORDER BY e.lastName ASC, e.firstName ASC", Employee.class)
                .getResultList();
    }

    @Override
    public List<Employee> findAllByCompany(Long companyId) {
        return em.createQuery("SELECT e FROM Employee e WHERE e.company.id = :companyId ORDER BY e.lastName ASC, e.firstName ASC",
                        Employee.class)
                .setParameter("companyId", companyId)
                .getResultList();
    }

    @Override
    public List<Employee> findDriversByQualificationAndSalary(DriverQualification qualification, BigDecimal minSalary, BigDecimal maxSalary) {
        return em.createQuery(
                        "SELECT e FROM Employee e " +
                                "WHERE e.role = :role AND e.qualification = :q " +
                                "AND e.salary BETWEEN :min AND :max " +
                                "ORDER BY e.salary DESC, e.lastName ASC",
                        Employee.class)
                .setParameter("role", EmployeeRole.DRIVER)
                .setParameter("q", qualification)
                .setParameter("min", minSalary)
                .setParameter("max", maxSalary)
                .getResultList();
    }

    @Override
    public Employee updateById(Long id, String firstName, String lastName, EmployeeRole role, DriverQualification qualification, BigDecimal salary) {
        Employee e = em.find(Employee.class, id);
        if (e == null) {
            return null;
        }
        e.setFirstName(firstName);
        e.setLastName(lastName);
        e.setRole(role);
        e.setQualification(qualification);
        e.setSalary(salary);
        return e;
    }

    @Override
    public void deleteById(Long id) {
        Employee e = em.find(Employee.class, id);
        if (e != null) {
            em.remove(e);
        }
    }
}
