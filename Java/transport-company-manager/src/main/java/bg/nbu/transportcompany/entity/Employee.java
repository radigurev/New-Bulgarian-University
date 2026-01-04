package bg.nbu.transportcompany.entity;

import javax.persistence.*;
import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.PositiveOrZero;
import javax.validation.constraints.Size;
import java.math.BigDecimal;

@Entity
@Table(name = "employee")
public class Employee {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @NotBlank
    @Size(max = 80)
    @Column(nullable = false, length = 80)
    private String firstName;

    @NotBlank
    @Size(max = 80)
    @Column(nullable = false, length = 80)
    private String lastName;

    @NotNull
    @Enumerated(EnumType.STRING)
    @Column(nullable = false, length = 40)
    private EmployeeRole role;

    @Enumerated(EnumType.STRING)
    @Column(length = 40)
    private DriverQualification qualification;

    @NotNull
    @PositiveOrZero
    @Column(nullable = false, precision = 12, scale = 2)
    private BigDecimal salary;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "company_id", nullable = false)
    private TransportCompany company;

    public Employee() {
    }

    public Employee(String firstName, String lastName, EmployeeRole role, DriverQualification qualification, BigDecimal salary) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.role = role;
        this.qualification = qualification;
        this.salary = salary;
    }

    public Long getId() {
        return id;
    }

    public String getFirstName() {
        return firstName;
    }

    public String getLastName() {
        return lastName;
    }

    public EmployeeRole getRole() {
        return role;
    }

    public DriverQualification getQualification() {
        return qualification;
    }

    public BigDecimal getSalary() {
        return salary;
    }

    public TransportCompany getCompany() {
        return company;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }

    public void setLastName(String lastName) {
        this.lastName = lastName;
    }

    public void setRole(EmployeeRole role) {
        this.role = role;
    }

    public void setQualification(DriverQualification qualification) {
        this.qualification = qualification;
    }

    public void setSalary(BigDecimal salary) {
        this.salary = salary;
    }

    public void setCompany(TransportCompany company) {
        this.company = company;
    }

    public String getFullName() {
        return firstName + " " + lastName;
    }
}
