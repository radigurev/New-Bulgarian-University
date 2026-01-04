package bg.nbu.transportcompany.dto;

import bg.nbu.transportcompany.entity.DriverQualification;
import bg.nbu.transportcompany.entity.EmployeeRole;

import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.PositiveOrZero;
import javax.validation.constraints.Size;
import java.math.BigDecimal;

public class EmployeeDTO {

    private Long id;

    @NotNull
    private Long companyId;

    @NotBlank
    @Size(max = 80)
    private String firstName;

    @NotBlank
    @Size(max = 80)
    private String lastName;

    @NotNull
    private EmployeeRole role;

    private DriverQualification qualification;

    @NotNull
    @PositiveOrZero
    private BigDecimal salary;

    public EmployeeDTO() {
    }

    public Long getId() {
        return id;
    }

    public Long getCompanyId() {
        return companyId;
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

    public void setId(Long id) {
        this.id = id;
    }

    public void setCompanyId(Long companyId) {
        this.companyId = companyId;
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

    public String getFullName() {
        return firstName + " " + lastName;
    }
}
